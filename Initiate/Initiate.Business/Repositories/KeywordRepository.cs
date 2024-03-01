using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;

namespace Initiate.Business.Repositories
{
    public class KeywordRepository : IKeywordRepository
    {
        private readonly ApplicationDbContext m_db;
        private readonly IMapper m_mapper;

        public KeywordRepository(ApplicationDbContext db, IMapper mapper)
        {
            m_db = db;
            m_mapper = mapper;
        }

        public async Task DeleteKeyword(string username, string keyword)
        {
            var user = await m_db.Users.Include(x=>x.Keywords).FirstOrDefaultAsync(x => x.UserName == username);
            var kw = user.Keywords.FirstOrDefault(x=>x.Word == keyword);
            if (kw != null)
            {
                user.Keywords.Remove(kw);
                await m_db.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<KeywordDTO>> GetAllKeyword(string username)
        {
            var user = await m_db.Users.Include(x=>x.Keywords).FirstOrDefaultAsync(x => x.UserName == username);
            var keywords = user.Keywords.Select(x => m_mapper.Map<KeywordDTO>(x)).ToList();
            return keywords;
        }

        public async Task CreateKeyword(KeywordDTO keywordDTO)
        {
            var user = await m_db.Users.Include(x=>x.Keywords).FirstOrDefaultAsync(x => x.UserName == keywordDTO.Username);
            if (user.Keywords.Any(x => x.Word.ToLower() == keywordDTO.Word.ToLower()))
                throw new Exception("Duplicated keyword found");
            
            user.Keywords.Add(new Keyword(){Word = keywordDTO.Word});
            await m_db.SaveChangesAsync();
        }
    }
}
