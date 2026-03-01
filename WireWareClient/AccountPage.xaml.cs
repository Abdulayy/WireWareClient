using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WireWareClient
{
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
            LoadSavedImages();
        }

        private async void PickAvatar_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window); // Standardized App window reference
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Save to local folder for persistence
                var localFolder = ApplicationData.Current.LocalFolder;
                var savedFile = await file.CopyAsync(localFolder, "custom_avatar.jpg", NameCollisionOption.ReplaceExisting);

                LocalAvatarPreview.ImageSource = new BitmapImage(new Uri(savedFile.Path));
                // Future: Update MenuPage header pic via a shared service or event
            }
        }

        private async void PickSkin_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                SkinPathText.Text = $"LINKED: {file.Name}";
                // Logic to copy to.minecraft/cachedImages/skins for OfflineSkins mod
            }
        }

        private void LoadSavedImages()
        {
            string avatarPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "custom_avatar.jpg");
            if (File.Exists(avatarPath))
            {
                LocalAvatarPreview.ImageSource = new BitmapImage(new Uri(avatarPath));
            }
        }
    }
}