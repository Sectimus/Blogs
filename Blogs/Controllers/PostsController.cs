using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blogs.Models;

namespace Blogs.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            return View(db.Posts.Include("Comments").ToList());
        }

        /// <summary>
        /// Post details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Where(p=>p.PostID ==id).Include("Comments").FirstOrDefault();
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Checks if the user is not suspended and also if the model state is valid then tests if the user is logged in and creates a post if all checks pass.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Body")] Post post)
        {
            post.UserID = User.Identity.Name;
            post.DatePosted = DateTime.Now;
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid && !User.IsInRole("Suspended"))
            {
                //Testing if the user is logged in
                if (User.Identity.IsAuthenticated)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Details/" + post.PostID);
                }
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        /// <summary>
        /// Checks if the user is not suspended and the modelstate is valid then allows the edit to go through.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Post post)
        {
            post.DateEdited = DateTime.Now;
            if (ModelState.IsValid && !User.IsInRole("Suspended"))
            {
                
                var dbPost = db.Posts.FirstOrDefault(p => p.PostID == post.PostID);
                if (dbPost == null)
                {
                    return HttpNotFound();
                }
                if (User.Identity.Name == post.UserID)
                {
                    dbPost.Body = post.Body;
                    dbPost.Title = post.Title;
                    dbPost.DateEdited = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Details/" + post.PostID);
                }
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        /// <summary>
        /// Checks if the user is a moderator or that the user was the original poster and allows the delete to go through if true.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            if (User.IsInRole("Moderator") || User.Identity.Name == post.UserID)
            {
                db.Posts.Remove(post);

                db.SaveChanges();
                return RedirectToAction("../");
            }
            return RedirectToAction("../");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
