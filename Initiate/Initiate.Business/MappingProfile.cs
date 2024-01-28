using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;

namespace Initiate.Business
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<News, NewsDTO>().ReverseMap();
        }
    }
}
