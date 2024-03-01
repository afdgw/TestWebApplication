using Repository.Models;

namespace WebApplication2.Services;

public interface ITreeNodeService
{
    List<TreeNode> GetAll();
    Task<TreeNode?> Create(int? rootId, string name);
    Task<TreeNode?> Rename(int nodeId, string name);
    Task Delete(int nodeId);
}