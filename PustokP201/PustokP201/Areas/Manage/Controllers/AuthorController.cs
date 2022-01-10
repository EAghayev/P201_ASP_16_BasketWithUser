using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly PustokContext _context;

        public AuthorController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = (int)Math.Ceiling(Convert.ToDouble(_context.Authors.Count()) / 2);
            ViewBag.SelectedPage = page;
            return View(_context.Authors.Include(x=>x.Books).Skip((page-1)*2).Take(2).ToList());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);

            return View(author);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Author author)
        {

            if (!ModelState.IsValid) return View();

            Author existAuthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (existAuthor == null)
            {
                return NotFound();
            }

            existAuthor.FullName = author.FullName;
            existAuthor.BornYear = author.BornYear;
            existAuthor.Country = author.Country;


            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);

            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Author author)
        {
            Author existAuthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (existAuthor == null) return NotFound();

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
