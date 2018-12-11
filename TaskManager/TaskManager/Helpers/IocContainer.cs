using DryIoc;

namespace TaskManager.Helpers
{
    public class IocContainer
    {
        private static IocContainer instance = null;
        private static readonly object padlock = new object();
        public Container Container { get; set; }

        private IocContainer()
        {
            Container = new Container();
        }

        public static IocContainer Instance
        {
            get
            { 
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new IocContainer();
                    }
                    return instance;
                } 
            }
        }
    }
}
