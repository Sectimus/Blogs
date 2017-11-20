using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogs.Models
{
    /// <summary>
    /// This is the model that is used for a comment on a post.
    /// </summary>
    public class Comment
    {
        [Required]
        [Key]
        public int CommentID { get; set; }

     
        public int PostID { get; set; }

        [Required]
        public DateTime DatePosted { get; set; } = DateTime.Now;

        public DateTime DateEdited { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Body { get; set; }

        [Required]
        public String UserID { get; set; }
    }
}