using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    public  class UpdateBomInput : CreateBomInput
    {
        public Guid Id { get; set; }
    }
}
