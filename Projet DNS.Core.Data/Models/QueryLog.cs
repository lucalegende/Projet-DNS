using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DNS.Core.Data.Models
{
    public class QueryLog
    {
        //public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string IP { get; set; }
        public string Domain { get; set; }
        public bool Blacklist { get; set; }
    }
}
