using CQRS.Core.infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Dispatchers;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    // Return always a list of PostEntity here
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
        // Check if handler contains specified query handler type
        if (_handlers.ContainsKey(typeof(TQuery)))
        {
            // what is query handler
            throw new IndexOutOfRangeException("You cant register the same query handler twice");
        }

        // X=BaseQuery - Cast BaseQuery->TQuery our concrete query 
        _handlers.Add(typeof(TQuery), x => handler((TQuery)x));
    }

    public Task<List<PostEntity>> SendAsync(BaseQuery query)
    {
        if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<PostEntity>>> handler))
        {
            // Will return the registered query handler
            return handler(query);
        }
        
        throw new ArgumentOutOfRangeException(nameof(handler), "No query handler was registered");
    }
}