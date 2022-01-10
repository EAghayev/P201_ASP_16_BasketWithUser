using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokP201.Models;
using PustokP201.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Services
{
    public class LayoutService
    {
        private readonly PustokContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public LayoutService(PustokContext context,IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }
        public async Task<List<Setting>> GetSettings()
        {
            return await _context.Settings.ToListAsync();
        }

        public async Task<HeaderViewModel> GetHeaderViewModel()
        {
            BasketViewModel basket = null;
            var basketItemsStr = _httpContext.HttpContext.Request.Cookies["basketItemList"];

            if (basketItemsStr != null)
            {
                List<CookieBasketItemViewModel> cookieItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(basketItemsStr);
                basket = _getBasketItems(cookieItems);
            }

            HeaderViewModel headerVM = new HeaderViewModel
            {
                Genres = await _context.Genres.ToListAsync(),
                Settings = await _context.Settings.ToListAsync(),
                Basket = basket
            };
            return headerVM;
        }

        private BasketViewModel _getBasketItems(List<CookieBasketItemViewModel> cookieBasketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };

            foreach (var item in cookieBasketItems)
            {
                Book book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == item.BookId);
                BasketItemViewModel basketItem = new BasketItemViewModel
                {
                    Name = book.Name,
                    Price = book.DiscountPercent > 0 ? (book.SalePrice * (1 - book.DiscountPercent / 100)) : book.SalePrice,
                    BookId = book.Id,
                    Count = item.Count,
                    PosterImage = book.BookImages.FirstOrDefault(x => x.PosterStatus == true)?.Image
                };

                basketItem.TotalPrice = basketItem.Count * basketItem.Price;
                basket.TotalAmount += basketItem.TotalPrice;
                basket.BasketItems.Add(basketItem);
            }

            return basket;
        }


    }
}
