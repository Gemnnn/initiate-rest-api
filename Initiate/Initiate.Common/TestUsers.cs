using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Initiate.Common
{
    public class TestUsers
    {
        /// <summary>
        /// Key: UserName
        /// Value: Password
        /// </summary>
        public static Dictionary<string, string> Users = new Dictionary<string, string>
        {
            { "test@gmail.com", "!1Qtesttest" },
            { "test@test.com", "Test123!" }
        };
    }
}
