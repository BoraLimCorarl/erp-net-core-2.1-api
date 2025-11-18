using Abp.AutoMapper;
using CorarlERP.Dto;
using CorarlERP.InventoryTransactionTypes;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JournalTransactionTypes.Dto
{
  public  class GetJournalTransactionTypeOutput
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsIssue { get; set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
        public InventoryTransactionType InventoryTransactionType { get; set; }
        public string InventoryTransactionTypeName { get; set; }
    }

    [AutoMapFrom(typeof(JournalTransactionType))]
    public class GetJournalTransactionTypeDetail
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsIssue { get; set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
        public InventoryTransactionType InventoryTransactionType { get; set; }    
        public string InventoryTransactionTypeName
        {
            get { return InventoryTransactionType.ToString();}
            set { InventoryTransactionTypeName = InventoryTransactionType.ToString(); }
        }
    }

    public class GetListTypeForJournalTransactionOutput { 
    
        public string Id { get; set; }
        public string Name { get; set; }
    }



    public class GetListTypeForJournalTransactionInput : PagedSortedAndFilteredInputDto { 
    
      
    }
    public class GetListInentoryTypeOutput
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }

    public  class GetInventoryTypeNameInput
    {
        public InventoryTransactionType InventoryTransactionType { get; set; }
    }
  


}
