using AppTemp.Core.Domain.Contracts;
using Ardalis.Specification;

namespace AppTemp.Core.Persistence;
public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
}

public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IAggregateRoot
{
}
