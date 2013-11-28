using Pierce.UI.Injection;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Pierce.UI
{
    public abstract class Presenter<TView> : IDisposable
    {
        protected readonly CompositeDisposable _disposables =
            new CompositeDisposable();

        public IContainer Container { protected get; set; }
        public IScheduler UIScheduler { protected get; set; }
        public TView View { protected get; set; }
        public abstract void Initialize();

        public virtual void Dispose()
        {
            _disposables.Dispose();

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

