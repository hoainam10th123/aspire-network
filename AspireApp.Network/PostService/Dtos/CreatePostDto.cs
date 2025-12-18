using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos
{
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
