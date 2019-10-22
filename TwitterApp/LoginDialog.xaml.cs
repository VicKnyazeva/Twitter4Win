using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetSharp;
using TwitterApp.Helpers;

namespace TwitterApp
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        private OAuthRequestToken requestToken;


        public LoginDialog()
        {
            InitializeComponent();

        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString() != "https://api.twitter.com/oauth/authorize")
                return;

            string verifier = GetVerifierFromPage();

            TwitterHelper.Authenticate(this.requestToken, verifier);

            var mw = new MainWindow();
            mw.ShowActivated = true;
            mw.Show();

            this.Close();
        }

        private string GetVerifierFromPage()
        {
            dynamic doc = this.browser.Document;
            var html = doc.documentElement.innerHtml;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var codeNode = htmlDoc.DocumentNode.SelectSingleNode("//code");
            return codeNode.InnerText;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = TwitterHelper.Init(out this.requestToken);
                this.browser.Navigate(uri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Произошло исключение {0}:\n\t{1}", ex.GetType().Name, ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
