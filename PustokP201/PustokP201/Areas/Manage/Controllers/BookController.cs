using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PustokP201.Helper;
using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {

            ViewBag.SelectedPage = page;
            ViewBag.TotalPage = (int)Math.Ceiling(_context.Books.Count() / 4d);

            return View(_context.Books.Include(x=>x.BookComments).Include(x=>x.Genre).Include(x=>x.Author).Include(x=>x.BookImages).Skip((page-1)*4).Take(4).ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();



            if (!ModelState.IsValid) return View();

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author not found");
                return View();
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found");
                return View();
            }

            book.BookImages = new List<BookImage>();

            if (book.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "Poster file is required");
                return View();
            }
            else
            {
                if (book.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterFile", "PosterFile max size is 2MB");
                    return View();
                }
                if (book.PosterFile.ContentType != "image/jpeg" && book.PosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterFile", "ContentType must be image/jpeg or image/png");
                    return View();
                }

                BookImage poster = new BookImage
                {
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterFile),
                    Book = book,
                    PosterStatus = true
                };

                //_context.BookImages.Add(poster);
                book.BookImages.Add(poster);
            }

            if (book.HoverPosterFile == null)
            {
                ModelState.AddModelError("HoverPosterFile", "HoverPosterFile file is required");
                return View();
            }
            else
            {
                if (book.HoverPosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverPosterFile", "HoverPosterFile max size is 2MB");
                    return View();
                }
                if (book.HoverPosterFile.ContentType != "image/jpeg" && book.HoverPosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterFile", "ContentType must be image/jpeg or image/png");
                    return View();
                }

                BookImage poster = new BookImage
                {
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterFile),
                    Book = book,
                    PosterStatus = false
                };

                _context.BookImages.Add(poster);
            }



            if (book.ImageFiles != null)
            {
                foreach (var item in book.ImageFiles)
                {
                    if (item.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "ImageFile max size is 2MB");
                        return View();
                    }
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "ContentType must be image/jpeg or image/png");
                        return View();
                    }
                    

                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", item)
                    };

                    _context.BookImages.Add(bookImage);
                }
            }

            if (book.TagIds != null)
            {
                book.BookTags = new List<BookTag>();

                foreach (var tagId in book.TagIds)
                {
                    BookTag bookTag = new BookTag
                    {
                        TagId = tagId,
                        Book = book
                    };

                    //_context.BookTags.Add(bookTag);
                    book.BookTags.Add(bookTag);
                }
            }



            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            Book book = await _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefaultAsync(x => x.Id == id);
            if (book == null) return NotFound();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            Book existBook = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null) return NotFound();

            if (book.PosterFile != null)
            {
                if (book.PosterFile.Length > 2097152)
                    ModelState.AddModelError("PosterFile", "File size can not be more than 2MB");
                if (book.PosterFile.ContentType != "image/jpeg" && book.PosterFile.ContentType != "image/png")
                    ModelState.AddModelError("PosterFile", "Content type must be png ,jpeg or jpg");
            }

            if(book.HoverPosterFile != null)
            {
                if (book.HoverPosterFile.Length > 2097152)
                    ModelState.AddModelError("HoverPosterFile", "File size can not be more than 2MB");
                if (book.HoverPosterFile.ContentType != "image/jpeg" && book.HoverPosterFile.ContentType != "image/png")
                    ModelState.AddModelError("HoverPosterFile", "Content type must be png ,jpeg or jpg");
            }

            if (book.ImageFiles != null)
            {
                foreach (var item in book.ImageFiles)
                {
                    if (item.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "File size can not be more than 2MB");
                        break;
                    }
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "Content type must be png ,jpeg or jpg");
                        break;
                    }
                }

            }

            if (!ModelState.IsValid) return View();

            if (book.PosterFile != null)
            {
                BookImage currentPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);

                if (currentPoster == null) return NotFound();

                _setBookImage(currentPoster, book.PosterFile);
            }

            if (book.HoverPosterFile != null)
            {
                BookImage currentPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                if (currentPoster == null) return NotFound();

                _setBookImage(currentPoster, book.HoverPosterFile);
            }

            if (book.ImageFiles != null)
            {
                foreach (var imageItem in existBook.BookImages.Where(x=>x.PosterStatus==null))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", imageItem.Image);
                    _context.BookImages.Remove(imageItem);
                }

                foreach (var imageFile in book.ImageFiles)
                {
                    BookImage bookImage = new BookImage
                    {
                        BookId = book.Id,
                        PosterStatus = null,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/books", imageFile)
                    };

                    _context.BookImages.Add(bookImage);
                    //existBook.BookImages.Add(bookImage);
                }
            }

            _setBookData(existBook, book);

            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));

            foreach (var tagId in book.TagIds.Where(x=>!existBook.BookTags.Any(bt=>bt.TagId==x)))
            {
                BookTag bookTag = new BookTag
                {
                    BookId = book.Id,
                    TagId = tagId
                };

                existBook.BookTags.Add(bookTag);
            }

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        private void _setBookData(Book existBook,Book book)
        {
            existBook.Code = book.Code;
            existBook.CostPrice = book.CostPrice;
            existBook.SalePrice = book.SalePrice;
            existBook.Name = book.Name;
            existBook.IsNew = book.IsNew;
            existBook.IsFeatured = book.IsFeatured;
            existBook.DiscountPercent = book.DiscountPercent;
        }
        private void _setBookImage(BookImage image, IFormFile file)
        {
            string newFileName = FileManager.Save(_env.WebRootPath, "uploads/books", file);

            FileManager.Delete(_env.WebRootPath, "uploads/books", image.Image);
            image.Image = newFileName;
        }

        public IActionResult Comments(int bookId)
        {
            List<BookComment> comments = _context.BookComments.Include(x=>x.Book).Where(x => x.BookId == bookId).ToList();
            return View(comments);
        }

        public IActionResult DeleteComment(int id)
        {
            BookComment comment = _context.BookComments.FirstOrDefault(x => x.Id == id);

            if (comment == null) return NotFound();

            _context.BookComments.Remove(comment);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult InfoComment(int id)
        {
            BookComment comment = _context.BookComments.Include(x=>x.Book).FirstOrDefault(x => x.Id == id);

            if (comment == null) return NotFound();

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcceptComment(int id)
        {
            BookComment comment = _context.BookComments.FirstOrDefault(x => x.Id == id);

            if (comment == null) return NotFound();

            comment.Status = true;

            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
