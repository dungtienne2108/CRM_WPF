using CRM.Domain.Interfaces;

namespace CRM.Infrastructure.Database
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void ClearChangeTracker()
        {
            _context.ChangeTracker.Clear();
        }

        public async Task ReloadEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            await _context.Entry(entity).ReloadAsync(cancellationToken);
        }
    }
}
