using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Models
{
    public class OrderItem:BaseEntity
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public int Count { get; set; }

        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}
