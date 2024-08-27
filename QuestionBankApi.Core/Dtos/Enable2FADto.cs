using System.ComponentModel.DataAnnotations;

namespace QuestionBankApi.Core.Dtos
{
    public class Enable2FADto
    {
        [Required]
        public string Username { get; set; }
    }
}
