namespace Blogs.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blogs.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
        /// <summary>
        /// add initial data to app: roles, users, data
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // create a default user   

            var user = new ApplicationUser();
            user.UserName = "test@test.com";
            user.Email = "test@test.com";

            string userPWD = "Password123!";

            UserManager.Create(user, userPWD);


            // create an admin role 

            if (!roleManager.RoleExists("Moderator"))
            {

                // first we create Admin role  
                var role = new IdentityRole();
                role.Name = "Moderator";
                roleManager.Create(role);
            }


            //Here we create a Admin  user who will maintain the website                  

            var adminuser = new ApplicationUser();
            adminuser.UserName = "god@god.com";
            adminuser.Email = "god@god.com";

            userPWD = "Password123!";

            var chkUser = UserManager.Create(adminuser, userPWD);

            //Add admin User to Role Admin   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(adminuser.Id, "Moderator");

            }

            // creating a suspended role for suspended users.  
            if (!roleManager.RoleExists("Suspended"))
            {
                var role = new IdentityRole();
                role.Name = "Suspended";
                roleManager.Create(role);

            }

            //create a post
            var testpost = new Post();

            testpost.PostID = 1;
            testpost.DatePosted = DateTime.Parse("1970-01-01 00:00:00");
            testpost.DateEdited = DateTime.Now;
            testpost.Title = "I am alive!";
            testpost.UserID = "god@god.com";
            testpost.Body = "Hello World!";

            context.Posts.AddOrUpdate(testpost);



            //create related comments

            var testcomment = new Comment();
            testcomment.CommentID = 1;
            testcomment.PostID = 1;
            testcomment.Body = ("I am comment #" + 1);
            testcomment.UserID = user.Id;
            testcomment.DatePosted = DateTime.Parse("1970-01-01 00:00:00");
            testcomment.DateEdited = DateTime.Now;

            context.Comments.AddOrUpdate(testcomment);

            //COMMENT 1 [I am comment #1] [UserID] [1970-01-01 00:00:00]
            //COMMENT 2 [I am comment #2] [UserID] [1970-01-01 00:00:00] [Edit:NOW]
            context.SaveChanges();

        }
    }
}
