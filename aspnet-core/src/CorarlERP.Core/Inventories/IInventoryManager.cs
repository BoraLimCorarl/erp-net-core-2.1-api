using Abp.Domain.Services;
using CorarlERP.AccountCycles;
using CorarlERP.BatchNos;
using CorarlERP.Inventories.Data;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.Journals;
using CorarlERP.Reports.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Inventories
{
    public interface IInventoryManager: IDomainService
    {
        //IQueryable<InventoryCostItem> GetAvgCostQuery(DateTime toDate, List<long?> Locations, Dictionary<Guid, Guid> updatedItemIssueIds = null);
        
        Task<List<InventoryCostItem>> GetItemsByAverageCost(DateTime toDate, List<long?> Locations, List<long?> lots, 
                                                            Dictionary<Guid, Guid> updatedItemIssueIds = null, 
                                                            Dictionary<Guid, Guid> itemsToSelect = null
                                                            //,List<GetListPropertyFilter> ItemProperties = null,
                                                            //bool groupByLocation = false, long? userId = null
                                                            );

        Task<IQueryable<InventoryCostItem>> GetItemsBalance(string filter,
                                       DateTime toDate,
                                       List<long?> locations,
                                       Dictionary<Guid, Guid> updatedItemIssueIds = null, FilterType filterType = FilterType.Contain);

        IQueryable<ItemBalanceDto> GetInventoryItemBalanceQuery(string filter,
                                    DateTime? fromDate, DateTime? toDate,
                                    List<long?> Locations,
                                    Dictionary<Guid, Guid> updatedItemIssueIds = null, FilterType filterType = FilterType.Contain);

        Task<IQueryable<InventoryCostItem>> GetItemsBalance(
                                    List<Guid> itemIds,
                                     DateTime toDate,
                                     List<long> locations,
                                     List<Guid> exceptIds = null);

        IQueryable<ItemBalanceDto> GetInventoryItemBalanceQuery(
                                    List<Guid> itemIds,
                                    DateTime? fromDate, 
                                    DateTime? toDate,
                                    List<long> Locations,
                                    List<Guid> exceptIds = null);

        IQueryable<ItemBalanceDto> GetItemsBalanceForReport(
           string filter,
           DateTime? fromDate,
           DateTime toDate,
           List<long?> locations,
           List<long?> lots,
           List<Guid?> inventoryAccounts,
           List<Guid> items,
           List<GetListPropertyFilter> itemProperties,
           long? userId = null,
           List<Guid> journalTransactionTypeIds = null,
           List<long> inventoryTypes = null);

        IQueryable<ItemBalanceDto> GetItemBalanceForReportQuery(
           string filter,
           DateTime? fromDate,
           DateTime? toDate,
           List<long?> locations,
           List<long?> lots,
           List<Guid?> inventoryAccounts,
           List<Guid> items,
           List<GetListPropertyFilter> itemProperties,
           List<Guid> journalTransactionTypeIds = null,
           List<long> inventoryTypes = null);


        IQueryable<ItemDto> GetInventoryDetailQuery(
                                DateTime? fromDate, 
                                DateTime? toDate,
                                List<long?> Locations, 
                                Dictionary<Guid, Guid> updatedItemIssueIds = null, 
                                Dictionary<Guid, Guid> itemsToSelect = null
                                //,List<GetListPropertyFilter> ItemProperties = null,
                                //List<Guid?> accounts = null,
                                //long? filterByUserId = null
            );
        
        //IQueryable<ItemDto> GetInventoryDetailQuery(DateTime toDate,
        //                        List<long?> Locations, 
        //                        List<GetListPropertyFilter> ItemProperties = null,
        //                        List<Guid> items = null,
        //                        List<Guid?> accounts = null,
        //                        long? filterByUserId = null);

        IQueryable<ItemTransactionDto> GetInventoryTransactionReportQuery(
                                    DateTime fromDate,
                                    DateTime toDate,
                                    List<long> locations,
                                    List<long> lots,
                                    List<Guid> items,
                                    List<JournalType> journalTypes,
                                    List<long?> users = null,
                                    long? filterByUserId = null,
                                    bool includeBeginning = false,
                                    List<Guid> journalTransactionTypeIds = null,
                                    List<long> inventoryTypes= null);
        //IQueryable<ItemTransactionDto> GetInventoryTransactionBeginningReportQuery(
        //                            DateTime fromDate,
        //                            //DateTime toDate,
        //                            List<long> locations,
        //                            List<long> lots,
        //                            List<Guid> items,
        //                            List<JournalType> journalTypes,
        //                            List<long?> users = null,
        //                            long? filterByUserId = null);

        IQueryable<ItemDto> GetInventoryTransactionDetailQuery(
                                DateTime fromDate, DateTime toDate,
                                List<long> locations,
                                List<Guid> items,
                                List<JournalType> journalTypes,
                                List<Guid> inventoryAccount,
                                List<GetListPropertyFilter> ItemProperties = null,
                                List<long?> Users = null,
                                long? filterByUserId = null);

        Task<GetAvgCostForIssueOutput> GetAvgCostForIssues(DateTime toDate, List<long?> locationIds, 
                                                                        List<GetAvgCostForIssueData> input
                                                                        //,Journal goodIssueJournalEntity
                                                                        //,Guid? roundingAccountId
            );

        List<InventoryValuationDetail> CalculateManualInventoryValuationDetail(List<AccountCycle> accountCyles,
                                                                               List<ItemDto> inventoryItemDetail,
                                                                               //Dictionary<Guid, List<RoundingAdjustmentItemOutput>> roundingItemsOutput = null,
                                                                               Dictionary<Guid, Guid> journalToRecalculateCost = null,
                                                                               bool groupByLocation = false,
                                                                               Dictionary<string, ItemCostSummaryDto> itemLatestCosts = null);

        List<InventoryTransactionItemDto> CalculateInventoryByItems(List<InventoryTransactionItem> beginningList,
                                                                    List<InventoryTransactionItem> inventoryItemDetail,
                                                                    Dictionary<string, ItemCostSummaryDto> itemLatestCosts = null,
                                                                    int roundDigit = 2,
                                                                    int roundDigitUnitCost = 2,
                                                                    bool groupByLocation = false);


        Task<IdentityResult> AutoCreateRoundingJournal(Journal goodIssueJournalEntity,
                                                                    List<RoundingAdjustmentItemOutput> roundingItems,
                                                                    Guid? roundingAccountId,
                                                                    bool remove = true,
                                                                    bool add = true);

        IQueryable<ItemCostSummaryDto> GetItemCostSummaryByLocationQuery(
            DateTime? fromDate,
            DateTime toDate,
            List<long?> locations,
            long filterUserId,
            List<Guid> items = null,
            List<Guid> exceptIds = null);

        IQueryable<ItemCostSummaryDto> GetItemCostSummaryQuery(
            DateTime? fromDate,
            DateTime toDate,
            List<long?> locations,
            long filterUserId,
            List<Guid> items = null,
            List<Guid> exceptIds = null);

        IQueryable<ItemCostSummaryDto> GetItemCostSummaryNewPeriodQuery(
           DateTime? fromDate,
           DateTime? toDate,
           List<long?> locations,
           List<Guid> items = null,
           List<Guid> exceptIds = null);

        IQueryable<ItemDto> GetInventoryValuationDetailQuery(
                                    DateTime? fromDate,
                                    DateTime? toDate,
                                    List<long?> Locations,
                                    List<Guid> itemsToSelect = null,
                                    long? filterByUserId = null);

        IQueryable<ItemDto> GetInventoryValuationDetailQuery(
                                DateTime toDate,
                                List<long?> Locations,
                                List<Guid> items = null,
                                long? filterByUserId = null);

        Task<List<ItemDto>> GetInventoryValuationDetailList(
                                    DateTime? fromDate,
                                    DateTime? toDate,
                                    List<long?> Locations,
                                    List<Guid> itemsToSelect = null,
                                    long? filterByUserId = null);

        Task<List<ItemDto>> GetInventoryValuationDetailList(
                                DateTime toDate,
                                List<long?> Locations,
                                List<Guid> items = null,
                                long? filterByUserId = null);


        IQueryable<ItemCostSummaryDto> GetItemLatestCostSummaryQuery(
            DateTime toDate,
            List<long?> locations,
            List<Guid> items = null,
            List<Guid> exceptIds = null);


        IQueryable<ItemBalanceDto> GetAssetBalanceForReport(
           string filter,
           DateTime? fromDate,
           DateTime toDate,
           List<long?> locations,
           List<long?> lots,
           List<Guid> items,
           List<GetListPropertyFilter> itemProperties,
            long? userId = null);

        IQueryable<ItemBalanceDto> GetAssetBalanceForReportQuery(
           string filter,
           DateTime? fromDate,
           DateTime? toDate,
           List<long?> locations,
           List<long?> lots,
           List<Guid> items,
           List<GetListPropertyFilter> itemProperties);



        Task RecalculationCostHelper(int tenantId, DateTime? inputFromDate, DateTime inputToDate, List<Guid> inputItems);

        Task<List<BatchNoItemBalanceOutput>> GetItemBatchNoBalance(List<Guid> items, List<long> lots, List<Guid> batchNos, DateTime date, List<Guid> exceptIds);
        Task<List<BatchNoItemBalanceOutput>> GetItemBatchNoBalance(DateTime date, long locationId, List<Guid> items,  List<Guid> exceptIds);
    }
}
