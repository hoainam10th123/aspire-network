using System.ComponentModel.DataAnnotations;

namespace RoomService.Dtos
{
    public class CreateRoomDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
