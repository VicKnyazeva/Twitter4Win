using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TwitterApp.Helpers;
using TwitterApp.ViewModels;

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

        private async Task<MainViewModel> PreloadServiceData()
        {
            TaskScheduler uiTaskSheduler = TaskScheduler.FromCurrentSynchronizationContext();
            MainViewModel vm = null;
            Exception exception = null;
            try
            {
                vm = new MainViewModel(true);

                var loadTrends = TwitterHelper.GetTrendsAsync().ContinueWith(loadTask =>
                {
                    if (exception != null) return;

                    exception = loadTask.Exception;
                    if (exception != null) return;

                    try
                    {
                        // Урок 2. Применить LINQ для фильтрации трендов, начинающихся на #, ...
                        //
                        var trendsCollection = loadTask.Result
                            .Where(trend => trend.Name.StartsWith("#"))
                            .Take<TwitterTrendViewModel>(10);

                        foreach (var t in trendsCollection)
                        {
                            vm.Trends.Add(new TwitterTrendViewModel { Name = t.Name });
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });

                var loadTweets = TwitterHelper.GetTweetsAsync().ContinueWith(loadTask =>
                {
                    if (exception != null) return;

                    exception = loadTask.Exception;
                    if (exception != null) return;

                    try
                    {
                        var tweetsCollection = loadTask.Result.Take(10);

                        using (var connection = DbHelper.CreateConnection())
                        {
                            connection.Open();
                            
                            connection.CleanupContent();

                            foreach (var tw in tweetsCollection)
                            {
                                int? userId = connection.FindUser(tw.User.UserName);
                                if (!userId.HasValue)
                                {
                                    userId = connection.InsertUser(
                                        tw.User.UserName,
                                        tw.User.UserScreenName,
                                        tw.User.FavouritesCount,
                                        tw.User.FollowersCount,
                                        tw.User.FriendsCount,
                                        tw.User.Description,
                                        tw.User.ProfileImageUrl);
                                }

                                int postId = connection.InsertPost(userId.Value, tw.Text, tw.CreatedDate, tw.RetweetCount);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }, uiTaskSheduler);

                var loadUserInfo = TwitterHelper.GetUserInfoAsync().ContinueWith(loadTask =>
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
                using (var connection = DbHelper.CreateConnection())
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
SELECT TOP (10) 
    [t0].[Id],
    [t0].[Text], 
	[t0].[CreatedDate], 
	[t0].[RetweetCount], 
	[t0].[AuthorId], 
	[t1].[UserName] AS [AuthorUserName], 
	[t1].[UserScreenName] AS[AuthorUserScreenName],
    [t1].[ProfileImageUrl] AS [AuthorProfileImageUrl]
FROM[Posts] AS[t0]
INNER JOIN[Users] AS[t1] ON[t1].[Id] = [t0].[AuthorId]
ORDER BY[t0].[CreatedDate] DESC";

                    Dictionary<int, TwitterUserViewModel> authors = new Dictionary<int, TwitterUserViewModel>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TwitterUserViewModel author;
                            int authorId = reader.GetInt32(4);
                            if (!authors.TryGetValue(authorId, out author))
                            {
                                author = new TwitterUserViewModel()
                                {
                                    UserName = reader.GetString(5),
                                    UserScreenName = reader.GetString(6),
                                    ProfileImageUrl = reader.GetString(7)
                                };
                                authors.Add(authorId, author);
                            }
                            vm.Posts.Add(new TwitterPostViewModel
                            {
                                Author = author,
                                CreatedDate = reader.GetDateTime(2),
                                Text = reader.GetString(1)
                            });
                        }
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
