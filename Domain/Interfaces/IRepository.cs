using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCatalogApp.Domain.Interfaces
{
    // Виконує вимогу: Generic Interface (Узагальнений інтерфейс)
    // T : BaseEntity означає, що цей репозиторій працює тільки з нашими класами (Owner, Pet, Visit)
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}