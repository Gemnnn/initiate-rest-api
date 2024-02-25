using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task DeleteKeyword(string keyword)
        {
            // Retrieve the keyword entity to delete.
            var keywordEntity = await m_db.Keywords.FirstOrDefaultAsync(k => k.Word == keyword);
            if (keywordEntity == null)
            {
                throw new Exception("No keyword found");
            }

            // Remove all associated NewsKeyword entries.
            var newsKeywords = m_db.NewsKeywords.Where(nk => nk.KeywordId == keywordEntity.KeywordId);
            m_db.NewsKeywords.RemoveRange(newsKeywords);

            // Remove the keyword entity itself.
            m_db.Keywords.Remove(keywordEntity);

            // Save changes to the database.
            await m_db.SaveChangesAsync();
        }


        public async Task<IEnumerable<KeywordDTO>> GetAllKeyword(string username)
        {
            var keywords = await m_db.Keywords.Where(x => x.UserKeywords.Any(y => y.User.UserName == username)).Select(x => m_mapper.Map<KeywordDTO>(x)).ToListAsync();

            if (keywords == null)
                throw new Exception("No Keywords Found");

            return keywords;
        }

        public async Task CreateKeyword(KeywordDTO keywordDTO)
        {

            var duplicatedKeyword = await m_db.Keywords.FirstOrDefaultAsync(x => x.Word == keywordDTO.Word);
            if (duplicatedKeyword != null)
                throw new Exception("Duplicated Keyword");

            var user = await m_db.Users.FirstOrDefaultAsync(x => x.UserName == keywordDTO.Username);
            if (user == null)
                throw new Exception("Wrong User Request");

            Keyword keyword = m_mapper.Map<Keyword>(keywordDTO);

            // Add the new keyword to the context.
            await m_db.Keywords.AddAsync(keyword);

            // Save changes to add the Keyword to the database.
            await m_db.SaveChangesAsync();

            // Assuming UserKeyword is the join entity between User and Keyword.
            var userKeyword = new UserKeyword
            {
                KeywordId = keyword.KeywordId,
                UserId = user.Id // Ensure this is the primary key of the User entity.
            };

            // Add the new UserKeyword to the context.
            await m_db.UserKeywords.AddAsync(userKeyword);

            // Save changes to add the UserKeyword to the database.
            await m_db.SaveChangesAsync();


        }
    }
}
