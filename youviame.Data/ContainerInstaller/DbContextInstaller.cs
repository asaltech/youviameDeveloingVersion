using Microsoft.Practices.Unity;
using youviame.Common;
using youviame.Data.Context;

namespace youviame.Data.ContainerInstaller {
    public class DbContextInstaller : IConfigureContainer {
        public void Configure(UnityContainer container) {
            container.RegisterType<IYouviameContext, YouviameContext>(new HierarchicalLifetimeManager());
        }
    }
}
