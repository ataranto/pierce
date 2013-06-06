using System;

namespace Pierce.Example.Console
{
    public class DateTimePresenter : Presenter<IDateTime, IDateTimeView>
    {
        public override void Initialize()
        {
            var value = Model.Value.ToString();
            View.SetValue(value);

            Model.Changed += model_Changed;
        }

        public override void Dispose()
        {
            Model.Changed -= model_Changed;
        }

        private void model_Changed (object sender, EventArgs e)
        {
            var value = Model.Value.ToString();
            View.SetValue(value);
        }
    }
}

