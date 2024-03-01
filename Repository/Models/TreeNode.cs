namespace Repository.Models;

public class TreeNode
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int? ParentId { get; set; }
    public virtual TreeNode Parent { get; set; }

    public virtual List<TreeNode> Children { get; set; }
}