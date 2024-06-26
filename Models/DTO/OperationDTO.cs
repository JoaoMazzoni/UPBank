using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class OperationDTO
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public string ?AccountNumber { get; set; }
        //[JsonIgnore]
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
