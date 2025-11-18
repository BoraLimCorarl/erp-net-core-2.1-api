using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Galleries
{
    public enum UploadFrom
    {
        FromTemplate = 1,
        FromItem = 2,
        Logo = 3,
        Profile = 4,
    }

    public enum UploadSource
    {
        Local = 1,
        AWS = 2
    }
}
