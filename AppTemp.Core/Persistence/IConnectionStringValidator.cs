namespace AppTemp.Core.Persistence;
public interface IConnectionStringValidator
{
    bool TryValidate(string connectionString, string? dbProvider = null);
}
