namespace GastroHomeCA.Application.Common.Interfaces;

using GastroHomeCA.Domain.Entities;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
