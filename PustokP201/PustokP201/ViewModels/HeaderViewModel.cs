using PustokP201.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokP201.ViewModels
{
    public class HeaderViewModel
    {
        public List<Setting> Settings { get; set; }
        public List<Genre> Genres { get; set; }
        public BasketViewModel Basket { get; set; }
    }
}
