using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DNS.Core.Data.Models
{
    public class Profile
    {

        public int Id { get; set; }
        public string EmailPremier { get; set; }
        public string EmailSecond { get; set; }
        public string PassWord { get; set; }
    }
}
