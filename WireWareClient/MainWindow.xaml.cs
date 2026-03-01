using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using WinRT.Interop;
using Windows.UI;
using Windows.Graphics;

namespace WireWareClient
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow? _mAppWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _mAppWindow = AppWindow.GetFromWindowId(windowId);

            SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };

            if (AppWindowTitleBar.IsCustomizationSupported() && _mAppWindow != null)
            {
                var titleBar = _mAppWindow.TitleBar;
                ExtendsContentIntoTitleBar = true;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(40, 186, 117, 67);
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(70, 186, 117, 67);
                SetTitleBar(AppTitleBar);
            }

            _mAppWindow?.Resize(new SizeInt32(1300, 900));
            CenterOnScreen();

            RootFrame.Navigate(typeof(InitialisationPage));
        }

        private void CenterOnScreen()
        {
            if (_mAppWindow == null) return;
            DisplayArea displayArea = DisplayArea.GetFromWindowId(_mAppWindow.Id, DisplayAreaFallback.Nearest);
            if (displayArea != null)
            {
                var centeredPos = _mAppWindow.Position;
                centeredPos.X = (displayArea.WorkArea.Width - _mAppWindow.Size.Width) / 2;
                centeredPos.Y = (displayArea.WorkArea.Height - _mAppWindow.Size.Height) / 2;
                _mAppWindow.Move(centeredPos);
            }
        }
    }
}