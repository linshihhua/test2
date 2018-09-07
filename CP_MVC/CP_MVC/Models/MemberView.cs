using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CP_MVC.Models {
    public class MemberView {
        public Member member { get; set; }
        public List<Book> BookList { get; set; }
    }
}