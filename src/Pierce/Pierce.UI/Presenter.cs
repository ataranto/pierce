using Pierce.Injection;
using System;

namespace Pierce.UI
{
    public abstract class Presenter<TView> : IDisposable
    {
        public IContainer Container { protected get; set; }
        public TView View { protected get; set; }
        public abstract void Initialize();

        public virtual void Dispose()
        {
            Container = null;
            View = default(TView);
        }
    }

    public abstract class Presenter<TModel, TView> : Presenter<TView>
    {
        public TModel Model { protected get; set; }

        public override void Dispose()
        {
            base.Dispose();
            Model = default(TModel);
        }
    }
}

