using AutoMapper;
using EventBus;
using EventBus.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;
using PostService.Service;
using SharedObject;
using System.Security.Claims;

namespace PostService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PostsController(DataContext context, 
        IBusRabbitmqService bus,
        IKeycloakService keycloakService,
        IMapper mapper) : ControllerBase
    {
        const long MaxFileSize = 3 * 1024L * 1024L; // 3mb

        [HttpGet]
        public async Task<IActionResult> GetPosts(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { msg = "notfound userid" });
            }           

            var listPost = await context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Photos)
                .ToListAsync();

            if (listPost.Count == 0)
            {
                return Ok(new Pagination<PostDto>(pageNumber, pageSize, 0, new List<PostDto>()));
            }

            var postDtos = mapper.Map<List<PostDto>>(listPost);            

            var listPostId = listPost.Select(x => x.Id).ToList();

            var listUserId = listPost.Select(x => x.UserId).Distinct();

            var tasks = listUserId.Select(keycloakService.GetUserByUserIdAsync);            

            // lay bai post (chi lay ID de xu ly) like boi user dang dang nhap
            var listPostByMe = await context.PostLikes
            .Where(x => listPostId.Contains(x.PostId) && x.UserId == userId)
            .Select(x => x.PostId)
            .ToListAsync();

            var setPostByMe = listPostByMe.ToHashSet(); // O(1)

            var users = await Task.WhenAll(tasks);

            var dictUsers = users.ToDictionary(u => u.Id, u => u);

            foreach (var post in postDtos)
            {
                post.LikedByMe = setPostByMe.Contains(post.Id);
                post.User = dictUsers.GetValueOrDefault(post.UserId);
            }

            var totalPosts = await context.Posts.CountAsync();

            return Ok(new Pagination<PostDto>(pageNumber, pageSize, totalPosts, postDtos));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsPostOwner")]
        public async Task<IActionResult> UpdatePost(string id, CreatePostDto dto)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new {msg = "not found post"});
            }
            post.Title = dto.Title;
            post.Content = dto.Content;

            await context.SaveChangesAsync();

            return Ok(post);
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> AddPost(IFormFile? file)
        {
            var arr = new string[] { "image/jpeg", "image/png" };
            if (file != null && !arr.Contains(file.ContentType))
                return BadRequest(new { msg = "Chỉ chấp nhận JPG hoặc PNG" });

            var form = await Request.ReadFormAsync();
            var json = form["json"];
            var dto = System.Text.Json.JsonSerializer.Deserialize<CreatePostDto>(json);
            if (dto == null)
            {
                return BadRequest(new { msg = "Invalid post data Request.Form" });
            }

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userid,
                CreatedAt = DateTime.UtcNow
            };

            //The configured execution strategy 'NpgsqlRetryingExecutionStrategy' does not support user-initiated transactions
            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(
            async () =>
            {
                using var tx = await context.Database.BeginTransactionAsync();                
                try
                {
                    context.Posts.Add(post);

                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                        string UploadPath = Path.Combine(UploadFolder, fileName);

                        using (var temp = new FileStream(UploadPath, FileMode.Create))
                        {
                            await file.CopyToAsync(temp);
                        }

                        var photo = new Photo
                        {
                            Url = $"/UploadedFiles/{fileName}",
                            PostId = post.Id
                        };
                        context.Photos.Add(photo);
                    }

                    await context.SaveChangesAsync();
                    // pub to search service
                    await bus.PublishAsync<CreatePost>(Contanst.CreatePostQueue, 
                        new CreatePost { Id = post.Id, 
                            Title = post.Title, 
                            Content = post.Content, 
                            UserId = userid, 
                            CreatedAt = post.CreatedAt, 
                            LastUpdatedAt = post.LastUpdatedAt });

                    await tx.CommitAsync();                    
                }
                catch (Exception e)
                {
                    await tx.RollbackAsync();
                    Console.WriteLine(e);
                    throw;
                }
            });

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(string id, int pageNumber = 1, int pageSize = 10)
        {
            var post = await context.Posts.Include(x=>x.Photos)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(new { msg = "Post not found" });
            }

            var comments = await context.Comments.Where(c => c.PostId == id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x=>x.CreatedAt)
                .ToListAsync();

            var listUserId = comments.Select(c => c.UserId).Distinct().ToList();

            var tasks = listUserId.Select(keycloakService.GetUserByUserIdAsync);

            var users = await Task.WhenAll(tasks);

            var usersDic = users.ToDictionary(u=>u.Id, u => u);

            var klUser = await keycloakService.GetUserByUserIdAsync(post.UserId);

            var dataPost = mapper.Map<PostDto>(post);
            var listCommentDto = mapper.Map<List<CommentDto>>(comments);

            foreach (var comment in listCommentDto) 
            {
                comment.User = usersDic.GetValueOrDefault(comment.UserId);
            }

            dataPost.Comments = listCommentDto;
            dataPost.User = klUser;

            return Ok(dataPost);
        }

        [Authorize(Policy = "IsPostOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { msg = "Post not found" });
            }
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
            return Ok(new { PostId = id });
        }

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> CreateComment(string postId, CreateComment dto)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            context.Comments.Add(comment);

            await context.SaveChangesAsync();

            var cmtDto = mapper.Map<CommentDto>(comment);
            cmtDto.User = await keycloakService.GetUserByUserIdAsync(comment.UserId);

            await bus.PublishAsync<CommentDto>(Contanst.RealtimeCommentQueue, cmtDto);

            return CreatedAtAction(nameof(Detail), 
                new { postId, id = comment.Id },
                comment
            );
        }

        [HttpGet("{postId}/comments/detail/{id}")]
        public async Task<IActionResult> Detail(string postId, string id)
        {
            var comment = await context.Comments
                .SingleOrDefaultAsync(c => c.Id == id && c.PostId == postId);

            if (comment == null) NotFound(new { msg = "not found comment" });

            return Ok(comment);
        }

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(string postId, int pageNumber = 1, int pageSize = 10)
        {
            var comments = await context.Comments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Where(c => c.PostId == postId)
                .ToListAsync();

            var totalComments = await context.Comments
                .Where(c => c.PostId == postId)
                .CountAsync();

            return Ok(new Pagination<Comment>(pageNumber, pageSize, totalComments, comments));
        }

        [HttpPut("like/{postId}")]
        public async Task<IActionResult> LikePost(string postId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userid))
            {
                return NotFound(new { msg = "not found userid" });
            }

            var post = await context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound(new { msg = "Post not found" });
            }            

            var existingLike = await context.PostLikes.FindAsync(postId, userid);

            if (existingLike != null)
            {
                post.TotalLike -= 1;
                context.PostLikes.Remove(existingLike);
                await context.SaveChangesAsync();
                return Ok(new { msg = "Success remove", isLike = false, userId = userid  });
            }
            else
            {
                post.TotalLike += 1;
                await context.PostLikes.AddAsync(new PostLike { PostId = postId, UserId = userid});
                await context.SaveChangesAsync();
                return Ok(new { msg = "Success add", isLike = true, userId = userid });
            }            
        }
    }
}
