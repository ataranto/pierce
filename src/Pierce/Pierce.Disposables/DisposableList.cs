using System;
using System.Collections.Generic;

namespace Pierce.Disposables
{
    public class DisposableList : List<IDisposable>, IDisposable
    {
        public void Dispose()
        {
            foreach (var disposable in this)
            {
                disposable.Dispose();
            }

            Clear();
        }
    }
}
