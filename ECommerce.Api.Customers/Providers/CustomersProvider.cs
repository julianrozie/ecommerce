using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            this.SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Db.Customer() { Id = 1, Name = "Emily Johnson", Address = "1234 Maple Street, Springfield, IL 62701" });
                dbContext.Customers.Add(new Db.Customer() { Id = 2, Name = "Benjamin Carter", Address = "5678 Elm Avenue, Denver, CO 80203" });
                dbContext.Customers.Add(new Db.Customer() { Id = 3, Name = "Sophia Anderson", Address = "9012 Oak Lane, New York, NY 10001" });
                dbContext.Customers.Add(new Db.Customer() { Id = 4, Name = "Liam Williams", Address = "3456 Pine Road, Los Angeles, CA 90012" });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers,
            string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                var customers = await dbContext.Customers.ToListAsync();

                if (customers != null && customers.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(
                        customers);
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

        public async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(p => p.Id == id);

                if (customer != null)
                {
                    var result = mapper.Map<Db.Customer, Models.Customer>(
                        customer);
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
