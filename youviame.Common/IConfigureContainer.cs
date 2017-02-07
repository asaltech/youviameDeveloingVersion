
using Microsoft.Practices.Unity;

namespace youviame.Common {
    public interface IConfigureContainer {
        void Configure(UnityContainer container);
    }
}