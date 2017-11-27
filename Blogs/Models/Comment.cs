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

        [Key]
        public int CommentID { get; set; }

        public int PostID { get; set; }

        public DateTime? DatePosted { get; set; }

        public DateTime? DateEdited { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.MultilineText)]
        public String Body { get; set; }

        public String UserID { get; set; }
    }
}