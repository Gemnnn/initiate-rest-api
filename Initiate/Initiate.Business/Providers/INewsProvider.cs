using Initiate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business
{
    public interface INewsProvider
    {
        Task<NewsDTO> GetNews();
    }
}
