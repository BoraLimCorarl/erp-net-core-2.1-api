using CorarlERP.Promotions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
   public interface IPromotionManager
    {
        Task<GetPromotionOutput> GetDefaultPromotion(GetPromotionInput input);
      
    }
}
