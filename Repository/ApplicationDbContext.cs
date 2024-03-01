using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TreeNode> TreeNodes { get; set; }
        public DbSet<ExceptionRecord> ExceptionRecords { get; set; }
    }
}
