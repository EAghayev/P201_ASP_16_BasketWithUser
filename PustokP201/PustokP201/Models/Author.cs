using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Models
{
    public class Author:BaseEntity
    {
        [Required(ErrorMessage ="Bos buraxmaaa!")]
        [StringLength(maximumLength:50)]
        public string FullName { get; set; }
        public int BornYear { get; set; }
        [StringLength(maximumLength:25,ErrorMessage ="25-den uzun yazma!")]
        public string Country { get; set; }

        public List<Book> Books { get; set; }
    }
}
