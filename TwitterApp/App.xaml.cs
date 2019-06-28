using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TwitterApp.ViewModels;
using TwitterApp.Helpers;
using System.Globalization;
using System.Threading;

namespace TwitterApp
{
    public class EntryPoint
    {
        // All WPF applications should execute on a single-threaded apartment (STA) thread
        [STAThread]
        public static void Main()
        {
            try
            {
                bool initResult = TwitterHelper.Init();
                if (initResult == true)
                {
                    App app = new App();
                    app.InitializeComponent();
                    app.Run(new MainWindow());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Произошло исключение {0}:\n\t{1}", ex.GetType().Name, ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //this.MainWindow.Width = 960;
            //this.MainWindow.Height = 760;
        }
    }
}
