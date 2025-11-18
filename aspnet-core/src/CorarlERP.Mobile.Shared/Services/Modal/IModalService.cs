using System.Threading.Tasks;
using CorarlERP.Views;
using Xamarin.Forms;

namespace CorarlERP.Services.Modal
{
    public interface IModalService
    {
        Task ShowModalAsync(Page page);

        Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView;

        Task<Page> CloseModalAsync();
    }
}
