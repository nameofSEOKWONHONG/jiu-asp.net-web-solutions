using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Context
{
    public class JIUDbContext : DbContext
    {
        private readonly DbConnection connection;
        
        /// <summary>
        /// init appsettings connection
        /// </summary>
        /// <param name="options"></param>
        public JIUDbContext(DbContextOptions options) : base(options)
        {
            
        }

        /// <summary>
        /// init manual connection
        /// </summary>
        /// <param name="connection"></param>
        public JIUDbContext(DbConnection connection)
        {
            this.connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            if (this.connection != null)
            {
                optionsBuilder.UseSqlServer(this.connection, options =>
                {
                    options.EnableRetryOnFailure();
                    // options.ExecutionStrategy(dependencies =>
                    // {
                    //     dependencies.
                    // })
                });
            }
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}