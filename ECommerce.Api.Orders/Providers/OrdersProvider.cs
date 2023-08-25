using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            this.SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 1, CustomerId = 1, OrderDate = DateTime.UtcNow.AddDays(-7),
                    Items = new List<OrderItem>
                    {
                        new Db.OrderItem() { Id = 11, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 20 },
                        new Db.OrderItem() { Id = 12, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5 },
                    },
                    Total = 55,
                });
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = DateTime.UtcNow.AddDays(-6),
                    Items = new List<OrderItem>
                    {
                        new Db.OrderItem() { Id = 21, OrderId = 2, ProductId = 3, Quantity = 4, UnitPrice = 150 },
                        new Db.OrderItem() { Id = 22, OrderId = 2, ProductId = 4, Quantity = 5, UnitPrice = 200 },
                    },
                    Total = 1600,
                });
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 3,
                    CustomerId = 3,
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    Items = new List<OrderItem>
                    {
                        new Db.OrderItem() { Id = 31, OrderId = 3, ProductId = 1, Quantity = 1, UnitPrice = 20 },
                        new Db.OrderItem() { Id = 32, OrderId = 3, ProductId = 3, Quantity = 2, UnitPrice = 5 },
                        new Db.OrderItem() { Id = 33, OrderId = 3, ProductId = 4, Quantity = 3, UnitPrice = 150 },
                    },
                    Total = 480,
                });
                dbContext.Orders.Add(new Db.Order()
                {
                    Id = 4,
                    CustomerId = 2,
                    OrderDate = DateTime.UtcNow.AddDays(-4),
                    Items = new List<OrderItem>
                    {
                        new Db.OrderItem() { Id = 41, OrderId = 4, ProductId = 1, Quantity = 7, UnitPrice = 20 },
                        new Db.OrderItem() { Id = 42, OrderId = 4, ProductId = 2, Quantity = 8, UnitPrice = 5 },
                        new Db.OrderItem() { Id = 43, OrderId = 4, ProductId = 3, Quantity = 9, UnitPrice = 150 },
                        new Db.OrderItem() { Id = 44, OrderId = 4, ProductId = 4, Quantity = 10, UnitPrice = 200 },
                    },
                    Total = 3530,
                });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders
                    .Where(order => order.CustomerId == customerId)
                    .Include(o => o.Items)
                    .ToListAsync();

                if (orders != null)
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(
                        orders);
                    return (true, result, null);
                }

                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
