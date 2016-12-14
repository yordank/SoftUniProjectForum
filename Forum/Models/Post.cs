using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace Forum.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }


        public int? ParentPostId { get; set; }
        [ForeignKey("ParentPostId")]
        public virtual Post Parent { get; set; }


       



    }
}