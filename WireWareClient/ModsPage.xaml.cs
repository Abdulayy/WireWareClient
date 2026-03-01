using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace WireWareClient
{
    public sealed partial class ModsPage : Page
    {
        public class ModEntry
        {
            public string Name { get; set; }
            public string Author { get; set; }
        }

        public ModsPage()
        {
            this.InitializeComponent();
            LoadPopularMods();
        }

        private void LoadPopularMods()
        {
            var mods = new List<ModEntry>()
            {
                new ModEntry { Name = "SODIUM", Author = "jellysquid3" },
                new ModEntry { Name = "CREATE", Author = "simibubi" },
                new ModEntry { Name = "IRIS SHADERS", Author = "coderbot" },
                new ModEntry { Name = "JEI", Author = "mezz" }
            };
            ModsGallery.ItemsSource = mods;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Future: Implement Modrinth or CurseForge API search here [1, 2]
            string query = ModSearchInput.Text.ToUpper();
            if (!string.IsNullOrEmpty(query))
            {
                // Simple placeholder search filtering
                var results = new List<ModEntry>() { new ModEntry { Name = query, Author = "SEARCH_RESULT" } };
                ModsGallery.ItemsSource = results;
            }
        }
    }
}