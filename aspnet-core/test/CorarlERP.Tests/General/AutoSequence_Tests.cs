using System;
using System.Data.SqlClient;
using CorarlERP.AutoSequences;
using Shouldly;
using Xunit;

namespace CorarlERP.Tests.General
{
    public class AutoSequence_Tests : AppTestBase
    {
        private readonly IAutoSequenceManager _autoSequenceManager;
      
        public AutoSequence_Tests()
        {
            _autoSequenceManager = Resolve<IAutoSequenceManager>();
        }

        [Fact]
        public void TestAutoSequenceNewYearFormatAsNon_Test()
        {

            var result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.None, "-", "001", "", DateTime.Now);
            result.ShouldBe("PO-001");

            result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.None, "-", "001", "PO-001", DateTime.Now);
            result.ShouldBe("PO-002");

           

        }

        [Fact]
        public void TestAutoSequenceNewYearFormatAsYY_Test()
        {

            var result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.YY, "-", "001", "", DateTime.Parse("2019/01/01"));
            result.ShouldBe("PO19-001");

            result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.YY, "-", "001", "PO19-001", DateTime.Parse("2019/01/02"));
            result.ShouldBe("PO19-002");

            result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.YY, "-", "001", "PO19-002", DateTime.Parse("2019/01/02"));
            result.ShouldBe("PO19-003");

            result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.YY, "-", "001", "PO19-999", DateTime.Parse("2019/01/02"));
            result.ShouldBe("PO19-1000");

            result = _autoSequenceManager.GetNewReferenceNumber("PO", enumStatus.EnumStatus.YearFormat.YY, "-", "001", "PO19-1000", DateTime.Parse("2020/01/01"));
            result.ShouldBe("PO20-001");

        }
    }
}
