using Microsoft.Practices.Unity;
using youviame.Common;
using youviame.Services;

namespace youviame.Data.ContainerInstaller {
    public class ServiceInstaller : IConfigureContainer {
        public void Configure(UnityContainer container) {
            container.RegisterType<INotificationService, NotificationService>(new HierarchicalLifetimeManager());
        }
    }
}