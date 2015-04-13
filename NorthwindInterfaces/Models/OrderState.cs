using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindInterfaces.Models
{
    public enum OrderState
    {
        [EnumMember]
        New,
        [EnumMember]
        InWork,
        [EnumMember]
        Shipped
    }
}
