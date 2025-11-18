using System.ComponentModel.DataAnnotations;

namespace CorarlERP.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
