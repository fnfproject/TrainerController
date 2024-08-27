using System;
using System.Collections.Generic;
using System.Linq;
using QuestionBankApi.Core.Dtos;
using QuestionBankApi.Core.Models;
using System.Text;
using System.Threading.Tasks;

namespace QuestionBankApi.Trainer.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(RegisterDto model, UserRole role);
        User AuthenticateUser(string username, string password);
        (byte[] QrCode, string SecretKey) EnableTwoFactorAuth(string username);
        bool VerifyTwoFactorCode(string username, string code);

        User MarkUserAsVerified(string username);
    }
}
