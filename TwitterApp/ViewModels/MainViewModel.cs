using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace TwitterApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(bool work)
        {
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
    }
}
