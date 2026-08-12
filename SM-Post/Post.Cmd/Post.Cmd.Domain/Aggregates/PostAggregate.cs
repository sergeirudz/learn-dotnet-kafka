using CQRS.Core.Commands;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    // if aggregate is active
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    // Aggregate should have a constructor that takes not arguments or parameters
    public PostAggregate()
    {
    }

    // Should also have a facilitator for handling the command that creates the new instance of the aggregate
    public PostAggregate(Guid id, string author, string message)
    {
        // Always raise the event that creates the Aggregate instance
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });
    }

    // Apply are used to alter the state of the aggregate 
    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }


    public void EditMessage(string message)
    {
        // Validation: Dont allow anyone to edit inactive post
        if (!_active)
        {
            throw new InvalidOperationException("Cannot edit message when not active");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException(
                $"The value of {nameof(message)} can't be  null or empty. Please provide a valid {nameof(message)}.");
        }

        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot like inactive post");
        }

        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
        
        
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }
    
    public void AddComment(string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot edit comment when not active");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException(
                $"The value of {nameof(comment)} can't be null or empty. Please provide a valid {nameof(comment)}.");
        }
        
        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
            
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot edit a comment of an inactive post");
        }

        // If the person who edits the comment is not the person who created the comment.
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("Cannot edit a comment created by another user");
        }
        
        RaiseEvent( new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now
        });
    }
    
    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot remove comment of an inactive post");
        }
        
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("Cannot remove a comment of another user");
        }
        
        RaiseEvent(new CommentRemovedEvent
        {
            Id = _id,
            CommentId = commentId,
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post has been already deleted");
        }

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post created by someone else");
        }
        
        RaiseEvent(new PostRemovedEvent
        {
            Id = _id,
        });
    }
    
    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}