using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using PetCatalogApp.Domain;
using PetCatalogApp.Infrastructure;
using PetCatalogApp.Domain.Interfaces;

namespace PetCatalogApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Owner> Owners { get; set; } = new();
        public ObservableCollection<Pet> Pets { get; set; } = new();

        // --- Властивість для Статус-бару з авто-очищенням ---
        private string _statusMessage = "Готово до роботи";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                SetProperty(ref _statusMessage, value);

                // Якщо повідомлення не пусте, запускаємо таймер на очищення
                if (!string.IsNullOrEmpty(value))
                {
                    ClearStatusAfterDelay(value);
                }
            }
        }

        // Асинхронний метод для очищення
        private async void ClearStatusAfterDelay(string currentMessage)
        {
            await Task.Delay(3000);

            // Перевірка
            if (_statusMessage == currentMessage)
            {
                StatusMessage = string.Empty;
            }
        }

        // --- Властивості для Вибору/Редагування ---
        private Owner _selectedOwner;
        public Owner SelectedOwner
        {
            get => _selectedOwner;
            set
            {
                SetProperty(ref _selectedOwner, value);
                if (value != null)
                {
                    EditableOwner = new Owner
                    {
                        Id = value.Id,
                        FullName = value.FullName,
                        PhoneNumber = value.PhoneNumber,
                        Address = value.Address
                    };

                    // Авто-підстановка
                    if (EditablePet.Id == 0)
                    {
                        EditablePet = new Pet
                        {
                            Name = EditablePet.Name,
                            Species = EditablePet.Species,
                            Breed = EditablePet.Breed,
                            Age = EditablePet.Age,
                            Weight = EditablePet.Weight,
                            OwnerId = value.Id
                        };
                    }
                }
            }
        }

        private Owner _editableOwner = new Owner();
        public Owner EditableOwner
        {
            get => _editableOwner;
            set => SetProperty(ref _editableOwner, value);
        }

        private Pet _selectedPet;
        public Pet SelectedPet
        {
            get => _selectedPet;
            set
            {
                SetProperty(ref _selectedPet, value);
                if (value != null)
                {
                    EditablePet = new Pet
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Species = value.Species,
                        Breed = value.Breed,
                        Age = value.Age,
                        Weight = value.Weight,
                        OwnerId = value.OwnerId,
                        Owner = value.Owner
                    };
                }
            }
        }

        private Pet _editablePet = new Pet();
        public Pet EditablePet
        {
            get => _editablePet;
            set => SetProperty(ref _editablePet, value);
        }

        // Властивість для тексту статистики
        private string _statisticsText = "Натисніть 'Оновити статистику'...";
        public string StatisticsText
        {
            get => _statisticsText;
            set => SetProperty(ref _statisticsText, value);
        }

        // --- АСИНХРОННЕ ЗАВАНТАЖЕННЯ ДАНИХ ---
        public async Task LoadDataAsync()
        {
            StatusMessage = "Завантаження даних...";
            try
            {
                using (var context = new AppDbContext())
                {
                    await context.Database.EnsureCreatedAsync();

                    if (!await context.Owners.AnyAsync())
                    {
                        // (Код наповнення даними скорочено для читабельності) ( Можна прибрати у разі потреби пустої програми )
                        var owner1 = new Owner { FullName = "Іван Петренко", PhoneNumber = "050-111-22-33", Address = "Київ, вул. Хрещатик 1" };
                        var owner2 = new Owner { FullName = "Олена Коваль", PhoneNumber = "097-555-66-77", Address = "Львів, пл. Ринок 10" };

                        var pet1 = new Pet { Name = "Мурчик", Species = "Кіт", Breed = "Британська", Age = 2, Weight = 5.5, Owner = owner1 };
                        var pet2 = new Pet { Name = "Барсік", Species = "Кіт", Breed = "Дворова", Age = 5, Weight = 4.0, Owner = owner1 };
                        var pet3 = new Pet { Name = "Рекс", Species = "Собака", Breed = "Вівчарка", Age = 4, Weight = 30.0, Owner = owner2 };
                        var pet4 = new Pet { Name = "Луна", Species = "Кіт", Breed = "Мейн-кун", Age = 1, Weight = 6.0, Owner = owner2 };

                        context.Owners.AddRange(owner1, owner2);
                        context.Pets.AddRange(pet1, pet2, pet3, pet4);
                        await context.SaveChangesAsync();
                    }

                    var ownersList = await context.Owners.ToListAsync();
                    Owners.Clear();
                    foreach (var owner in ownersList) Owners.Add(owner);

                    var petsList = await context.Pets.Include(p => p.Owner).ToListAsync();
                    Pets.Clear();
                    foreach (var pet in petsList) Pets.Add(pet);
                }
                StatusMessage = "Дані успішно оновлено";
            }
            catch (Exception ex)
            {
                StatusMessage = "Помилка завантаження!";
                MessageBox.Show($"Критична помилка при завантаженні БД:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- ВАЛІДАЦІЯ ---
        private bool IsValidOwner(Owner owner)
        {
            if (string.IsNullOrWhiteSpace(owner.FullName)) { MessageBox.Show("ПІБ пусте!"); return false; }
            if (!Regex.IsMatch(owner.FullName, @"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ\s\-]+$")) { MessageBox.Show("ПІБ тільки літери!"); return false; }
            if (string.IsNullOrWhiteSpace(owner.PhoneNumber)) { MessageBox.Show("Введіть телефон!"); return false; }
            return true;
        }

        private bool IsValidPet(Pet pet)
        {
            if (string.IsNullOrWhiteSpace(pet.Name)) { MessageBox.Show("Кличка пуста!"); return false; }
            if (pet.Age <= 0 || pet.Age > 50) { MessageBox.Show("Некоректний вік (має бути > 0)!"); return false; }
            if (pet.Weight <= 0) { MessageBox.Show("Вага має бути більше нуля!"); return false; }

            // КРИТИЧНО: Перевірка на вибір власника
            if (pet.OwnerId <= 0) { MessageBox.Show("Оберіть власника!"); return false; }
            return true;
        }

        // --- CRUD ВЛАСНИКИ (ASYNC + TRY-CATCH) ---
        public async Task AddOwnerAsync()
        {
            if (!IsValidOwner(EditableOwner)) return;
            StatusMessage = "Збереження власника...";

            try
            {
                using (var context = new AppDbContext())
                {
                    // Використання патерну Repository
                    IRepository<Owner> ownerRepo = new Repository<Owner>(context);

                    var newOwner = new Owner
                    {
                        FullName = EditableOwner.FullName,
                        PhoneNumber = EditableOwner.PhoneNumber,
                        Address = EditableOwner.Address
                    };

                    await ownerRepo.AddAsync(newOwner);

                    // SaveChanges залишається за контекстом (патерн UnitOfWork)
                    await context.SaveChangesAsync();
                }

                StatusMessage = $"Власник {EditableOwner.FullName} доданий успішно";
                EditableOwner = new Owner();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = "Помилка збереження";
                MessageBox.Show($"Помилка додавання: {ex.Message}", "Помилка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task UpdateOwnerAsync()
        {
            if (SelectedOwner == null) return;
            if (!IsValidOwner(EditableOwner)) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    var ownerToUpdate = await context.Owners.FindAsync(EditableOwner.Id);
                    if (ownerToUpdate != null)
                    {
                        ownerToUpdate.FullName = EditableOwner.FullName;
                        ownerToUpdate.PhoneNumber = EditableOwner.PhoneNumber;
                        ownerToUpdate.Address = EditableOwner.Address;
                        await context.SaveChangesAsync();
                    }
                }
                StatusMessage = "Власника оновлено";
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}");
            }
        }

        public async Task DeleteOwnerAsync()
        {
            if (SelectedOwner == null) return;
            StatusMessage = "Видалення...";

            try
            {
                using (var context = new AppDbContext())
                {
                    var ownerToDelete = await context.Owners.FindAsync(SelectedOwner.Id);
                    if (ownerToDelete != null)
                    {
                        context.Owners.Remove(ownerToDelete);
                        await context.SaveChangesAsync();
                    }
                }
                StatusMessage = "Власника (і його тварин) видалено";
                EditableOwner = new Owner();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка видалення: {ex.Message}");
            }
        }

        // --- CRUD ТВАРИНИ (ASYNC + TRY-CATCH) ---
        public async Task AddPetAsync()
        {
            if (!IsValidPet(EditablePet)) return;
            StatusMessage = "Збереження тварини...";

            try
            {
                using (var context = new AppDbContext())
                {
                    var newPet = new Pet
                    {
                        Name = EditablePet.Name,
                        Species = EditablePet.Species,
                        Breed = EditablePet.Breed,
                        Age = EditablePet.Age,
                        Weight = EditablePet.Weight,
                        OwnerId = EditablePet.OwnerId
                    };
                    context.Pets.Add(newPet);
                    await context.SaveChangesAsync();
                }
                StatusMessage = $"Тварина {EditablePet.Name} додана";
                EditablePet = new Pet();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання: {ex.Message}");
            }
        }

        public async Task UpdatePetAsync()
        {
            if (SelectedPet == null) return;
            if (!IsValidPet(EditablePet)) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    var petToUpdate = await context.Pets.FindAsync(EditablePet.Id);
                    if (petToUpdate != null)
                    {
                        petToUpdate.Name = EditablePet.Name;
                        petToUpdate.Species = EditablePet.Species;
                        petToUpdate.Breed = EditablePet.Breed;
                        petToUpdate.Age = EditablePet.Age;
                        petToUpdate.Weight = EditablePet.Weight;
                        petToUpdate.OwnerId = EditablePet.OwnerId;
                        await context.SaveChangesAsync();
                    }
                }
                StatusMessage = "Дані тварини оновлено";
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}");
            }
        }

        public async Task DeletePetAsync()
        {
            if (SelectedPet == null) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    var petToDelete = await context.Pets.FindAsync(SelectedPet.Id);
                    if (petToDelete != null)
                    {
                        context.Pets.Remove(petToDelete);
                        await context.SaveChangesAsync();
                    }
                }
                StatusMessage = "Тварину видалено";
                EditablePet = new Pet();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка видалення: {ex.Message}");
            }
        }

        // --- СТАТИСТИКА (ASYNC) ---
        public async Task CalculateStatisticsAsync()
        {

            StatusMessage = "Обрахунок статистики...";
            try
            {
                using (var context = new AppDbContext())
                {
                    var allPets = await context.Pets.Include(p => p.Owner).ToListAsync();
                    var allOwners = await context.Owners.Include(o => o.Pets).ToListAsync();

                    var report = new System.Text.StringBuilder();
                    report.AppendLine($"=== ЗАГАЛЬНА СТАТИСТИКА ===\n");

                    // 1. Кількість тварин по видам
                    var speciesStats = allPets
                        .GroupBy(p => p.Species.ToLower()) // Группуємо все в малому регістрі
                        .Select(g => new
                        {
                            // Робимо першу літеру великою для краси (TextInfo потребує System.Globalization)
                            Species = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
                            Count = g.Count()
                        });

                    foreach (var item in speciesStats) report.AppendLine($"{item.Species}: {item.Count}");

                    // 2. Найпоширеніша порода
                    var popularBreed = allPets.GroupBy(p => p.Breed)
                                              .OrderByDescending(g => g.Count())
                                              .Select(g => g.Key)
                                              .FirstOrDefault();
                    report.AppendLine($"\nНайпоширеніша порода: {(popularBreed ?? "Немає даних")}");

                    // 3. Топ власників
                    report.AppendLine("\n--- Топ власників ---");
                    var ownersStats = allOwners.OrderByDescending(o => o.Pets.Count).Take(5);
                    foreach (var owner in ownersStats) report.AppendLine($"{owner.FullName}: {owner.Pets.Count} тварин");

                    StatisticsText = report.ToString();
                }
                StatusMessage = "Статистику сформовано";
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Помилка статистики: {ex.Message}");
            }
        }

        public async Task ExportYoungCatsAsync()
        {
            StatusMessage = "Експорт даних...";
            try
            {
                using (var context = new AppDbContext())
                {
                    // 1. Спочатку завантажимо ВСІХ тварин, щоб перевірити, що в базі взагалі щось є
                    var allPets = await context.Pets.Include(p => p.Owner).ToListAsync();

                    if (!allPets.Any())
                    {
                        MessageBox.Show("База даних порожня! Додайте хоча б одну тварину.");
                        StatusMessage = "База пуста";
                        return;
                    }

                    // 2. Фільтруємо у пам'яті (це надійніше для діагностики)
                    // Шукаємо: вік < 3  І  (вид містить "кіт" АБО "cat")
                    var youngCats = allPets
                        .Where(p => p.Age < 3 &&
                                   (p.Species.ToLower().Trim().Contains("кіт") ||
                                    p.Species.ToLower().Trim().Contains("cat")))
                        .ToList();

                    var sb = new System.Text.StringBuilder();
                    string filename = "YoungCatsReport.txt";

                    if (youngCats.Any())
                    {
                        // ВАРІАНТ А: Знайшли котів
                        sb.AppendLine("ЗВІТ: МОЛОДІ КОТИ (до 3 років)");
                        sb.AppendLine($"Знайдено записів: {youngCats.Count}");
                        sb.AppendLine($"Дата: {DateTime.Now}\n");
                        sb.AppendLine(new string('-', 50));

                        foreach (var cat in youngCats)
                        {
                            sb.AppendLine($"[+] Кличка: {cat.Name}");
                            sb.AppendLine($"    Вид: '{cat.Species}' | Вік: {cat.Age}");
                            sb.AppendLine($"    Власник: {cat.Owner.FullName}");
                            sb.AppendLine(new string('-', 20));
                        }
                        StatusMessage = $"Звіт збережено! ({youngCats.Count} тварин)";
                    }
                    else
                    {
                        // ВАРІАНТ Б: Нічого не знайшли -> робимо ДІАГНОСТИКУ
                        sb.AppendLine("УВАГА: Фільтр не знайшов котів, але ось список ВСІХ тварин у базі для перевірки:");
                        sb.AppendLine($"Всього тварин: {allPets.Count}\n");

                        foreach (var p in allPets)
                        {
                            // Перевіряємо умови і пишемо, що не так
                            string ageCheck = p.Age < 3 ? "OK" : "НЕ ПІДХОДИТЬ (> 2)";
                            string speciesLower = p.Species?.ToLower().Trim() ?? "пусто";
                            bool isCat = speciesLower.Contains("кіт") || speciesLower.Contains("cat");
                            string speciesCheck = isCat ? "OK" : $"НЕ ПІДХОДИТЬ (Програма бачить: '{p.Species}')";

                            sb.AppendLine($"Тварина ID: {p.Id} | Ім'я: {p.Name}");
                            sb.AppendLine($" -> Вік: {p.Age} ({ageCheck})");
                            sb.AppendLine($" -> Вид: '{p.Species}' ({speciesCheck})");
                            sb.AppendLine("");
                        }
                        StatusMessage = "Збережено діагностичний звіт";
                        MessageBox.Show("Котів за критеріями не знайдено, але я зберіг файл зі списком усіх тварин, щоб ви побачили причину.", "Діагностика");
                    }

                    // Зберігаємо файл
                    await System.IO.File.WriteAllTextAsync(filename, sb.ToString());

                    // Відкриваємо файл автоматично (щоб ти одразу побачив результат)
                    var processStartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filename,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(processStartInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка експорту: {ex.Message}");
                StatusMessage = "Помилка";
            }
        }
    }
}