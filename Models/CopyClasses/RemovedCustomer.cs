

namespace Models.CopyClasses
{
    public class RemovedCustomer : Person
    {
        public bool Restriction { get; set; }


        public RemovedCustomer CopyCustomer(Customer customer)
        {
            var removedCustomer = new RemovedCustomer
            {

                Document = customer.Document,
                Name = customer.Name,
                BirthDate = customer.BirthDate,
                Gender = customer.Gender,
                Salary = customer.Salary,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                AddressId = customer.AddressId
            };

            return removedCustomer;

        }

    }
}
