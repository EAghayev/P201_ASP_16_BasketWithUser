using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PustokP201.Helper;
using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SliderController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (slider.Title1 == slider.Title2)
                ModelState.AddModelError("Title2", "Title2 must not be same as Title1");

            if (slider.ImageFile == null)
                ModelState.AddModelError("ImageFile", "ImageFile is required");
            else if (slider.ImageFile.Length > 2097152)
                ModelState.AddModelError("ImageFile", "ImageFile max size is 2MB");
            else if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                ModelState.AddModelError("ImageFile", "ContentType must be image/jpeg or image/png");

            if (!ModelState.IsValid) return View();

            slider.Image = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            
            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            if (slider.ImageFile != null)
            {
                return Content("Yeni fayl gelir,kohne gedir");
            }
            else if (slider.Image == null)
            {
                return Content("Silmek isteyir");
            }
            else return Content("her sey oldugu kimi qalsin");

            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null) return NotFound();

            existSlider.Title1 = slider.Title1;
            existSlider.Title2 = slider.Title2;
            existSlider.Desc = slider.Desc;
            existSlider.BtnText = slider.BtnText;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.Order = slider.Order;

            _context.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(slider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", slider.Image);
            }
            
            _context.Sliders.Remove(slider);
            _context.SaveChanges();

            return Ok();
        }
    }
}
