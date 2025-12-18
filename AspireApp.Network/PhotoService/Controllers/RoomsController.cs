using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomService.Dtos;
using RoomService.Services;

namespace RoomService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly Data.DataContext _context;
        private readonly ILivekitService _livekit;

        public RoomsController(Data.DataContext context, ILivekitService livekit)
        {
            _context = context;
            _livekit = livekit;
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(string id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { msg = "Room not found" });
            }
            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            var room = new Models.Room
            {
                Title = dto.Title,
                Description = dto.Description
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        [HttpGet("livekit")]
        public async Task<IActionResult> GetLivekitToken(string roomId)
        {
            var token = await _livekit.GetToken(roomId);

            return Ok(new { lkToken = token });
        }
    }
}
