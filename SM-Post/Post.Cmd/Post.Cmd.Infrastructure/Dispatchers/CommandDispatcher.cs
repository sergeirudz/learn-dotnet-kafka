using CQRS.Core.Commands;
using CQRS.Core.infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher: ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new(); // = new will be the instance of the specified type
    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            // If we already have handler in our handler dictionary
            throw new IndexOutOfRangeException("You cannot register the same command handler twice");
        }
        // If we dont have a handler already in handler dictionary
        _handlers.Add(typeof(T), x => handler((T)x)); // Cast x is base command T is the concrete command type. T is what we are passing to func delegate
    }

    // This will disptach the command to the Registered Command method 
    public async Task SendAsync(BaseCommand command)
    {
        // If we get command type method of this specific command type
        // If I call 'NewPostCommand' and try to dispatch it. command.GetType gets its value.
        // if found then 'NewPostCommand' is assigned to handler
        if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler))
        {
            // If we get the value we can invoke it
            await handler(command);
        }
        else
        {
            throw new ArgumentNullException(nameof(handler), "No command handler was registered!");
        }
        
    }
}