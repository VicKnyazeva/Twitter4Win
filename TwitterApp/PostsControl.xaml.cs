using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TwitterApp.ViewModels;

namespace TwitterApp
{
    /// <summary>
    /// Interaction logic for PostsControl.xaml
    /// </summary>
    public partial class PostsControl : UserControl
    {
        public PostsControl()
        {
            InitializeComponent();
        }

        private static void updatePostText(TextBlock tb, string text)
        {
            if (string.IsNullOrEmpty(text))
                tb.Text = null;
            else
            {
                //Style linkStyle = tb.TryFindResource("postLink") as Style;

                foreach (string part in Regex.Split(text, @"([#@]\w+)", RegexOptions.Multiline))
                {
                    if (part.Length > 1 && (part.StartsWith("#") || part.StartsWith("@")))
                    {
                        var t = new Hyperlink();
                        {
                            t.Inlines.Add(part);
                            t.Command = ApplicationCommands.Find;
                            t.CommandParameter = part;
                        }
                        tb.Inlines.Add(t);
                    }
                    else
                    {
                        tb.Inlines.Add(part);
                    }
                }
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            TwitterPostViewModel post = tb.DataContext as TwitterPostViewModel;

            updatePostText(tb, post.Text);
        }
    }
}
