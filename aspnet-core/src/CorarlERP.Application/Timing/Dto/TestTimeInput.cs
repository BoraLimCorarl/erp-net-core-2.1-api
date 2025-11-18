using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Timing.Dto
{
    public class TestTimeInput
    {
        public DateTime DateTime { get; set; }
        public DateTime ConvertedDateTime { get; set; }
    }

    public class TestTimeOutput
    {
        public string DateTime { get; set; }
        public string ConvertedDateTime { get; set; }
    }
}
