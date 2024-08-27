﻿using System.ComponentModel.DataAnnotations;

namespace BIZNEWS_FREE.Models;
  


    //Solid
    //Single Responsibility

    public class Article
    {
        internal List<Tag> Tags;

        //[Key]
        public int Id { get; set; }
        [Required]      //boş göndərmək olmaz
        [MinLength(5)]
        [MaxLength(50)]
        public string Title { get; set; }
        public string Content { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int ViewCount { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeature { get; set; }
        public bool IsDeleted { get; set; }
        public string SeoUrl { get; set; } //sayti axtarisda qabağa cekmek

    }
