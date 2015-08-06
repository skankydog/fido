using Fido.WebUI.Flash.Messages;

namespace Fido.WebUI.Flash
{
    public interface IFlasher : IPusher, IPopper
    {
        int Count { get; }
        void Clear();
    }

    public interface IPusher
    {
        void Success(string Description, object Data = null);
        void Info(string Description, object Data = null);
        void Warning(string Description, object Data = null);
        void Error(string Description, object Data = null);
    }

    public interface IPopper
    {
        FlashMessage Pop();
    }
}
