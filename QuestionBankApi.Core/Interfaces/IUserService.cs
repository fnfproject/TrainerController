using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestionBankApi.Core.Models;

namespace QuestionBankApi.Core.Interfaces
{
    public interface IUserService
    {
        User GetUserByUsername(string username);

        void AddUser(User user);
    }
}
