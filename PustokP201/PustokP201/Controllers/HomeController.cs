using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PustokP201.Models;
using PustokP201.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokContext _context;

        public HomeController(PustokContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel homeVM = new HomeViewModel
            {
                Sliders = _context.Sliders.ToList(),
                Features = _context.Features.ToList(),
                FeaturedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsFeatured).Take(20).ToList(),
                NewBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsNew).Take(20).ToList(),
                DiscountedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x=>x.DiscountPercent>0).Take(20).ToList()
            };
            return View(homeVM);
        }

        public IActionResult Partial()
        {
            return PartialView("_BookTabSlider", _context.Books.Include(x => x.Author).Include(x => x.BookImages).Take(20).ToList());
        }

        public IActionResult GetBook(int id)
        {
            Book book = _context.Books.Include(x=>x.BookTags).ThenInclude(bt=>bt.Tag).Include(x=>x.BookImages).Include(x=>x.Genre).FirstOrDefault(x => x.Id == id);
                

            //return Json(book);
            return PartialView("_ModalBookDetail", book);
        }

      
      
    }
}
