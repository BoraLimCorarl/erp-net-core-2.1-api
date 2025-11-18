using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
   public class UpdateBomItemInput : CreateBomItemInput
    {
        public Guid Id { get; set; }
    }
}
