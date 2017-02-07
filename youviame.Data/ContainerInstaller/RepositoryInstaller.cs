using Microsoft.Practices.Unity;
using youviame.Common;
using youviame.Data.Repositories;

namespace youviame.Data.ContainerInstaller {
    public class RepositoryInstaller : IConfigureContainer {
        public void Configure(UnityContainer container) {
            container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());

            container.RegisterType<IMatchRepository, MatchRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IChatRepository, ChatRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ILogRepository, LogRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IReportRepository, ReportRepository>(new HierarchicalLifetimeManager());
        }
    }
}