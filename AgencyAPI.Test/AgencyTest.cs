using AgencyAPI.Controllers;
using AgencyAPI.Data;
using AgencyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Moq;
using System.Xml.Linq;

namespace AgencyAPI.Test
{
    public class AgencyTest
    {
        private readonly AgencyAPIContext _context;
        private readonly Mock<IAddressService> _mockAddressService;
        private readonly Mock<IEmployeeService> _mockEmployeeService;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AgenciesController _controller;

        static Employee employee = new Employee
        {
            Name = "marcelo",
            Document = "08512290854",
            Manager = false,
            BirthDate = "1985-05-15",
            Gender = "Male",
            Salary = 75000.00,
            Email = "marco.doe@example.com",
            Phone = "1234567890",

            Address = new Address
            {
                ZipCode = "15997060",
                Number = 12,
                Complement = ""
            },
            Register = 0,
            AddressId = "1599706012"
        };
        
        static Employee employee1 = new Employee
        {
            Name = "maria",
            Document = "49625029800",
            Manager = false,
            BirthDate = "1985-05-15",
            Gender = "Female",
            Salary = 75000.00,
            Email = "maria.doe@example.com",
            Phone = "1234567890",

            Address = new Address
            {
                ZipCode = "15997050",
                Number = 190,
                Complement = ""
            },
            Register = 0,
            AddressId = "15997050190"
        };
        
        static AgencyDTO agencyDTO = new AgencyDTO
        {
            Number = "0932",
            CNPJ = "35159537000183",
            Employees = new List<string> { employee.Document },
            Restriction = false,
            Address = new AddressDTO
            {
                ZipCode = "15997060",
                Number = 12,
                Complement = ""
            }
        };

        static AgencyPatchDTO agencyPatchDTO = new AgencyPatchDTO
        {
            Employees = new List<string> { employee1.Document },
            Address = new AddressDTO
            {
                ZipCode = "",
                Number = 0,
                Complement = ""
            }
        };

        static Agency agency = new Agency
        {
            Number = "0932",
            CNPJ = "35159537000183",
            Employees = new List<Employee> { employee },
            Restriction = false,
            Address = address,
            AddressId = "1599706012"
        };

        static AddressDTO addressDTO = new AddressDTO
        {
            ZipCode = "15997060",
            Number = 12,
            Complement = ""
        };

        static Address address = new Address
        {
            Id = "1599706012",
            ZipCode = agencyDTO.Address.ZipCode,
            Number = agencyDTO.Address.Number,
            Complement = agencyDTO.Address.Complement,
            Street = "Rua 1",
            Neighborhood = "Bairro 1",
            City = "Cidade 1",
            State = "SP"
        };

        public AgencyTest()
        {
            var options = new DbContextOptionsBuilder<AgencyAPIContext>().UseInMemoryDatabase("AgencyAPITest").Options;
            _context = new AgencyAPIContext(options);
            _mockAddressService = new Mock<IAddressService>();
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AgenciesController(_context, _mockAddressService.Object, _mockEmployeeService.Object, _mockAccountService.Object);
        }

        [Fact]
        public async void PostAgency_ReturnBadRequest_WhenCNPJInvalid()
        {
            var agencyDTO = new AgencyDTO
            {
                CNPJ = "10000000000",
            };

            var result = await _controller.PostAgency(agencyDTO);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void PostAgency_ReturnBadRequest_WhenCNPJEmpty()
        {
            var agencyDTO = new AgencyDTO
            {
                CNPJ = "",
            };

            var result = await _controller.PostAgency(agencyDTO);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void PostAgency_ReturnBadRequest_WhenManagerNotPresent()
        {
            employee.Manager = false;
            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            var agencyDTO = new AgencyDTO
            {
                CNPJ = "35159537000183",
                Employees = new List<string> { employee.Document }
            };

            var result = await _controller.PostAgency(agencyDTO);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void PostAgency_ReturnSucess()
        {

            employee.Manager = true;

            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            var result = await _controller.PostAgency(agencyDTO);

            Assert.True(result.Result is CreatedAtActionResult);
        }

        [Fact]
        public async void DeleteAgency_ReturnSucess()
        {

            employee.Manager = true;

            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            await _controller.PostAgency(agencyDTO);            

            var resultDelete = await _controller.DeleteAgency(agencyDTO.Number);

            Assert.IsType<OkObjectResult>(resultDelete);
        }

        [Fact]
        public async void DeleteAndRestorageAgency_ReturnSucess()
        {
            employee.Manager = true;

            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            await _controller.PostAgency(agencyDTO);

            var resultDelete = await _controller.DeleteAgency(agencyDTO.Number);

            var resultRestorage = await _controller.RestorageAgency(agencyDTO.Number);

            Assert.IsType<OkObjectResult>(resultRestorage);
        }

        [Fact]
        public async void GetAgencyByNumber_ReturnSucess()
        {
            employee.Manager = true;

            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            await _controller.PostAgency(agencyDTO);

            var resultGet = await _controller.GetAgency(agencyDTO.Number);

            Assert.IsType<OkObjectResult>(resultGet.Result);
        }

        [Fact]
        public async void GetAgency_ReturnSucess()
        {
            employee.Manager = true;

            _mockEmployeeService.Setup(service => service.PostEmployee(employee));
            var employeeResult = _mockEmployeeService.Setup(e => e.GetEmployee(It.IsAny<string>())).ReturnsAsync(employee);

            await _controller.PostAgency(agencyDTO);

            var resultGet = await _controller.GetAgency();

            Assert.IsType<OkObjectResult>(resultGet.Result);
        }
    }
}