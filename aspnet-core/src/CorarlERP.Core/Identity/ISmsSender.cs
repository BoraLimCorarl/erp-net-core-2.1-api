using System.Threading.Tasks;

namespace CorarlERP.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}