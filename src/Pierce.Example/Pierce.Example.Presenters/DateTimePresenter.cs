using System;
using Pierce.Example.Models;
using Pierce.Example.Views;
using Pierce.UI;

namespace Pierce.Example.Presenters
{
    public class DateTimePresenter : Presenter<IDateTimeModel, IDateTimeView>
    {
        Action<Action> _ui_thread;

        public DateTimePresenter(Action<Action> ui_thread)
        {
            _ui_thread = ui_thread;
        }

        public override void Initialize()
        {
            UpdateView();

            Model.Changed += model_Changed;
        }

        public override void Dispose()
        {
            Model.Changed -= model_Changed;
        }

        private void model_Changed(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var value = Model.Value;

            _ui_thread(() =>
            {
                View.SetValue(value);
            });
        }
    }
}

