using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Pierce.UI.Injection
{
    public class Syntax<TView>
        where TView : IView
    {
        protected readonly IContainer _container;
        protected readonly TView _view;

        public Syntax(IContainer container, TView view)
        {
            _container = container;
            _view = view;
        }

		public Syntax<TView> Do(Action<TView> action)
		{
			action(_view);
			return this;
		}

        public Syntax<TModel, TView> Model<TModel>(TModel model = null) where TModel : class
        {
            model = model ?? _container.Get<TModel>();
            return new Syntax<TModel, TView>(_container, model, _view);
        }

        public TView ToView()
        {
            return _view;
        }
    }

    public class Syntax<TModel, TView> : Syntax<TView>
        where TView : IView
    {
        private readonly CompositeDisposable _disposable =
            new CompositeDisposable();
        private readonly TModel _model;

        public Syntax(IContainer container, TModel model, TView view)
            : base(container, view)
        {
            _model = model;
        }

        public Syntax<TModel, TView> Presenter<TViewInterface, TPresenter>()
            where TViewInterface : class, IView
            where TPresenter : Presenter<TModel, TViewInterface>
        {
            var presenter = _container.Get<TPresenter>();
            _disposable.Add(presenter);

            presenter.Container = _container;
            presenter.UIScheduler = _container.Get<IScheduler>();
            presenter.Model = _model;
            presenter.View = _view as TViewInterface;

            _view.State.
                Where(state => state == ViewState.Initialize).
                Take(1).
                Subscribe(state => presenter.Initialize());
            _view.State.
                Where(state => state == ViewState.Dispose).
                Take(1).
                Subscribe(state => presenter.Dispose());

            return this;
        }

        public IDisposable ToDisposable()
        {
            return _disposable;
        }
    }
}

