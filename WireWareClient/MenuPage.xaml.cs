using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WireWareClient
{
    public sealed partial class MenuPage : Page
    {
        private string _userAlias = "PLAYER";

        public MenuPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string nickname)
            {
                _userAlias = nickname;
                UserAliasText.Text = nickname.ToUpper();
                ContentFrame.Navigate(typeof(DashboardPage), _userAlias);
            }
            base.OnNavigatedTo(e);
        }

        private void MainNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer == null) return;

            var tag = args.SelectedItemContainer.Tag?.ToString();
            if (string.IsNullOrEmpty(tag)) return;

            Type target = tag.ToLower() switch
            {
                "dash" => typeof(DashboardPage),
                "news" => typeof(NewsPage),
                "mods" => typeof(ModsPage),
                "account" => typeof(AccountPage),
                "settings" => typeof(SettingsPage),
                _ => null
            };

            if (target != null && ContentFrame.CurrentSourcePageType != target)
                ContentFrame.Navigate(target, _userAlias);
        }
    }

    // ============================================================
    // DASHBOARD PAGE
    // ============================================================

    public sealed partial class DashboardPage : Page
    {
        private readonly string[] VanillaVersions =
        {
            "1.7.10","1.12.2","1.16.5","1.18.2",
            "1.19.2","1.19.4","1.20.1","1.20.4",
            "1.21","1.21.1","1.21.3","1.21.4","1.21.5"
        };

        private string _userAlias;
        private readonly ObservableCollection<string> _instances = new();
        private readonly MinecraftPath _mcPath;
        private readonly MinecraftLauncher _launcher;

        private string _selectedInstallId;
        private string _selectedDisplayName = "SELECT VERSION";
        private string _selectedLoader = "vanilla";

        public DashboardPage()
        {
            InitializeComponent();

            InstanceGallery.ItemsSource = _instances;

            _mcPath = new MinecraftPath();
            _launcher = new MinecraftLauncher(_mcPath);

            IpText.Text = "LOADING...";
            LocationText.Text = "LOADING...";
            IspText.Text = "LOADING...";

            _ = LoadRadminInfoAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string alias)
                _userAlias = alias;

            LoadVersionsToFlyout();
            _ = LoadInstancesAsync();

            base.OnNavigatedTo(e);
        }

        // ============================================================
        // VERSION PICKER
        // ============================================================

        private void LoadVersionsToFlyout()
        {
            VersionListFlyout.Items.Clear();

            VersionListFlyout.Items.Add(new MenuFlyoutItem
            {
                Text = "Vanilla",
                IsEnabled = false,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            foreach (var v in VanillaVersions)
            {
                var item = new MenuFlyoutItem { Text = $"{v} (Vanilla)" };
                item.Click += (_, _) => SelectVersion(v, $"{v} (Vanilla)", "vanilla");
                VersionListFlyout.Items.Add(item);
            }
        }

        private void SelectVersion(string id, string display, string loader)
        {
            _selectedInstallId = id;
            _selectedDisplayName = display;
            _selectedLoader = loader;
            SelectedVersionText.Text = display;
        }

        // ============================================================
        // ENFORGE (FIXED)
        // ============================================================

        private async void EnForge_Click(object sender, RoutedEventArgs e)
        {
            var instanceName = (sender as Button)?.Tag as string;
            if (string.IsNullOrEmpty(instanceName)) return;

            try
            {
                StatusLabel.Text = "Please install Forge manually first!\n\n" +
                                  "1. Download Forge installer from https://files.minecraftforge.net\n" +
                                  "2. Run it and choose \"Install client\"\n" +
                                  "3. Select your minecraft folder (the same one launcher uses)\n" +
                                  "4. After installation click Refresh versions";

                // Optional: try to open browser
                try
                {
                    var psi = new ProcessStartInfo("https://files.minecraftforge.net")
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch { }

                // You can also add big nice button that opens link
                // But **never** try to run java -jar forge-installer.jar yourself!
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Error: " + ex.Message;
            }
        }

        // ============================================================
        // INSTALL
        // ============================================================

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedInstallId))
            {
                StatusLabel.Text = "No version selected.";
                return;
            }

            try
            {
                StatusLabel.Text = $"Installing {_selectedDisplayName}...";
                DownloadProgress.Visibility = Visibility.Visible;

                await _launcher.InstallAsync(_selectedInstallId);
                await LoadInstancesAsync();

                StatusLabel.Text = "Install complete.";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Install failed: {ex.Message}";
            }
            finally
            {
                DownloadProgress.Visibility = Visibility.Collapsed;
            }
        }

        // ============================================================
        // PLAY
        // ============================================================

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            var instanceName = (sender as Button)?.Tag as string;
            if (string.IsNullOrEmpty(instanceName))
            {
                StatusLabel.Text = "No instance selected.";
                return;
            }

            try
            {
                var session = MSession.CreateOfflineSession(_userAlias);

                var options = new MLaunchOption
                {
                    Session = session,
                    MaximumRamMb = 4096
                };

                var proc = await _launcher.BuildProcessAsync(instanceName, options);
                proc.Start();

                StatusLabel.Text = "KERNEL ONLINE";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Launch failed: {ex.Message}";
            }
        }

        // ============================================================
        // INSTANCES
        // ============================================================

        private async Task LoadInstancesAsync()
        {
            _instances.Clear();

            foreach (var dir in Directory.GetDirectories(_mcPath.Versions))
            {
                var name = Path.GetFileName(dir);
                if (File.Exists(Path.Combine(dir, name + ".json")))
                    _instances.Add(name);
            }

            StatusLabel.Text = _instances.Any() ? "IDLE" : "No versions installed";
        }

        // ============================================================
        // IP INFO
        // ============================================================

        private async Task LoadRadminInfoAsync()
        {
            try
            {
                using var client = new HttpClient();
                var json = await client.GetStringAsync("https://ipapi.co/json/");
                var data = JsonDocument.Parse(json).RootElement;

                DispatcherQueue.TryEnqueue(() =>
                {
                    IpText.Text = data.GetProperty("ip").GetString();
                    LocationText.Text =
                        $"{data.GetProperty("city").GetString()}, " +
                        $"{data.GetProperty("region").GetString()}, " +
                        $"{data.GetProperty("country_name").GetString()}";
                    IspText.Text = data.GetProperty("org").GetString();
                });
            }
            catch
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    IpText.Text = "127.0.0.1";
                    LocationText.Text = "LOCAL";
                    IspText.Text = "UNKNOWN";
                });
            }
        }
    }
}
