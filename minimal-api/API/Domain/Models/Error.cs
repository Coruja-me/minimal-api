using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.Models
{
    public struct Error
    {
        public List<string> Msgs { get; set; }
    }
}