using System.Threading.Tasks;

namespace DevFlow.CLI.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}
