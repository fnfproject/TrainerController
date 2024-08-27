using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestionBankApi.Core.Interfaces;
using QuestionBankApi.Core.Models;

namespace QuestionBankApi.Admin.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();


        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public User GetUserByUsername(string username)
        {
            return _users.SingleOrDefault(u => u.Username == username);
        }
    }
}
