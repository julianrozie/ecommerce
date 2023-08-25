using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Orders.Db
{
    public class OrderItemsDbContext : DbContext
    {
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderItemsDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
