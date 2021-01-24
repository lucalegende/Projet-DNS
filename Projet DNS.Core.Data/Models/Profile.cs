using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Projet_DNS.Core.Data.Models
{
    [Table("Profile")]
    public partial class Profile
    {
        public int Id { get; set; }
        public string EmailFirst { get; set; }
        public string Password { get; set; }
    }
}
