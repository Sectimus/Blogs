using Blogs.Models;
using Blogs.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blogs.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class ManageUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ManageUsers
        public ActionResult Index()
        {
            List<UserViewModel> theUsers = new List<UserViewModel>();
            foreach (ApplicationUser aUser in db.Users)
            {
                UserViewModel uViewModel = new UserViewModel();
                uViewModel.Id = aUser.Id;
                uViewModel.Email = aUser.Email;
                uViewModel.IsAdmin = aUser.IsAdmin;
                uViewModel.IsSuspended = aUser.IsSuspended;
                theUsers.Add(uViewModel);
            }
            return View(theUsers);
        }

        /// <summary>
        /// view details of users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            ApplicationUser aUser = db.Users.Single(m => m.Id == id);
            UserViewModel uvm = new UserViewModel();
            uvm.UserName = aUser.UserName;
            uvm.Email = aUser.Email;
            uvm.IsAdmin = aUser.IsAdmin;
            uvm.IsSuspended = aUser.IsSuspended;
            return View(uvm);
        }

        // GET: ManageUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManageUsers/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ManageUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserViewModel uvm = new UserViewModel();
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                uvm.UserName = applicationUser.UserName;
                uvm.Email = applicationUser.Email;
                uvm.IsAdmin = applicationUser.IsAdmin;
                uvm.IsSuspended = applicationUser.IsSuspended;
            }
            return View(uvm);
        }

        /// <summary>
        /// Checks if the user exists via a userviewmodel then will change attributes on their profile to match the new input, with direct access to suspend and/or promote users to administrator.
        /// </summary>
        /// <param name="userViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel userViewModel)
        {
            //get app user
            ApplicationUser au = db.Users.Find(userViewModel.Id);
            au.Id = userViewModel.Id;
            au.UserName = userViewModel.UserName;
            au.Email = userViewModel.Email;
            au.IsAdmin = userViewModel.IsAdmin;
            au.IsSuspended = userViewModel.IsSuspended;

            if(ModelState.IsValid)
            {
                db.Entry(au).State = System.Data.Entity.EntityState.Modified;

                //declaring usermanager
                var UserManager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(db));

                //role editing
                if ((au.IsAdmin) && (!UserManager.IsInRole(au.Id, "Moderator")))
                {
                    UserManager.AddToRole(au.Id, "Moderator");
                }
                else if ((!au.IsAdmin) && (UserManager.IsInRole(au.Id, "Moderator")))
                {
                    UserManager.RemoveFromRoles(au.Id, "Moderator");
                }
                if ((au.IsSuspended) && (!UserManager.IsInRole(au.Id, "Suspended")))
                {
                    UserManager.AddToRole(au.Id, "Suspended");
                }
                else if ((!au.IsSuspended) && (UserManager.IsInRole(au.Id, "Suspended")))
                {
                    UserManager.RemoveFromRoles(au.Id, "Suspended");
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userViewModel);
        }

        /// <summary>
        /// Will check if the user profile to be deleted matches an existing user id then continues if it matches, and aborts the delete if no such user exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(string id)
        {
            UserViewModel uvm = new UserViewModel();
            ApplicationUser applicationUser = db.Users.Single(m => m.Id == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                uvm.Id = applicationUser.Id;
                uvm.UserName = applicationUser.UserName;
                uvm.Email = applicationUser.Email;
                uvm.IsAdmin = applicationUser.IsAdmin;
                uvm.IsSuspended = applicationUser.IsSuspended;
            }
            return View(uvm);
        }

        // POST: ManageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
               ApplicationUser applicatonUser = db.Users.Single(m => m.Id == id);
                db.Users.Remove(applicatonUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
