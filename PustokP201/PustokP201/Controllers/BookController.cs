using Microsoft.AspNetCore.Http;
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

namespace PustokP201.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookController(PustokContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int? genreId)
        {
            var books = _context.Books.Include(x => x.Genre).Include(x => x.Author).Include(x => x.BookImages).AsQueryable();

            if (genreId != null)
                books = books.Where(x => x.GenreId == genreId);

            BookViewModel bookVM = new BookViewModel
            {
                Genres = _context.Genres.Include(x => x.Books).ToList(),
                Books = books.ToList()
            };
            return View(bookVM);
        }

        public ActionResult SetSession(int id)
        {
            HttpContext.Session.SetString("bookId", id.ToString());

            return Content("added");
        }

        public ActionResult GetSession()
        {
            string idStr = HttpContext.Session.GetString("bookId");

            return Content(idStr);
        }

        public IActionResult SetCookie(int id)
        {
            HttpContext.Response.Cookies.Append("bookId", id.ToString());
            return Content("cookie");
        }

        public IActionResult GetCookie(string key)
        {
            string str = HttpContext.Request.Cookies[key];

            return Content(str);
        }

        public async Task<IActionResult> AddBasket(int bookId)
        {
            if (!_context.Books.Any(x => x.Id == bookId))
            {
                return NotFound();
            }

            BasketViewModel data = null;

            AppUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user!=null && user.IsAdmin==false)
            {
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(x => x.AppUserId == user.Id && x.BookId == bookId);

                if(basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        BookId = bookId,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }

                _context.SaveChanges();

                data = _getBasketItems(_context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.BookImages).Where(x => x.AppUserId == user.Id).ToList());
            }
            else
            {
                List<CookieBasketItemViewModel> basketItems = new List<CookieBasketItemViewModel>();
                string existBasketItems = HttpContext.Request.Cookies["basketItemList"];

                if (existBasketItems != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(existBasketItems);
                }

                CookieBasketItemViewModel item = basketItems.FirstOrDefault(x => x.BookId == bookId);

                if (item == null)
                {
                    item = new CookieBasketItemViewModel
                    {
                        BookId = bookId,
                        Count = 1
                    };
                    basketItems.Add(item);
                }
                else
                    item.Count++;

                var bookIdsStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("basketItemList", bookIdsStr);

                data = _getBasketItems(basketItems);
            }
            

            return Ok(data);
        }

        public IActionResult ShowBasket()
        {
            var bookIdsStr = HttpContext.Request.Cookies["basketItemList"];
            List<CookieBasketItemViewModel> bookIds = new List<CookieBasketItemViewModel>();
            if (bookIdsStr != null)
            {
                bookIds = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(bookIdsStr);
            }
            return Json(bookIds);
        }

        public async Task<IActionResult> Checkout()
        {
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();

            AppUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user != null && user.IsAdmin == false)
            {
                List<BasketItem> basketItems = _context.BasketItems.Include(x=>x.Book).Where(x => x.AppUserId == user.Id).ToList();

                foreach (var item in basketItems)
                {
                    CheckoutItemViewModel checkoutItem = new CheckoutItemViewModel
                    {
                        Book = item.Book,
                        Count = item.Count
                    };
                    checkoutItems.Add(checkoutItem);
                }
            }
            else
            {
                string basketItemsStr = HttpContext.Request.Cookies["basketItemList"];
                if (basketItemsStr != null)
                {
                    List<CookieBasketItemViewModel> basketItems = JsonConvert.DeserializeObject<List<CookieBasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItems)
                    {
                        CheckoutItemViewModel checkoutItem = new CheckoutItemViewModel
                        {
                            Book = _context.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }
            

            return View(checkoutItems);
        }

        private BasketViewModel _getBasketItems(List<CookieBasketItemViewModel> cookieBasketItems)
        {
            BasketViewModel basket = new BasketViewModel
            {
                BasketItems = new List<BasketItemViewModel>(),
            };

            foreach (var item in cookieBasketItems)
            {
                Book book = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == item.BookId);
                BasketItemViewModel basketItem = new BasketItemViewModel
                {
                    Name = book.Name,
                    Price = book.DiscountPercent>0?(book.SalePrice*(1-book.DiscountPercent/100)):book.SalePrice,
                    BookId = book.Id,
                    Count = item.Count,
                    PosterImage = book.BookImages.FirstOrDefault(x=>x.PosterStatus==true)?.Image
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

        public IActionResult Detail(int id)
        {
            Book book = _context.Books
                .Include(x=>x.BookImages).Include(x=>x.Genre)
                .Include(x=>x.BookTags).ThenInclude(x=>x.Tag)
                .Include(x=>x.Author).Include(x=>x.BookComments)
                .FirstOrDefault(x => x.Id == id);

            if (book == null) return NotFound();

            BookDetailViewModel bookDetailVM = new BookDetailViewModel
            {
                Book = book,
                Comment = new BookComment(),
                RelatedBooks = _context.Books
                .Include(x=>x.BookImages).Include(x=>x.Author)
                .Where(x => x.GenreId == book.GenreId).OrderByDescending(x => x.Id).Take(5).ToList()
            };

            return View(bookDetailVM);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(BookComment comment)
        {
            Book book = _context.Books
               .Include(x => x.BookImages).Include(x => x.Genre)
               .Include(x => x.BookTags).ThenInclude(x => x.Tag)
               .Include(x => x.Author).Include(x => x.BookComments)
               .FirstOrDefault(x => x.Id == comment.BookId);

            if (book == null) return NotFound();

            BookDetailViewModel bookDetailVM = new BookDetailViewModel
            {
                Book = book,
                Comment = comment,
                RelatedBooks = _context.Books
                .Include(x => x.BookImages).Include(x => x.Author)
                .Where(x => x.GenreId == book.GenreId).OrderByDescending(x => x.Id).Take(5).ToList()
            };

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Comment model is not valid!";
                return View("Detail", bookDetailVM);
            }

            if (!_context.Books.Any(x=>x.Id == comment.BookId))
            {
                TempData["error"] = "Selected Book not found";
                return View("Detail", bookDetailVM);
            }

            if (!User.Identity.IsAuthenticated)
            {
                if(string.IsNullOrWhiteSpace(comment.Email))
                {
                    TempData["error"] = "Email is required";
                    return View("Detail", bookDetailVM);
                }

                if (string.IsNullOrWhiteSpace(comment.FullName))
                {
                    TempData["error"] = "FullName is required";
                    return View("Detail", bookDetailVM);
                }
            }
            else
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                comment.AppUserId = user.Id;
                comment.FullName = user.FullName;
                comment.Email = user.Email;
            }

            comment.Status = false;
            comment.CreatedAt = DateTime.UtcNow.AddHours(4);
            _context.BookComments.Add(comment);
            _context.SaveChanges();

            TempData["Success"] = "Comment added successfully";

            return RedirectToAction("detail",new { Id = comment.BookId});
        }

    }
}
