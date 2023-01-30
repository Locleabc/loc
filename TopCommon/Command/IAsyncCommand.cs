using System.Threading.Tasks;
using System.Windows.Input;

namespace TopCom.Command
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
