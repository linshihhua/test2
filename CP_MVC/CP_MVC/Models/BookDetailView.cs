using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CP_MVC.Models
{
    public class BookDetailView
    {
        public Book Book { get; set; }
        public List<BookDetailEpisode> HaveList { get; set; }
        public List<BookDetailEpisode> NotHaveList { get; set; }
    }
}