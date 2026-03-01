using Microsoft.UI.Xaml;

namespace WireWareClient
{
    public partial class App : Application
    {
        // Expose the window globally
        public static Window m_window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }
    }
}