using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Models
{
    public class BasketItem:BaseEntity
    {
        public int BookId { get; set; }
        public string AppUserId { get; set; }
        public int Count { get; set; }


        public AppUser AppUser { get; set; }
        public Book Book { get; set; }
    }
}
