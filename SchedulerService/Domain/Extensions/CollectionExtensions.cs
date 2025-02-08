namespace SchedulerService.Domain.Extensions;

public static class CollectionExtensions
{
    public static bool RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        var itemToRemove = default(T);
        
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                itemToRemove = item;
                break;
            }
        }
        
        return itemToRemove is not null && collection.Remove(itemToRemove);
    }
}