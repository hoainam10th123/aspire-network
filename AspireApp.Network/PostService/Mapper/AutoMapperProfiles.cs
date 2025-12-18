using AutoMapper;
using PostService.Dtos;
using PostService.Models;
using SharedObject;

namespace PostService.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Post, PostDto>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<Comment, CommentDto>();
        }
    }
}
