using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace youviame.Config {
    public class UnityResolver : IDependencyResolver {

        protected IUnityContainer Container;
        public UnityResolver(IUnityContainer container) {
            if (container == null) {
                throw new ArgumentException("Container is null");
            }
            Container = container;
        }
        public void Dispose() {
            Container.Dispose();
        }

        public object GetService(Type serviceType) {
            try {
                return Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException) {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            try {
                return Container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException) {
                return new List<object>();
            }
        }
        public IDependencyScope BeginScope() {
            var childContainer = Container.CreateChildContainer();
            return new UnityResolver(childContainer);
        }
    }
}