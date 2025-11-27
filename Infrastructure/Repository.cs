using Microsoft.EntityFrameworkCore;
using PetCatalogApp.Domain;
using PetCatalogApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCatalogApp.Infrastructure
{
    // Виконує вимогу: Generic Class (Узагальнений клас)
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Автоматично визначає потрібну таблицю (Owners або Pets)
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}