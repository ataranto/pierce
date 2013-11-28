namespace Pierce.UI.Injection
{
    public abstract class Container : IContainer
    {
        public abstract T Get<T>() where T : class;

        public Syntax<TView> View<TView>() where TView : class, IView
        {
            var view = Get<TView>();
            return new Syntax<TView>(this, view);
        }

        public Syntax<TView> View<TView>(TView view) where TView : IView
        {
            return new Syntax<TView>(this, view);
        }
    }
}

