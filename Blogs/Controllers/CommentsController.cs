﻿using System;
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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View(db.Comments.ToList());
        }

        /// <summary>
        /// Will pull up the direct details of any comment, though cannot be accessed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
         public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

            /// <summary>
            /// Assigns the passed postID to the comment model PostID
            /// </summary>
            /// <param name="postID"></param>
            /// <returns></returns>
        [HttpGet]
        public ActionResult Create(int postID)
        {
            return View(new Comment { PostID = postID });
        }

        /// <summary>
        /// Autofills the fields needed to leave a comment (date posted and last edited with the current time, username with logged in user etc.)
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (User.Identity.IsAuthenticated && !User.IsInRole("Suspended"))
            {
                comment.UserID = User.Identity.Name;
                comment.DatePosted = DateTime.Now;
                comment.DateEdited = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Posts", new { @id = comment.PostID });
        }


    // GET: Comments/Edit/5
    public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        /// <summary>
        /// Checks if the user is suspended, and if not: will allow them to edit any comments they have created.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,PostID,DatePosted,DateEdited,Body,UserID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                //checks to see if the user is suspended before editing
                if (User.Identity.Name == comment.UserID && !User.IsInRole("Suspended"))
                {
                    db.Entry(comment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("../Posts/Details/" + comment.PostID);
                }
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        /// <summary>
        /// Checks if either the user that left the comment is the person logged in, or if the person logged in has administration rights then if so: deletes the comment.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            //checks to see if the posted comment is made by a moderator or the same user thar is logged in
            if (User.IsInRole("Moderator") || User.Identity.Name == comment.UserID)
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("../Posts/Details/" + comment.PostID);
            }
            return RedirectToAction("../Posts/Details/" + comment.PostID);
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
