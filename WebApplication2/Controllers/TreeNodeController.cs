using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreeNodeController : ControllerBase
    {
        private readonly ITreeNodeService _treeNodeService;

        public TreeNodeController(ITreeNodeService treeNodeService)
        {
            _treeNodeService = treeNodeService;
        }

        [HttpGet]
        public List<TreeNode> GetAll() => _treeNodeService.GetAll();

        [HttpPut]
        public async Task<ActionResult<TreeNode?>> UpdateName(int id, string name)
        {
            TreeNode? node = await _treeNodeService.Rename(id, name);
            if (node == null)
            {
                return NotFound();
            }

            return node;
        }

        [HttpDelete]
        public async Task Delete(int id) =>
            await _treeNodeService.Delete(id);

        [HttpPost]
        public async Task<ActionResult<TreeNode?>> Create(int? parentId, string name)
        {
            TreeNode? node = await _treeNodeService.Create(parentId, name);
            if (node == null)
            {
                return NotFound();
            }

            return node;
        }
    }
}