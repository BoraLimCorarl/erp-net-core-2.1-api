using System.ComponentModel.DataAnnotations;

namespace CorarlERP.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}