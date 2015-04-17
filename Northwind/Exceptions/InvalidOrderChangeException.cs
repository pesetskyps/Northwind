using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Exceptions
{
    public class InvalidOrderChangeException
    {
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
