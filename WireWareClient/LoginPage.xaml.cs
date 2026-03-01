using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace WireWareClient
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                string alias = UsernameInput.Text;
                LoginButton.Content = "LINKING TO KERNEL...";
                LoginButton.IsEnabled = false;

                await System.Threading.Tasks.Task.Delay(1200);
                this.Frame.Navigate(typeof(MenuPage), alias, new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
        }
    }
}