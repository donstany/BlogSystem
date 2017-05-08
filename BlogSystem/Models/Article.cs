using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogSystem.Models
{
    public class Article
    {
        public Article()
        {
            this.CreatedOn = DateTime.UtcNow;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

   
        public string AuthorId { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ApplicationUser Author { get; set; }
        
        public  bool IsAuthor(string authorId)
        {
            return this.AuthorId == authorId;
        }

    }

}