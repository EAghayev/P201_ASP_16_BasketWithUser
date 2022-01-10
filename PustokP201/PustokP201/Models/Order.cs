using PustokP201.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Models
{
    public class Order:BaseEntity
    {
        public string AppUserId { get; set; }

        [Required]
        [StringLength(maximumLength:100)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string Phone { get; set;}
        [Required]
        [StringLength(maximumLength: 250)]
        public string Address1 { get; set; }
        [StringLength(maximumLength: 250)]
        public string Address2 { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string Country { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string City { get; set; }
        [Required]
        [StringLength(maximumLength: 20)]
        public string Zipcode { get; set; }
        [StringLength(maximumLength: 350)]
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public OrderDeliveryStatus DeliveryStatus { get; set; }

        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
