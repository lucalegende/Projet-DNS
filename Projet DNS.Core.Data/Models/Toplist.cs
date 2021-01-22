using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DNS.Core.Data.Models
{
    public class Toplist
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public int Hits { get; set; }
        public int Frequence { get; set; }
        public bool blocked { get; set; }
    }
}
