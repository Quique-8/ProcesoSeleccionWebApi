using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaProsegur.WebApi.Data.Entities
{
    public class Explication
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
    }
}
