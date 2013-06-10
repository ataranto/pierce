using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pierce.Example.Mac
{
    public class VerticalBox : Box
    {
        protected override float Extent
        {
            get { return Frame.Height; }
        }

        protected override void Update(IEnumerable<Child> visible_children, float expand_extent)
        {
            var y = 0f;

            foreach (var child in visible_children)
            {
                var height = child.Extent > 0 ?
                    child.Extent :
                    expand_extent;

                child.View.Frame = new RectangleF
                {
                    X = 0,
                    Y = y,
                    Width = Frame.Width,
                    Height = height,
                };

                y += height + _padding;
            }
        }
    }
}

