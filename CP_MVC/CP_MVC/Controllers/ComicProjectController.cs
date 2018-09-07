using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP_MVC.Models;

namespace CP_MVC.Controllers
{
    public class ComicProjectController : Controller
    {
        // GET: ComicProject

        ComicProjectDBEntities db = new ComicProjectDBEntities();

        public ActionResult Index()
        {
            IndexView IV = new IndexView();

            var query1 = (from o in db.WebMessages
                          orderby o.WebMessageID descending
                          select o).Take(4);

            IV.TopWebMessageList = query1.ToList();

            var query2 = from o in db.Books
                         select o.BookID;

            int i = 0;
            int end = query2.Count();

            int[] People = new int[end];

            foreach (var item in query2.ToList()) {
                var query3 = from o in db.BookDetailEpisodes
                             where o.BookID == item
                             select o.BookDetailEpisodeID;

                var query4 = from o in db.Possessions
                             where query3.Contains(o.BookDetailEpisodeID.Value)
                             select o;

                People[i] = query4.Count();
                i++;
            }
            List<top> topList = new List<top>();
            top temp;
            for (int j = 0; j < People.Length; j++) {
                int k = j + 1;
                temp.BookID = k;
                temp.People = People[j];
                topList.Add(temp);
            }

            List<top> SortedList = topList.OrderBy(o => o.People).ToList();
            SortedList.Reverse();

            IV.TopBookList = new List<Book>();

            for (int j = 0; j < 6; j++) {
                var query5 = from o in db.Books.AsEnumerable()
                             where o.BookID == SortedList[j].BookID
                             select o;
                IV.TopBookList.Add(query5.Single());
            }
            return View(IV);
        }

        public ActionResult Login() {
            Member member = new Member();
            return View(member);
        }

        [HttpPost]
        public ActionResult Login(Member member1) {

            var query = from o in db.Members.AsEnumerable()
                        where o.MemberName == member1.MemberName & o.MemberPassword == member1.MemberPassword
                        select o;

            
            Member member2 = query.SingleOrDefault();
            if (member2 != null) {
                Session["Login"] = "true";
                Session["MemberID"] = member2.MemberID;
                Session["MemberName"] = member2.MemberName;
                Session["MemberStatus"] = member2.MemberStatus;
                string url = "/ComicProject/Member/" + member2.MemberID.ToString();
                return Redirect(url);
            }else {
                return View(member1);
            }
           
        }


        public ActionResult Member(int? id) {
            if (Session["Login"].ToString() == "false" || Session["MemberName"].ToString() == "Guest") {
                return RedirectToAction("Login");
            }
            else {
                if(id == null) {
                    id = Convert.ToInt32(Session["MemberID"]);
                }
                MemberView MV = new MemberView();
                var query4 = from o in db.Members.AsEnumerable()
                             where o.MemberID == id
                             select o;

                MV.member = query4.SingleOrDefault();

                MV.BookList = new List<Book>();
                var query = from o in db.Possessions.AsEnumerable()
                            where o.MemberID.Value == MV.member.MemberID
                            select o.BookDetailEpisodeID;

                var query1 = from o in db.Books
                             select o;
                for (int i = 1; i <= query1.Count(); i++) {
                    bool HaveTest = false;
                    var query2 = from o in db.BookDetailEpisodes.AsEnumerable()
                                 where o.BookID.Value == i
                                 select o.BookDetailEpisodeID;
                    foreach (var item in query2) {
                        if (query.Contains(item)) {
                            HaveTest = true;
                        }
                    }
                    if (HaveTest) {
                        var query3 = from o in db.Books
                                     where o.BookID == i
                                     select o;
                        MV.BookList.Add(query3.Single());
                    }
                }
                return View(MV);
            }
        }

        public ActionResult Edit()
        {
            if(Session["Login"].ToString() == "true")
            {
                var query = from o in db.Members.AsEnumerable()
                            where o.MemberID == Convert.ToInt32(Session["MemberID"])
                            select o;
                Member member = query.Single();
                return View(member);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Edit(Member member)
        {
            if(Request.Form["OkOrCancelButton"] == "Ok")
            {
                db.Entry(member).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Member", member);
            }
            else
            {
                Member member1 = db.Members.Find(member.MemberID);
                return RedirectToAction("Member", member1);
            }
        }

        public ActionResult BookDetail(int? id)
        {
            BookDetailView BV = new BookDetailView();
            BV.Book = new Book();
            BV.HaveList = new List<BookDetailEpisode>();
            BV.NotHaveList = new List<BookDetailEpisode>();

            var query = from o in db.Books.AsEnumerable()
                        where o.BookID == id
                        select o;
            BV.Book = query.SingleOrDefault();

            if(Session["Login"].ToString() == "false")
            {
                var query1 = from o in db.BookDetailEpisodes.AsEnumerable()
                             where o.BookID.Value == id
                             select o;

                BV.NotHaveList = query1.ToList();
                return View(BV);
            }else
            {
                var query2 = from o in db.BookDetailEpisodes.AsEnumerable()
                             where o.BookID == id
                             select o.BookDetailEpisodeID;
                var query3 = from o in db.Possessions.AsEnumerable()
                             where o.MemberID == Convert.ToInt32(Session["MemberID"])
                             select o.BookDetailEpisodeID;

                foreach(var item in query2)
                {
                    if (query3.Contains(item))
                    {
                        var query4 = from o in db.BookDetailEpisodes.AsEnumerable()
                                     where o.BookDetailEpisodeID == item
                                     select o;

                        BV.HaveList.Add(query4.Single());
                    }else
                    {
                        var query5 = from o in db.BookDetailEpisodes.AsEnumerable()
                                     where o.BookDetailEpisodeID == item
                                     select o;
                        BV.NotHaveList.Add(query5.Single());
                    }
                }
                return View(BV);
            }
        }

        public ActionResult Add(int? id) {
            if (Session["Login"].ToString() == "false") {
                return RedirectToAction("Login");
            }
            else {
                Possession possession = new Possession();
                var query = from o in db.Possessions
                            select o.PossessionID;
                int possessionID = query.Max() + 1;
                possession.PossessionID = possessionID;
                possession.MemberID = Convert.ToInt32(Session["MemberID"]);
                possession.BookDetailEpisodeID = id;

                db.Possessions.Add(possession);
                db.SaveChanges();

                var query1 = from o in db.BookDetailEpisodes.AsEnumerable()
                             where o.BookDetailEpisodeID == id
                             select o.BookID;

                int BookID = (int)query1.SingleOrDefault();
                string url = "/ComicProject/BookDetail/" + BookID.ToString();
                return Redirect(url);
            }
        }

        public ActionResult Remove(int? id) {
            var query = from o in db.Possessions.AsEnumerable()
                        where o.MemberID == Convert.ToInt32(Session["MemberID"]) & o.BookDetailEpisodeID == id
                        select o;
            Possession possession = query.SingleOrDefault();
            db.Possessions.Remove(possession);
            db.SaveChanges();

            var query1 = from o in db.BookDetailEpisodes.AsEnumerable()
                         where o.BookDetailEpisodeID == id
                         select o.BookID;

            int BookID = (int)query1.SingleOrDefault();
            string url = "/ComicProject/BookDetail/" + BookID.ToString();
            return Redirect(url);

        }

        public ActionResult Logout() {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult Search() {
            List<Book> Result = new List<Book>();
            string SearchString = Request.Form["Search"];
            var query = from o in db.Books
                        where (o.BookAuthor.Contains(SearchString) || o.BookName.Contains(SearchString))
                        select o;
            Result = query.ToList();
            return View(Result);
        }

        
    }
}