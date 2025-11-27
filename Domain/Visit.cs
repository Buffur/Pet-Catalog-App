namespace PetCatalogApp.Domain
{
    public class Visit : BaseEntity
    {
        public DateTime VisitDate { get; set; } = DateTime.Now;
        public string Reason { get; set; } = string.Empty; // Причина звернення
        public string Diagnosis { get; set; } = string.Empty; // Діагноз/Примітка
        public decimal Cost { get; set; } // Вартість послуг

        // Зв'язок з твариною
        public int PetId { get; set; }
        public virtual Pet Pet { get; set; } = null!;
    }
}