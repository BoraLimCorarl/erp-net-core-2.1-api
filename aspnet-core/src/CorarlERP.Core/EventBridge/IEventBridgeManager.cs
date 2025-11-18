using Abp.Domain.Services;
using Amazon.EventBridge;
using CorarlERP.EventBridge.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.EventBridge
{
    public interface IEventBridgeManager : IDomainService
    {
        Task CreateEventBridge(CreateEventBridgeInput input);
        Task DeleteEventBridge(DeleteEventBridgeInput input);
     
    }
}
