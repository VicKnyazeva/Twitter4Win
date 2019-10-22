using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TwitterApp.EFClasses;
using TwitterApp.Helpers;
using TwitterApp.ViewModels;
using System.Data.Entity.Infrastructure;

namespace TwitterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = await PreloadServiceData();
            GetServiceData(vm);
        }

        class MockData
        {
            public TwitterTrendViewModel[] Trends { get; set; }

            public TwitterPostViewModel[] Posts { get; set; }
        }

        private async Task<MainViewModel> PreloadServiceData()
        {
            TaskScheduler uiTaskSheduler = TaskScheduler.FromCurrentSynchronizationContext();
            MainViewModel vm = null;
            Exception exception = null;

            try
            {
                vm = new MainViewModel(true);

                var loadTrends = TwitterHelper.GetTrendsAsync()
                    .ContinueWith(loadTask =>
                {
                    if (exception != null) return;

                    exception = loadTask.Exception;
                    if (exception != null) return;

                    try
                    {
                        var trendsCollection = loadTask.Result
                            .Where(trend => trend.Name.StartsWith("#"))
                            .Take<TwitterTrendViewModel>(10);

                        using (var db = new TwitterContext())
                        {
                            foreach (var t in trendsCollection)
                            {
                                if (db.Trends.Any(tt => tt.TrendName == t.Name))
                                    continue;

                                db.Trends.Add(new TwitterTrend
                                {
                                    TrendName = t.Name
                                });
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });


                var loadTweets = TwitterHelper.GetTweetsAsync()

                .ContinueWith(loadTask =>
                {
                    if (exception != null) return;

                    exception = loadTask.Exception;
                    if (exception != null) return;

                    try
                    {
                        var tweetsCollection = loadTask.Result.Take(10);

                        using (var db = new TwitterContext())
                        {
                            using (var transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    var users = db.Users.ToDictionary(u => u.UserName);
                                    foreach (var tw in tweetsCollection)
                                    {
                                        TwitterUser user;
                                        if (!users.TryGetValue(tw.User.UserName, out user))
                                        {
                                            user = new TwitterUser()
                                            {
                                                UserName = tw.User.UserName,
                                                UserScreenName = tw.User.UserScreenName,
                                                FavouritesCount = tw.User.FavouritesCount,
                                                FollowersCount = tw.User.FollowersCount,
                                                FriendsCount = tw.User.FriendsCount,
                                                Description = tw.User.Description,
                                                ProfileImageUrl = tw.User.ProfileImageUrl
                                            };
                                            users.Add(tw.User.UserName, user);
                                            db.Users.Add(user);
                                        }

                                        user.Posts.Add(
                                            new TwitterPost
                                            {
                                                Text = tw.Text,
                                                CreatedDate = tw.CreatedDate,
                                                RetweetCount = tw.RetweetCount
                                            });
                                    }

                                    db.SaveChanges();
                                    transaction.Commit();
                                }
                                catch
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }, uiTaskSheduler);

                var loadUserInfo = TwitterHelper.GetUserInfoAsync()
                    .ContinueWith(loadTask =>
                {
                    if (exception != null) return;

                    exception = loadTask.Exception;
                    if (exception != null) return;

                    try
                    {
                        var userInfo = loadTask.Result;
                        if (userInfo != null)
                            vm.User = userInfo;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });

                await Task.WhenAll(loadTrends, loadTweets, loadUserInfo); // три задачи в параллель!!!
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
            {
                return vm;
            }
            else
            {
                string error;
                if (exception is TwitterException)
                {
                    error = string.Format("Произошло специфическое исключение {0}:\n\t{1}", exception.GetType().Name, exception.Message);
                }
                else
                {
                    error = string.Format("Произошло исключение {0}:\n\t{1}", exception.GetType().Name, exception.Message);
                }
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            return null;
        }

        private void GetServiceData(MainViewModel vm)
        {
            Exception exception = null;
            try
            {
                Dictionary<int, TwitterUserViewModel> authors = new Dictionary<int, TwitterUserViewModel>();
                using (var db = new TwitterContext())
                {
                    var trends =
                        from t in db.Trends
                        select new TwitterTrendViewModel()
                        {
                            Name = t.TrendName
                        };

                    foreach (var trend in trends.Take(10))
                        vm.Trends.Add(trend);

                    IQueryable<TwitterPost> allPosts = db.Posts.Include(nameof(TwitterPost.Author));

                    var posts =
                        from p in allPosts
                        orderby p.CreatedDate descending, p.Id
                        select p;

                    foreach (var post in posts.Take(10).ToArray())
                    {
                        TwitterUserViewModel author;
                        if (!authors.TryGetValue(post.AuthorId, out author))
                        {
                            author = new TwitterUserViewModel()
                            {
                                UserName = post.Author.UserName,
                                UserScreenName = post.Author.UserScreenName,
                                ProfileImageUrl = post.Author.ProfileImageUrl
                            };
                            authors.Add(post.AuthorId, author);
                        }
                        vm.Posts.Add(new TwitterPostViewModel
                        {
                            Author = author,
                            CreatedDate = post.CreatedDate,
                            Text = post.Text
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
            {
                this.DataContext = vm;
            }
            else
            {
                string error;
                if (exception is TwitterException)
                {
                    error = string.Format("Произошло специфическое исключение {0}:\n\t{1}", exception.GetType().Name, exception.Message);
                }
                else
                {
                    error = string.Format("Произошло исключение {0}:\n\t{1}", exception.GetType().Name, exception.Message);
                }
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
