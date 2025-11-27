using PetCatalogApp.Domain;
using PetCatalogApp.Infrastructure;
using Microsoft.EntityFrameworkCore; // AppDbContext є частиною EF Core

namespace PetCatalogApp.Data
{
    public static class DataSeeder
    {
        public static void SeedData()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();

                if (context.Owners.Any())
                {
                    return;
                }
                // Заповнення БД, ( Можна прибрати у разі потреби пустої програми )
                var owner1 = new Owner { FullName = "Іван Петренко", PhoneNumber = "050-111-22-33", Address = "Київ, вул. Хрещатик 1" };
                var owner2 = new Owner { FullName = "Олена Коваль", PhoneNumber = "097-555-66-77", Address = "Львів, пл. Ринок 10" };

                var pet1 = new Pet { Name = "Мурчик", Species = "Кіт", Breed = "Британська", Age = 2, Weight = 5.5, Owner = owner1 };
                var pet2 = new Pet { Name = "Барсік", Species = "Кіт", Breed = "Дворова", Age = 5, Weight = 4.0, Owner = owner1 };
                var pet3 = new Pet { Name = "Рекс", Species = "Собака", Breed = "Вівчарка", Age = 4, Weight = 30.0, Owner = owner2 };
                var pet4 = new Pet { Name = "Луна", Species = "Кіт", Breed = "Мейн-кун", Age = 1, Weight = 6.0, Owner = owner2 };

                context.Owners.AddRange(owner1, owner2);
                context.Pets.AddRange(pet1, pet2, pet3, pet4);

                context.SaveChanges();
            }
        }
    }
}