using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos
{
    public class CreateComment
    {
        [Required]
        public string Content { get; set; } = null!;
    }
}
