using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace WireWareClient
{
    public sealed partial class NewsPage : Page
    {
        public class NewsItem
        {
            public string Title { get; set; }
            public string Date { get; set; }
            public string Content { get; set; }
        }

        public NewsPage()
        {
            this.InitializeComponent();
            PopulateNews();
        }

        private void PopulateNews()
        {
            var news = new List<NewsItem>()
            {
                new NewsItem { Title = "KERNEL v1.0 DEPLOYED", Date = "2026.01.15", Content = "The WireWare Client has successfully initialized. Persistence modules and high-fidelity UI framework are now stable." },
                new NewsItem { Title = "CML-CORE ENGINE SYNC", Date = "2026.01.12", Content = "Integrated industrial-grade distribution engine. Local instance scanning and background downloading are now operational." },
                new NewsItem { Title = "MODULAR NAVIGATION ALPHA", Date = "2026.01.10", Content = "Sidebar navigation and tabbed architecture finalized. Settings and Identity modules are linked to the central kernel." }
            };
            NewsList.ItemsSource = news;
        }
    }
}