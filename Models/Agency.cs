using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class Agency
{
    [Key] public string Number { get; set; }
    public Address Address { get; set; }
    public string CNPJ { get; set; }
    public List<Employee> Employees { get; set; }
    public bool Restriction { get; set; }

    public ICollection<AgencyEmployee> AgencyEmployees { get; set; }
}