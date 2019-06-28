using System;

namespace TwitterApp.ViewModels
{
    public class TwitterUserViewModel
    {
        public string UserName { get; set; }

        public string UserScreenName
        {
            get { return this.userScreenName; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !value.StartsWith("@"))
                    value = "@" + value;

                this.userScreenName = value;
            }
        }
        private string userScreenName;

        public string Description { get; set; }

        public int FollowersCount { get; set; }

        public int FavouritesCount { get; set; }

        public int FriendsCount { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
