using System.Windows;
using System.Windows.Controls;
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
                Style linkStyle = tb.TryFindResource("postLink") as Style;

                tb.Inlines.Add(text);
                //tb.Inlines.Add(" -- ");
                //tb.Inlines.Add(new Run()
                //{
                //    Text = "#hash", Style = linkStyle

                //});
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
