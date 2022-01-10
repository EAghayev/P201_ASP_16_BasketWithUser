using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GenreController : Controller
    {
        private readonly PustokContext _context;

        public GenreController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View(_context.Genres.Include(x=>x.Books).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) return View();

            _context.Genres.Add(genre);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid) return View();
            Genre existGenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);

            if (existGenre == null) return NotFound();

            existGenre.Name = genre.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
