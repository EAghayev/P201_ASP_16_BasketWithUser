using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Models
{
    public class Book:BaseEntity
    {
        public int GenreId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [StringLength(maximumLength:150)]
        public string Name { get; set; }
        [StringLength(maximumLength: 500)]
        public string Desc { get; set; }
        [StringLength(maximumLength: 10)]
        public string Code { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public bool StockStatus { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }

        public Genre Genre { get; set; }
        public Author Author { get; set; }

        public List<BookImage> BookImages { get; set; }
        public List<BookTag> BookTags { get; set; }
        [NotMapped]
        public IFormFile PosterFile { get; set; }
        [NotMapped]
        public IFormFile HoverPosterFile { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        [NotMapped]
        public List<int> BookImageIds { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; } = new List<int>();

        public List<BookComment> BookComments { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}
