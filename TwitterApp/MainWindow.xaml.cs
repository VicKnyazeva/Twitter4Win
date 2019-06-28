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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetServiceData();
        }

        //private Task<IEnumerable<TwitterPostViewModel>> LoadTweets(int? count = null)
        //{
        //    return Task.Run<IEnumerable<TwitterPostViewModel>>(() =>
        //    {
        //        Task.Delay(2000).Wait();
        //        IEnumerable<TwitterPostViewModel> result = TwitterHelper.GetTweets();
        //        if (count.HasValue && count.Value > 0)
        //        {
        //            result = result.Take(count.Value);
        //        }
        //        return result;
        //    });
        //}

        private async void GetServiceData()
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

                        foreach (var tw in tweetsCollection)
                        {
                            vm.Posts.Add(new TwitterPostViewModel
                            {
                                Author = tw.User,
                                CreatedDate = tw.CreatedDate,
                                Text = tw.Text
                            });
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
