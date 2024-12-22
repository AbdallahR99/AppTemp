namespace AppTemp.Core.Persistence;
public interface IDbInitializer
{
    Task MigrateAsync(CancellationToken cancellationToken);
    Task SeedAsync(CancellationToken cancellationToken);
}
