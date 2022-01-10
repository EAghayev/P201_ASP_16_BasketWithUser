using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokP201.Models;
using PustokP201.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly PustokContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(PustokContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            BasketViewModel basket = null;

            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }


            if(user!=null && user.IsAdmin == false)
            {
                basket = _getBasketItems(_context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.BookImages).Where(x => x.AppUserId == user.Id).ToList());
            }
            else
            {
                var basketItemsStr = HttpContext.Request.Cookies["basketItemList"];

                if (basketItemsStr != null)
                {
                    List<CookieBasketItemViewModel> cookieItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(basketItemsStr);
                    basket = _getBasketItems(cookieItems);
                }
            }
           

            HeaderViewModel headerVM = new HeaderViewModel
            {
                Genres = await _context.Genres.ToListAsync(),
                Settings = await _context.Settings.ToListAsync(),
                Basket = basket
            };
            return View(headerVM);
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

        private BasketViewModel _getBasketItems(List<BasketItem> basketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };

            foreach (var item in basketItems)
            {
                BasketItemViewModel basketItem = new BasketItemViewModel
                {
                    Name = item.Book.Name,
                    Price = item.Book.DiscountPercent > 0 ? (item.Book.SalePrice * (1 - item.Book.DiscountPercent / 100)) : item.Book.SalePrice,
                    BookId = item.Book.Id,
                    Count = item.Count,
                    PosterImage = item.Book.BookImages.FirstOrDefault(x => x.PosterStatus == true)?.Image
                };

                basketItem.TotalPrice = basketItem.Count * basketItem.Price;
                basket.TotalAmount += basketItem.TotalPrice;
                basket.BasketItems.Add(basketItem);
            }

            return basket;
        }
    }
}
