namespace BlogSystem.Controllers
{
    using Microsoft.AspNet.Identity;
    using Models;
    using System.Web.Mvc;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    public class ArticleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var artticles = db.Articles.Include(a => a.Author).ToList();

                return View(artticles);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Article model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = this.User.Identity.GetUserId();

                    model.AuthorId = authorId;

                    db.Articles.Add(model);

                    db.SaveChanges();
                }
                return RedirectToAction("List");
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Include(a => a.Author)
                    .Where(a => a.Id == id)
                    .FirstOrDefault();

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                // Get article from database
                //var article = db.Articles
                //                .Where(a => a.Id == id)
                //                .FirstOrDefault();
                var article = db.Articles.Find(id);

                if (!IsAuthorized(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                // Check if article exist
                if (article == null)
                {
                    return HttpNotFound();
                }
                // Pass to View
                return View(article);
            }
        }

        [Authorize]
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new BlogDbContext())
            {
                //var article = db.Articles
                //                .Where(a => a.Id == id)
                //                .FirstOrDefault();
                var article = db.Articles.Find(id);

                if (!IsAuthorized(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    return HttpNotFound();
                }
                db.Articles.Remove(article);
                db.SaveChanges();

                return RedirectToAction("List");
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (!IsAuthorized(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    AuthorId = article.AuthorId
                };

                return View(articleViewModel);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var article = db.Articles.Find(model.Id);

                    if (!IsAuthorized(article))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }

                    article.Title = model.Title;
                    article.Content = model.Content;

                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        private bool IsAuthorized(Article article)
        {
            var isAdmin = this.User.IsInRole("Admin");
            var isAuthor = article.IsAuthor(this.User.Identity.GetUserId());

            return isAdmin || isAuthor;
        }
    }
}