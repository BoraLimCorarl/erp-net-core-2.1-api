using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using CorarlERP.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Editions
{
    public class EditionManager : AbpEditionManager
    {
        public const string DefaultEditionName = "Standard";
        public const string SimpleEditName = "Simple Accounting";
        public const string AdvanceEditName = "Advance Accounting";

        public EditionManager(
            IRepository<Edition> editionRepository,
            IAbpZeroFeatureValueStore featureValueStore)
            : base(
                editionRepository,
                featureValueStore
            )
        {
        }

        public async Task<List<Edition>> GetAllAsync()
        {
            return await EditionRepository.GetAllListAsync();
        }

        public async Task<decimal> GetPackPrice(int editionId, DurationType durationType)
        {
            var edition =(SubscribableEdition) await EditionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == editionId);
            if (edition != null)
            {
                switch (durationType)
                {
                    case DurationType.Month:
                        return edition.MonthlyPrice ?? 0;
                    case DurationType.Year:
                        return edition.AnnualPrice ?? 0;
                    default:
                        return edition.AnnualPrice ?? 0;
                }
            }

            return 0;
        }
    }
}
