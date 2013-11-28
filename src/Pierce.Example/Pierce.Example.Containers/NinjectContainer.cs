using Ninject;
using Pierce.UI.Injection;

namespace Pierce.Example.Containers
{
    public class NinjectContainer : Container
    {
        private readonly IKernel _kernel;

        public NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override T Get<T>()
        {
            return _kernel.Get<T>();
        }
    }
}

