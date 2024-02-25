using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.Business
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<News, NewsDTO>().ReverseMap();
            CreateMap<Keyword, KeywordDTO>().ReverseMap();
        }
    
    }

}