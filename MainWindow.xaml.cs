using System.Windows;
using PetCatalogApp.ViewModels;

namespace PetCatalogApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Подія "Вікно завантажено", щоб завантажити дані
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }

        // --- ВЛАСНИКИ ---
        private async void AddOwner_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.AddOwnerAsync();
        }

        private async void UpdateOwner_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.UpdateOwnerAsync();
        }

        private async void DeleteOwner_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var result = MessageBox.Show("Видалити власника та його тварин?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await vm.DeleteOwnerAsync();
                }
            }
        }

        // --- ТВАРИНИ ---
        private async void AddPet_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.AddPetAsync();
        }

        private async void UpdatePet_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.UpdatePetAsync();
        }

        private async void DeletePet_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var result = MessageBox.Show("Видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await vm.DeletePetAsync();
                }
            }
        }

        // --- ЗВІТИ ---
        private async void CalcStats_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.CalculateStatisticsAsync();
        }

        private async void ExportCats_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) await vm.ExportYoungCatsAsync();
        }
    }
}