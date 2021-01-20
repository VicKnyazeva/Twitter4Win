using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TwitterApp.EFClasses;
using TwitterApp.Helpers;

namespace TwitterApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(bool work)
        {
            this._cmdLoadPosts = new LoadPostsCommandImpl(this);

            this._Posts = new ObservableCollection<TwitterPostViewModel>();
            this._PostsSource = new ListCollectionView(this._Posts);
            this._PostsSource.Filter = new Predicate<object>(this.SearchFilter);
            this._Trends = new ObservableCollection<TwitterTrendViewModel>();
        }

        public MainViewModel()
            : this(false)
        {
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-1), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-2), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-3), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-14), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-15), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-26), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-37), Text = "post text #1" });
            this._Posts.Add(new TwitterPostViewModel { CreatedDate = DateTime.UtcNow.AddDays(-48), Text = "post text #1" });

            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });
            this._Trends.Add(new TwitterTrendViewModel { Name = "qaweqwe" });

            this.User = new TwitterUserViewModel()
            {
                UserName = "Pupkin",
                UserScreenName = "@pupkin",
                ProfileImageUrl = "pack://siteoforigin:,,,/Resources/home icon.png",
                FollowersCount = 100,
                FavouritesCount = 12345
            };
        }

        private int postsPageSize = 3;
        private int trendsMaxCount = 10;

        public long? PostMinId = null;

        public TwitterUserViewModel User { get; set; }

        internal void DoSearch()
        {
            this._PostsSource.Refresh();
        }

        internal void GoHome()
        {
            this.SearchString = null;
        }

        private bool SearchFilter(object obj)
        {
            TwitterPostViewModel post = obj as TwitterPostViewModel;
            if (post == null)
                return false;

            if (string.IsNullOrEmpty(this._searchStringLower))
                return true;

            if (string.IsNullOrEmpty(post.Text))
                return false;

            return
                post.AuthorName.ToLower().Contains(this._searchStringLower) ||
                post.AuthorNick.ToLower().Contains(this._searchStringLower) ||
                post.Text.ToLower().Contains(this._searchStringLower);
        }

        public ObservableCollection<TwitterPostViewModel> Posts { get { return this._Posts; } }
        private readonly ObservableCollection<TwitterPostViewModel> _Posts;

        public ListCollectionView PostsSource
        {
            get { return this._PostsSource; }
        }
        private readonly ListCollectionView _PostsSource;

        public ObservableCollection<TwitterTrendViewModel> Trends { get { return this._Trends; } }
        private readonly ObservableCollection<TwitterTrendViewModel> _Trends;

        public string SearchString
        {
            get { return this._SearchString; }
            set
            {
                if (this._SearchString == value) return;

                bool emptyBefore = string.IsNullOrEmpty(this._searchStringLower);

                this._SearchString = value;
                this._searchStringLower = this._SearchString?.Trim().ToLower();
                this.RaisePropertyChanged();
                if (string.IsNullOrEmpty(this._searchStringLower) && !emptyBefore)
                {
                    this._PostsSource.Refresh();
                }
            }
        }
        private string _SearchString;
        private string _searchStringLower;

        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;

            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        class LoadPostsCommandImpl : ICommand
        {
            private readonly MainViewModel owner;

            public LoadPostsCommandImpl(MainViewModel owner)
            {
                this.owner = owner;
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                var handler = this.CanExecuteChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

            public bool CanExecute(object parameter)
            {
                return this.owner.CanLoadPosts;
            }

            public void Execute(object parameter)
            {
                if (this.CanExecute(parameter))
                {
                    TaskScheduler uiTaskSheduler = TaskScheduler.FromCurrentSynchronizationContext();

                    var maxPostId = this.owner.PostMinId;
                    this.owner.LoadPostsAsync(uiTaskSheduler)
                        .ContinueWith((prevTask) =>
                        {
                            this.owner.GetTwitterPosts(maxPostId);
                        }, uiTaskSheduler);
                }
            }
        }

        public ICommand LoadPostsCommand { get { return this._cmdLoadPosts; } }
        private readonly LoadPostsCommandImpl _cmdLoadPosts;

        public bool CanLoadPosts
        {
            get { return this._CanLoadPosts; }
            private set
            {
                if (this._CanLoadPosts == value) return;
                this._CanLoadPosts = value;
                this.RaisePropertyChanged(nameof(CanLoadPosts));
                this._cmdLoadPosts.RaiseCanExecuteChanged();
            }
        }
        private bool _CanLoadPosts = true;

        internal Task LoadPostsAsync(TaskScheduler uiTaskSheduler)
        {
            Exception exception = null;

            this.CanLoadPosts = false;

            long? maxPostId = null;
            if (this.PostMinId.HasValue)
            {
                maxPostId = this.PostMinId.Value - 1;
            }

            var loadTweets = TwitterHelper.GetTweetsAsync(postsPageSize, maxPostId)
            .ContinueWith(loadTask =>
            {
                if (exception != null) return;

                exception = loadTask.Exception;
                if (exception != null) return;

                try
                {
                    var tweetsCollection = loadTask.Result;//.Take(pageSize);

                    using (var db = new TwitterContext())
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var users = db.Users.ToDictionary(u => u.UserName);
                                foreach (var tw in tweetsCollection)
                                {
                                    if (this.PostMinId == null || this.PostMinId.Value > tw.Id)
                                        this.PostMinId = tw.Id;

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
                                            TwitterId = tw.Id,
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
                finally
                {
                    this.CanLoadPosts = true;
                }
            }, uiTaskSheduler);
            
            return loadTweets;
        }

        internal void GetTwitterPosts(long? maxPostId)
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

                    if (this.Trends.Count == 0)
                    {
                        foreach (var trend in trends.Take(trendsMaxCount))
                            this.Trends.Add(trend);
                    }

                    IQueryable<TwitterPost> qPosts = db.Posts.Include(nameof(TwitterPost.Author));
                    if (maxPostId.HasValue)
                    {
                        qPosts = qPosts.Where(p => p.TwitterId < maxPostId.Value);
                    }

                    var posts =
                        from p in qPosts
                        orderby p.CreatedDate descending, p.Id
                        select p;

                    //this.Posts.Clear();

                    foreach (var post in posts.Take(postsPageSize).ToArray())
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
                        this.Posts.Add(new TwitterPostViewModel
                        {
                            Id = post.TwitterId,
                            Text = post.Text,
                            Author = author,
                            CreatedDate = post.CreatedDate
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            this.PostsSource.Refresh();

            if (exception != null)
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
            }
        }

    }
}
