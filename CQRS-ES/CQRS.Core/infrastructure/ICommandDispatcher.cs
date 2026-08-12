using CQRS.Core.Commands;

namespace CQRS.Core.infrastructure;

public interface ICommandDispatcher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}