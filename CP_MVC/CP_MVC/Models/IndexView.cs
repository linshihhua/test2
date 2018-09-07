using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CP_MVC.Models;

namespace CP_MVC.Models {

    public struct top {
        public int BookID;
        public int People;
    }

    public class IndexView {
        public List<WebMessage> TopWebMessageList { get; set; }
        public List<Book> TopBookList { get; set; }
    }
}