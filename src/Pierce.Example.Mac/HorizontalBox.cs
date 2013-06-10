using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pierce.Example.Mac
{
    public class HorizontalBox : Box
    {
        protected override float Extent
        {
            get { return Frame.Width; }
        }

        protected override void Update(IEnumerable<Child> visible_children, float expand_extent)
        {
            var x = 0f;

            foreach (var child in visible_children)
            {
                var width = child.Extent > 0 ?
                    child.Extent :
                    expand_extent;
                
                child.View.Frame = new RectangleF
                {
                    X = x,
                    Y = 0,
                    Width = width,
                    Height = Frame.Height,
                };
                
                x += width + _padding;
            }
        }
    }
}

