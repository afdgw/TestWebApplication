using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using WebApplication2.Exceptions;

namespace WebApplication2.Services;


internal class TreeNodeService : ITreeNodeService
{
    private readonly ApplicationDbContext _dbContext;

    public TreeNodeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<TreeNode> GetAll() => _dbContext.TreeNodes
        .Where(tn => !tn.ParentId.HasValue)
        .ToList();

    public async Task<TreeNode?> Create(int? rootId, string name)
    {
        TreeNode? rootNode = null;
        if (rootId.HasValue)
        {
            rootNode = await _dbContext.TreeNodes
                .FirstOrDefaultAsync(n => n!.Id == rootId);

            if (rootNode == null)
                return null;
        }

        var newNode = new TreeNode
        {
            ParentId = rootNode?.Id ?? rootId,
            Name = name
        };

        _dbContext.TreeNodes.Add(newNode);

        await _dbContext.SaveChangesAsync();

        return newNode;
    }


    public async Task<TreeNode?> Rename(int nodeId, string name)
    {
        TreeNode? node = await _dbContext.TreeNodes
            .FirstOrDefaultAsync(n => n!.Id ==nodeId);

        if (node == null)
            return null;

        node.Name = name;

        _dbContext.TreeNodes.Update(node);

        await _dbContext.SaveChangesAsync();

        return node;
    }

    public async Task Delete(int nodeId)
    {
        TreeNode? node = await _dbContext.TreeNodes.Include("Children")
            .FirstOrDefaultAsync(n => n!.Id == nodeId);

        if (node == null)
            return;

        if (node.Children.Any())
        {
            throw new SecurityException("You have to delete all children nodes first");
        }

        _dbContext.TreeNodes.Remove(node);

        await _dbContext.SaveChangesAsync();
    }
}
