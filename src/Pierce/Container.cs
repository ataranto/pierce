namespace Pierce
{
    public abstract class Container : IContainer
    {
        public abstract T Get<T>() where T : class;

        public Syntax<TView> GetView<TView>() where TView : class
        {
            var view = Get<TView>();
            return new Syntax<TView>(this, view);
        }
    }
}

