using Initiate.Model;

namespace Initiate.Business.Repositories;

public interface ISharedKeywordRepository
{
    Task ShareKeywordAsync(SharedKeywordDTO sharedKeywordDTO);
    Task<IEnumerable<SharedKeywordDTO>> GetSharedKeywordsAsync(string username);
    Task AcceptOrRejectKeywordAsync(SharedKeywordDTO sharedKeywordDTO);
}