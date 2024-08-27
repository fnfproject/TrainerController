using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionBankApi.Trainer.Dtos
{
    public class VerifyAdminCodeDto
    {
        public string UserName { get; set; }

        public string Code { get; set; }
    }
}
