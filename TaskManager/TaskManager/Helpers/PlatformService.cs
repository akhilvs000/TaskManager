using Plugin.Connectivity;
using TaskManager.Interfaces;

namespace TaskManager.Helpers
{
    public class PlatformService: IPlatformService
    {
        public bool IsConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }
    }
}
