using Xamarin.Forms.Internals;

namespace CorarlERP.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}