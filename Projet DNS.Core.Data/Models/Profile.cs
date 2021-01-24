using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DNS.Core.Data.Models
{
    [Table("Profile")]
    public class Profile
    {

        public int Id { get; set; }
        public string EmailFirst { get; set; }
        public string Password { get; set; }
    }
}
