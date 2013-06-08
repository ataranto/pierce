using System;

namespace Pierce.Example.Containers
{
    public class SimpleInjectorContainer : Container
    {
        public SimpleInjectorContainer()
        {
            Container = new SimpleInjector.Container();    
        }

        public SimpleInjector.Container Container { get; private set; }

        public override T Get<T>()
        {
            return Container.GetInstance<T>();
        }
    }
}

