using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository(DatabaseContextFactory contextFactory) : IPostRepository
{
    private readonly DatabaseContextFactory _contextFactory = contextFactory;

    public async Task CreateAsync(PostEntity post)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Add(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        context.Posts.Remove(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid postId)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            // Since we are using navigation properties on entity.
            // UseLazyLoadingProxies in configureDbContext allows to use navigation properties
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(x => x.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .ToListAsync();
    }


    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity postId)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Update(postId);
        _ = await context.SaveChangesAsync();
    }
}