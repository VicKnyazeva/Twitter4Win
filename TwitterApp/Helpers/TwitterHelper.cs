using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterApp.ViewModels;

namespace TwitterApp.Helpers
{
    static class TwitterHelper
    {
        const string consumerKey = "ZY4BFJOOziWVfhK7d8rvBx0Fu";
        const string consumerSecret = "mUKFkJcmf9hRgYxusbvBJ66nKelYd8oqZc79OVXT3BXg774Ryb";

        static TwitterService service;

        public static Uri Init(out OAuthRequestToken requestToken)
        {
            string errorMessage = "Не удалось выполнить запрос.";
            try
            {
                service = new TwitterService(consumerKey, consumerSecret);

                requestToken = service.GetRequestToken();
                if (requestToken == null)
                    throw new TwitterException(errorMessage);

                errorMessage = "Не удалось авторизовать приложение.";
                return service.GetAuthorizationUri(requestToken);
            }
            catch (Exception ex)
            {
                throw new TwitterException(errorMessage, ex);
            }
        }

        public static void Authenticate(OAuthRequestToken requestToken, string verifier)
        {
            var access = service.GetAccessToken(requestToken, verifier);
            service.AuthenticateWith(access.Token, access.TokenSecret);
        }

        public static TwitterUserViewModel GetUserInfo()
        {
            TwitterUserViewModel userViewModel = null;

            var user = service.GetUserProfile(new GetUserProfileOptions() { IncludeEntities = false, SkipStatus = true });
            if (user != null)
            {
                userViewModel = new TwitterUserViewModel()
                {
                    UserName = user.Name,
                    UserScreenName = user.ScreenName,
                    FavouritesCount = user.FriendsCount,
                    FollowersCount = user.FollowersCount,
                    ProfileImageUrl = GetUserImage(user)

                };
            }
            return userViewModel;
        }

        /// <summary>
        /// Асинхронное получение информации о юзере
        /// </summary>
        /// <returns></returns>
        public static Task<TwitterUserViewModel> GetUserInfoAsync()
        {
            return Task.Run(GetUserInfo);
        }

        public static List<TwitterPostViewModel> GetTweets()
        {
            try
            {
                var source = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions()).ToList();
                var result = new List<TwitterPostViewModel>();
                if (source != null)
                {
                    foreach (var src in source)
                    {
                        result.Add(Convert(src));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new TwitterException("Не удалось получить список твитов.", ex);
            }
        }

        /// <summary>
        /// Асинхронное получение твитов
        /// </summary>
        /// <returns></returns>
        public static Task<List<TwitterPostViewModel>> GetTweetsAsync()
        {
            return Task.Run(GetTweets);
        }

        private static TwitterPostViewModel Convert(TwitterStatus src)
        {
            TwitterPostViewModel result = new TwitterPostViewModel(src.Text)
            {
                Text = src.Text,
                CreatedDate = src.CreatedDate,
                RetweetCount = src.RetweetCount,
                User = new TwitterUserViewModel
                {
                    UserName = src.User.Name,
                    UserScreenName = src.User.ScreenName,
                    ProfileImageUrl = GetUserImage(src.User),
                    FollowersCount = src.User.FollowersCount,
                    FavouritesCount = src.User.FavouritesCount,
                    FriendsCount = src.User.FriendsCount,
                    CreatedDate = src.User.CreatedDate,
                    Description = src.User.Description
                }
            };
            return result;
        }

        public static List<TwitterTrendViewModel> GetTrends()
        {
            try
            {
                var source = service.ListLocalTrendsFor(new ListLocalTrendsForOptions() { Id = 1 }); // 1 - весь мир
                var result = new List<TwitterTrendViewModel>();

                if (source != null)
                {
                    foreach (var src in source)
                    {
                        result.Add(Convert(src));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new TwitterException("Не удалось получить список трендов.", ex);
            }

        }

        /// <summary>
        /// Асинхронное получение трендов
        /// </summary>
        /// <returns></returns>
        public static Task<List<TwitterTrendViewModel>> GetTrendsAsync()
        {
            return Task.Run(GetTrends);
        }

        private static TwitterTrendViewModel Convert(TwitterTrend src)
        {
            TwitterTrendViewModel result = new TwitterTrendViewModel
            {
                Name = src.Name
            };
            return result;
        }

        public static List<string> GetSortedTrendsList()
        {
            try
            {
                var source = service.ListLocalTrendsFor(new ListLocalTrendsForOptions() { Id = 1 }); // 1 - весь мир
                List<string> result = new List<string>();

                if (source != null)
                {
                    foreach (var src in source)
                    {
                        result.Add(src.Name);
                    }
                    result.Sort();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new TwitterException("Не удалось получить список трендов.", ex);
            }
        }

        public static string GetHashedTrends(IEnumerable<TwitterTrendViewModel> trends)
        {
            var sb = new StringBuilder();

            foreach (var trend in trends)
            {
                if (trend.Name.StartsWith("#"))
                {
                    if (sb.Length != 0)
                        sb.Append(", ");

                    sb.Append(trend.Name);
                }
            }
            return sb.ToString();
        }

        private static string GetDownloadFilePath(string fileName)
        {
            string result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images", fileName);
            return Path.GetFullPath(result);
        }

        private static string GetUserImage(TwitterUser user)
        {
            string url = user.ProfileImageUrl;
            string path = GetDownloadFilePath(user.Name + Path.GetExtension(url));

            if (!File.Exists(path))
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(url, path);
                }
            }
            return path;
        }
    }
}
