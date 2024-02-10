using Initiate.DataAccess;
using Initiate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business
{
    public interface IUserRepository
    {
        Task<bool> RegisterUser(UserRegistrationDTO userDto);
        Task<User> LoginUser(string email, string password);
    }
}