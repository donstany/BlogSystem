using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    // Major idea is to hide property which is not interesting from current view - html
    public class ArticleViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }
    }
}