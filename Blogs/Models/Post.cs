using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogs.Models
{
    /// <summary>
    /// This is the model for a Post on the Blog.
    /// </summary>
    public class Post
    {

        [Key]
        public int PostID { get; set; }

        public DateTime? DatePosted { get; set; }

        public DateTime? DateEdited { get; set; } = DateTime.Now;

        [Required]
        public String Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Body { get; set; }

        
        public String UserID { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }
}