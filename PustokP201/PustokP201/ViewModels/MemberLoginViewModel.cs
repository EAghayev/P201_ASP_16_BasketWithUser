using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [StringLength(maximumLength:20)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 25, MinimumLength = 8)]
        public string Password { get; set; }
        public bool IsPersistent { get; set; }

    }
}
