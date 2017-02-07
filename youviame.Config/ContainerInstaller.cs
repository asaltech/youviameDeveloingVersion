using Microsoft.Practices.Unity;
using youviame.Data.ContainerInstaller;

namespace youviame.Config
{
    public class ContainerInstaller {
        private static UnityContainer _container;
        public static UnityContainer Container {
            get {
                if (_container == null) Initialize();
                return _container;
            }
        }
        private static void Initialize() {
            _container = new UnityContainer();
            new DbContextInstaller().Configure(_container);
            new RepositoryInstaller().Configure(_container);
            new ServiceInstaller().Configure(_container);
        }
    }
}
