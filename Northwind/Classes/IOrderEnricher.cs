using System.Collections.Generic;

namespace Northwind.Classes
{
    public interface IOrderEnricher<T>
    {
        List<T> Enrich(IEnumerable<T> entity);
        T Enrich(T entity);
    }
}