using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TwitterApp.ViewModels;

namespace TwitterApp
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MainViewModel;
            vm.GoHome();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MainViewModel;
            vm.DoSearch();
        }

        private void _input_GotFocus(object sender, RoutedEventArgs e)
        {
            this._searchButton.IsDefault = true;
            this.updatePlaceholderVisibility();
        }

        private void _input_LostFocus(object sender, RoutedEventArgs e)
        {
            this._searchButton.IsDefault = false;
            this.updatePlaceholderVisibility();
        }

        private void _input_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.updatePlaceholderVisibility();
        }

        private void updatePlaceholderVisibility()
        {
            bool textEmpty = string.IsNullOrEmpty(this._input.Text);
            bool focused = Keyboard.FocusedElement == this._input;

            if (textEmpty && !focused)
                this._placehoder.Visibility = Visibility.Visible;
            else
                this._placehoder.Visibility = Visibility.Collapsed;
        }
    }
}
