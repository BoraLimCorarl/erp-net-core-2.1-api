using System;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.LockTransactions.Dto
{
    public class GetListLockInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "LockDate";
            }
        }
    }

    public class GetListFindLockInput : GetListLockInput {

        public   List<TransactionLockType> TransactionLockType { get; set; }
    }

}
