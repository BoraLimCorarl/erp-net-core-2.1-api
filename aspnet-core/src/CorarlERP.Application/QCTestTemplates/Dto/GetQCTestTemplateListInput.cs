using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.QCTestTemplates.Dto
{
   public class GetQCTestTemplateListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
