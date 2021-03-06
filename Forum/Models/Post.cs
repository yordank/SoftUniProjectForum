﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace Forum.Models
{
    public class Post
    {
        private ICollection<Tag> tags;

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        public Post()
        {
            this.tags = new HashSet<Tag>();
            this.Views = 0;
        }

        public Post(string AuthorId, string title, string content, int categoryId,int? ParentPostId)
        {
            this.AuthorId = AuthorId;
            this.Title = title;
            this.Content = content;
            this.CategoryId = categoryId;
            this.ParentPostId = ParentPostId;
            this.tags = new HashSet<Tag>();
            this.Views = 0;
        }

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

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public bool isAuthor(string name)
        {
            if (AuthorId == null) return false;
            return this.Author.UserName.Equals(name);
        }

        [DefaultValue(0)]
        public int? Views { get; set; }

    }
}