namespace PetCatalogApp.Domain
{
    
       // Звя'зок з тваринами, БД
    public class Pet : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Weight { get; set; }
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; } = null!;
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}