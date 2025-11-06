using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Bắt buộc phải có username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bắt buộc phải có mật khẩu")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
