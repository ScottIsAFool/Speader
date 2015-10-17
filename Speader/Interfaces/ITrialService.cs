using System.Threading.Tasks;

namespace Speader.Interfaces
{
    public interface ITrialService
    {
        bool IsTrial { get; }
        void Buy();
        Task ShowTrialMessage(string message);
    }
}