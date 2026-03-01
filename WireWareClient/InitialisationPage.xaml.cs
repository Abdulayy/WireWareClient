using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace WireWareClient
{
    public sealed partial class InitialisationPage : Page
    {
        private DispatcherTimer? _dotTimer;
        private int _dotCount = 0;
        private const string _baseText = "Scanning Identity Modules";

        public InitialisationPage() { this.InitializeComponent(); }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FadeInAura.Begin();
            StartDotAnimation();
            StartHeartbeat();

            // Total 8-second cinematic boot [3]
            await Task.Delay(7600);

            FadeOutAura.Begin();
            await Task.Delay(400);

            _dotTimer?.Stop();
            this.Frame.Navigate(typeof(LoginPage), null, new SuppressNavigationTransitionInfo());
        }

        private void StartDotAnimation()
        {
            _dotTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _dotTimer.Tick += (s, e) => {
                _dotCount = (_dotCount + 1) % 4;
                StatusDots.Text = _baseText + new string('.', _dotCount);
            };
            _dotTimer.Start();
        }

        private void StartHeartbeat()
        {
            Storyboard pulse = new Storyboard();
            DoubleAnimation aniX = new DoubleAnimation { From = 0.85, To = 1.2, Duration = new Duration(TimeSpan.FromSeconds(2.8)), AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, EasingFunction = new CubicEase() };
            DoubleAnimation aniY = new DoubleAnimation { From = 0.85, To = 1.2, Duration = new Duration(TimeSpan.FromSeconds(2.8)), AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, EasingFunction = new CubicEase() };
            Storyboard.SetTarget(aniX, GlowScale); Storyboard.SetTargetProperty(aniX, "ScaleX");
            Storyboard.SetTarget(aniY, GlowScale); Storyboard.SetTargetProperty(aniY, "ScaleY");
            pulse.Children.Add(aniX); pulse.Children.Add(aniY);
            pulse.Begin();
        }
    }
}