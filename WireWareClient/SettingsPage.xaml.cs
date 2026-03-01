using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using System;

namespace WireWareClient
{
    public sealed partial class SettingsPage : Page
    {
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        // Defaults
        private const double DefaultRamMB = 4096;
        private const string DefaultJavaPath = "";

        public SettingsPage()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // RAM loading
            if (_localSettings.Values.TryGetValue("RamAlloc", out var ramObj) &&
                ramObj is double savedRam &&
                savedRam >= RamSlider.Minimum && savedRam <= RamSlider.Maximum)
            {
                RamSlider.Value = savedRam;
            }
            else
            {
                RamSlider.Value = DefaultRamMB;
                // Optional: save default if not present
                _localSettings.Values["RamAlloc"] = DefaultRamMB;
            }

            // Java path loading
            if (_localSettings.Values.TryGetValue("JavaPath", out var javaObj) &&
                javaObj is string savedPath)
            {
                JavaPathInput.Text = savedPath;
            }
            else
            {
                JavaPathInput.Text = DefaultJavaPath;
                // Optional: save default
                _localSettings.Values["JavaPath"] = DefaultJavaPath;
            }

            UpdateRamLabel();
        }

        private void RamSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (sender is Slider slider)
            {
                double value = slider.Value;
                _localSettings.Values["RamAlloc"] = value;
                UpdateRamLabel();
            }
        }

        private void UpdateRamLabel()
        {
            if (RamValueText != null)
            {
                double gb = RamSlider.Value / 1024.0;
                RamValueText.Text = gb >= 1.0 ? $"{gb:F1} GB" : $"{(int)RamSlider.Value} MB";
            }
        }

        private void JavaPathInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string trimmed = textBox.Text?.Trim() ?? "";
                _localSettings.Values["JavaPath"] = trimmed;
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            // Reset UI
            RamSlider.Value = DefaultRamMB;
            JavaPathInput.Text = DefaultJavaPath;

            // Reset storage
            _localSettings.Values.Remove("RamAlloc");
            _localSettings.Values.Remove("JavaPath");

            // Refresh UI
            UpdateRamLabel();

            // Optional feedback (add a TextBlock x:Name="FeedbackText" in XAML if you want)
            // FeedbackText.Text = "Settings restored to defaults";
        }
    }
}