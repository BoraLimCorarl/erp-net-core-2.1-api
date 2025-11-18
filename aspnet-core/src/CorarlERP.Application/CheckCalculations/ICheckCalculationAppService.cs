using Abp.Application.Services;
using CorarlERP.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CheckCalculations
{
    public interface ICheckCalculationAppService: IApplicationService
    {
        Task<bool> CheckAndCalculate(CheckCalculationInput input);
    }
}
