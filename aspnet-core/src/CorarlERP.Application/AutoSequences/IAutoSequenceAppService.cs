using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AutoSequences
{
    public interface IAutoSequenceAppService
    {
        Task Sync();
    }
}
