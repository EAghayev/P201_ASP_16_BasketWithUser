using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.ViewModels
{
    public class BookViewModel
    {
        public List<Genre> Genres { get; set; }
        public List<Book> Books { get; set; }
    }
}
