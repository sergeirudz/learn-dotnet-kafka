namespace Post.Cmd.Api.Commands;

using CQRS.Core.Commands;

public class DeletePostCommand : BaseCommand
{
    public string Username { get; set; }
}