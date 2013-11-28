using System;

namespace Pierce.UI
{
	public interface IView
	{
        IObservable<ViewState> State { get; }
	}
}

