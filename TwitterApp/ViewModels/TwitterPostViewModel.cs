using System;
using System.Text;
using System.Text.RegularExpressions;
using TwitterApp.Helpers;

namespace TwitterApp.ViewModels
{
    public class TwitterPostViewModel
    {
        public const int TextMaxLength = 10;

        public TwitterPostViewModel()
        {

        }

        public TwitterPostViewModel(string text)
        {
            this.Text = StringHelper.Cut(text, TextMaxLength);
        }

        public TwitterUserViewModel Author { get; set; }

        public string AuthorName
        {
            get { return this.Author != null ? this.Author.UserName : "AuthorName"; }
        }

        public string AuthorNick
        {
            get { return this.Author != null ? this.Author.UserScreenName : "@author"; }
        }

        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RetweetCount { get; set; }

        public TwitterUserViewModel User { get; set; }

        private static Regex rgx = new Regex(@"(\#\w+)"); //Выберет тэги вида: #hashtag, #hash_tag, #123, #hash123

        public string GetHashTags()
        {
            var hashTags = rgx.Matches(this.Text);
            if (hashTags != null && hashTags.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var tag in hashTags)
                {
                    if (sb.Length != 0)
                        sb.Append(", ");

                    sb.Append(tag);

                }
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
