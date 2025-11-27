using PetCatalogApp.Domain;

namespace PetCatalogApp.Domain
{
    // Успадкування
    public class Owner : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public virtual ICollection<Pet> Pets { get; set; } = new System.Collections.ObjectModel.ObservableCollection<Pet>();
    }
}