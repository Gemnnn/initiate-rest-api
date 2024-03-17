using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;
using Initiate.DataAccess.Data;

namespace Initiate.Business.Repositories
{
    public class SharedKeywordRepository : ISharedKeywordRepository
    {
        private readonly ApplicationDbContext _context;

        public SharedKeywordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ShareKeywordAsync(SharedKeywordDTO sharedKeywordDTO)
        {
            var sharedKeyword = new SharedKeyword
            {
                Sender = sharedKeywordDTO.Requestor, 
                Receiver = sharedKeywordDTO.Receiver,
                Keyword = sharedKeywordDTO.Keyword,
                IsAccepted = null 
            };

            _context.SharedKeywords.Add(sharedKeyword);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SharedKeywordDTO>> GetSharedKeywordsAsync(string username)
        {
            return await _context.SharedKeywords
                .Where(sk => sk.Receiver == username && sk.IsAccepted == null)
                .Select(sk => new SharedKeywordDTO
                {
                    Requestor = sk.Sender, 
                    Receiver = sk.Receiver, 
                    Keyword = sk.Keyword,
                    IsAccepted = sk.IsAccepted
                })
                .ToListAsync();
        }

        public async Task AcceptOrRejectKeywordAsync(SharedKeywordDTO sharedKeywordDTO)
        {
            var sharedKeyword = await _context.SharedKeywords
                .FirstOrDefaultAsync(sk =>
                    sk.Receiver == sharedKeywordDTO.Receiver && sk.Keyword == sharedKeywordDTO.Keyword);

            if (sharedKeyword == null)
            {
                throw new InvalidOperationException("Shared keyword not found.");
            }

            if (sharedKeywordDTO.IsAccepted == true)
            {
                var sender = await _context.Users
                    .Include(u => u.News)
                    .Include(u => u.Keywords)
                    .FirstOrDefaultAsync(u => u.UserName == sharedKeyword.Sender);

                var receiver = await _context.Users
                    .Include(u => u.News)
                    .Include(u => u.Keywords)
                    .FirstOrDefaultAsync(u => u.UserName == sharedKeyword.Receiver);

                if (sender == null || receiver == null)
                {
                    throw new InvalidOperationException("Sender or receiver not found.");
                }

                
                var keywordEntity =
                    await _context.Keywords.FirstOrDefaultAsync(k => k.Word == sharedKeywordDTO.Keyword);
                if (!receiver.Keywords.Any(k => k.Word == sharedKeywordDTO.Keyword))
                {
                    var clonedKeyword = new Keyword { Word = sharedKeywordDTO.Keyword };
                    receiver.Keywords.Add(clonedKeyword);
                }

                
                var newsItems = sender.News.Where(n => n.Keyword == sharedKeywordDTO.Keyword).ToList();
                foreach (var news in newsItems)
                {
                    
                    var clonedNews = new News
                    {
                        Title = news.Title,
                        ShortTitle = news.ShortTitle,
                        PublishedDate = news.PublishedDate,
                        Author = news.Author,
                        Source = news.Source,
                        Desciprtion = news.Desciprtion,
                        Content = news.Content,
                        Keyword = news.Keyword,
                        IsLocation = news.IsLocation
                    };
                    
                    receiver.News.Add(clonedNews);
                }
                
                sharedKeyword.IsAccepted = true;
            }
            else
            {
                _context.SharedKeywords.Remove(sharedKeyword);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}