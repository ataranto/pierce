namespace Pierce.Example.Console
{
    public class SimpleInjectorContainer : Container
    {
        SimpleInjector.Container _container;

        public SimpleInjectorContainer(SimpleInjector.Container container)
        {
            _container = container;
        }

        public override T Get<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}

