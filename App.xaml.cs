using System.Windows;
using PetCatalogApp.Data;

namespace PetCatalogApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запуск створення бази ДО того, як відкриється вікно
            DataSeeder.SeedData();
        }
    }
}