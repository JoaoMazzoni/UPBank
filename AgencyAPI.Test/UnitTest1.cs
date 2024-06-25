using AgencyAPI.Controllers;
using AgencyAPI.Data;
using AgencyAPI.Services;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Moq;

namespace AgencyAPI.Test
{
    public class UnitTest1
    {
        private DbContextOptions<AgencyAPIContext> options;
        private DbContextOptions<AddressAPIContext> options2;


        private void InitializeDataBase()
        {
            options = new DbContextOptionsBuilder<AgencyAPIContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            using (var context = new AgencyAPIContext(options))
            {  
            }
        }
        [Fact]
        public void TestInsertOne()
        {
            InitializeDataBase();

            using (var context = new AgencyAPIContext(options))
            {
                // Mock services
                var addressServiceMock = new Mock<AddressService>(context);
                var employeeServiceMock = new Mock<EmployeeService>(context);
                var accountServiceMock = new Mock<AccountService>(context);

                var controller = new AgenciesController(context, addressServiceMock.Object, employeeServiceMock.Object, accountServiceMock.Object);

                AddressDTO address = new AddressDTO { Complement = "GHI", Number = 30, ZipCode = "15997039" };
                List<string> employees = new List<string> { "085.122.908-56" };
                AgencyDTO agency = new AgencyDTO { Number = "1009", Address = address, CNPJ = "74955443000174", Employees = employees, Restriction = false };

                //var result = controller.PostAgency(agency);
                //var okResult = result.Result as OkObjectResult;
                //var agencyResult = okResult.Value as AgencyDTO;

                //Assert.NotNull(agencyResult);
                //Assert.Equal(agency.Number, agencyResult.Number);
                //Assert.Equal(agency.Address.Complement, agencyResult.Address.Complement);
                //Assert.Equal(agency.Address.Number, agencyResult.Address.Number);
                //Assert.Equal(agency.Address.ZipCode, agencyResult.Address.ZipCode);
                //Assert.Equal(agency.CNPJ, agencyResult.CNPJ);
                //Assert.Equal(agency.Employees, agencyResult.Employees);
                //Assert.Equal(agency.Restriction, agencyResult.Restriction);
            }
        }



        [Fact]
        public void TestGetAll()
        {
            InitializeDataBase();

            using (var context = new AgencyAPIContext(options))
            {
                // Mock services
                var addressServiceMock = new Mock<AddressService>(context);
                var employeeServiceMock = new Mock<EmployeeService>(context);
                var accountServiceMock = new Mock<AccountService>(context);

                var controller = new AgenciesController(context, addressServiceMock.Object, employeeServiceMock.Object, accountServiceMock.Object);

                var result = controller.GetAgency();
                var okResult = result.Result;
                var agencies = okResult.Value as List<Agency>;

                Assert.NotNull(agencies);
                Assert.Equal(2, agencies.Count);
            }


        }
    }
}