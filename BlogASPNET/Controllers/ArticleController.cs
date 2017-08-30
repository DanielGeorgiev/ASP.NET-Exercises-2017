using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //GET: Article/List
        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var articles = db.Articles.Include(a => a.Author)
                    .ToList();

                return View(articles);
            }
        }

        //GET: Article/Details/{INT id}
        public ActionResult Details(int? id)
        {
            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .FirstOrDefault();

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        //GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //POST: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    article.AuthorId = db.Users.Where(us => us.UserName == User.Identity.Name).First().Id;

                    db.Articles.Add(article);

                    db.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(article);
        }

        //GET: Article/Delete/{INT id}
        [Authorize]
        public ActionResult Delete(int? id)
        {
            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (!isUserPermitted(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        //POST: Article/Delete/{INT id}
        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (article == null)
                {
                    return HttpNotFound();
                }

                db.Articles.Remove(article);

                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        //GET: Article/Edit/{INT id}
        [Authorize]
        public ActionResult Edit(int? id)
        {
            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (!isUserPermitted(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    return HttpNotFound();
                }

                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;

                return View(model);
            }
        }

        //POST: Article/Edit/{INT id}
        [HttpPost]
        [Authorize]
        [ActionName("Edit")]
        public ActionResult EditConfirmed(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var article = db.Articles.Find(model.Id);

                    article.Title = model.Title;
                    article.Content = model.Content;

                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(model);
        }

        //Helper methods
        private bool isUserPermitted(Article article)
        {
            bool isAdmin = User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }
    }
}