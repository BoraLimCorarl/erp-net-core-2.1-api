using Abp.Application.Services.Dto;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.Classes
{
   public class ClassAppService_Test : AppTestBase
    {
        private readonly IClassAppService _classAppService;
      //  private readonly Class _classes;

        public ClassAppService_Test()
        {
            LoginAsDefaultTenantAdmin();
              _classAppService = Resolve<IClassAppService>();
          //  _classes = CreateClass(null, null, "AENON I", false, null);
        }
        private CreateClassInput CreateHelper()
        {
            var ClassExemptInput = new CreateClassInput()
            {
                ClassName = "AEON Shop",
                ClassParent = false,
                ParentClassId =1, 
            };
            return ClassExemptInput;
        }
        private void CheckClass(CreateClassInput ClassExemptInput, long? Id = null)
        {
            UsingDbContext(context =>
            {
                var ClassEntity = context.Classes.FirstOrDefault(p => p.ClassName == ClassExemptInput.ClassName);
                ClassEntity.ShouldNotBeNull();
                if (Id != null) ClassEntity.Id.ShouldBe(Id.Value);               
                ClassEntity.ClassName.ShouldBe(ClassExemptInput.ClassName);
                ClassEntity.ParentClassId.ShouldBe(ClassExemptInput.ParentClassId);
                ClassEntity.ClassParent.ShouldBe(ClassExemptInput.ClassParent);
                ClassEntity.IsActive.ShouldBe(true);                                                                       
            });
        }
        [Fact]
        public async Task Test_CreateSingleClass()
        {
            var ClassExemptInput = CreateHelper();
            var result = await _classAppService.Create(ClassExemptInput);
            result.ShouldNotBeNull();
            CheckClass(ClassExemptInput);
        }

        [Fact]
        public async Task Test_CreateSingleGetList()
        {
            var classes = CreateHelper();
            var result = await _classAppService.Create(classes);
            
            var output = await _classAppService.GetList(new GetClassListInput()
            {
                Filter = ""


            });
            foreach (var i in output.Items)
            {

                i.ClassName.ShouldBe(classes.ClassName);
                i.ParentClassId.Equals(classes.ParentClassId);
                i.ClassParent.ShouldBe(classes.ClassParent);
                
            }
        }

        [Fact]
        public async Task Test_CreateSingleFind()
        {
            var classes = CreateHelper();
            var result = await _classAppService.Create(classes);

            var output = await _classAppService.Find(new GetClassListInput()
            {
                Filter = ""


            });
            foreach (var i in output.Items)
            {

                i.ClassName.ShouldBe(classes.ClassName);
                i.ParentClassId.Equals(classes.ParentClassId);
                i.ClassParent.ShouldBe(classes.ClassParent);

            }
        }

        [Fact]
        public async Task Test_GetDetailClass()
        {

            var CleassExemptInput = CreateHelper();
            var result = await _classAppService.Create(CleassExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _classAppService.GetDetail(new EntityDto<long>()
            {

                Id = result.Id,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id);
            output.ClassName.ShouldBe(CleassExemptInput.ClassName);
            output.ClassParent.ShouldBe(CleassExemptInput.ClassParent);
            output.ParentClassId.ShouldBe(CleassExemptInput.ParentClassId);
            CheckClass(CleassExemptInput);
        }

        [Fact]
        public async Task Test_DelectClass()
        {
            var ClassExemptInput = CreateHelper();
            var result = await _classAppService.Create(ClassExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var ClassEntity = context.Classes.FirstOrDefault(p => p.ClassName == ClassExemptInput.ClassName);
            });
            await _classAppService.Delete(new EntityDto<long> { Id = result.Id });

            UsingDbContext(context =>
            {
                context.PurchaseOrders.Count().ShouldBe(0);
            });
        }

    }
}
