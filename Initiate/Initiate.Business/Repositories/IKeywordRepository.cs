using Initiate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business.Repositories
{
    public interface IKeywordRepository
    {
        public Task CreateKeyword(KeywordDTO keywordDTO);
        public Task DeleteKeyword(string username, string keyword);
        public Task<IEnumerable<KeywordDTO>> GetAllKeyword(string username);
    }
}
