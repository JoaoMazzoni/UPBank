using AgencyAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.DTO;

namespace AgencyAPI.Test
{
    public class UnitTest1
    {
            private DbContextOptions<AgencyAPIContext> options;

            private void InitializeDataBase()
            {
                options = new DbContextOptionsBuilder<AgencyAPIContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

                using (var context = new AgencyAPIContext(options))
                {
                    AgencyDTO agency1 = new AgencyDTO { Number = "1007", Address = { Complement = "ABC", Number = 10, ZipCode = "15997037" }, CNPJ = "74955443000172", Employees = { "085.122.908-54" }, Restriction = false };
                  
                    context.SaveChanges();
                }
            }

            [Fact]
        public void Test1()
        {

        }
    }
}