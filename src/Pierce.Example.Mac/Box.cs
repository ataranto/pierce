using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoMac.AppKit;

namespace Pierce.Example.Mac
{
    public abstract class Box : NSView
    {
        protected class Child
        {
            public NSView View { get; set; }
            public float Extent { get; set; }
        }

        protected const int _padding = 6;
        private readonly List<Child> _children = new List<Child>();

        protected abstract float Extent { get; }
        protected abstract void Update(IEnumerable<Child> visible_children, float extent);

        public override void AddSubview(NSView view)
        {
            AddSubview(view, 0);
        }
        
        public void AddSubview(NSView view, float extent)
        {
            base.AddSubview(view);
            _children.Add(new Child
            {
                View = view,
                Extent = extent,
            });
        }

        public void Update()
        {
            var visible_children = _children.Where(x => x.View.Hidden == false);
            var padding_extent = visible_children.Count() < 2 ?
                0 :
                (visible_children.Count() - 1) * _padding;
            var extra_extent = Extent - padding_extent - visible_children.
                Where(x => x.Extent > 0).
                Select(x => x.Extent).
                Sum();
            var expand_count = visible_children.
                Where(x => x.Extent == 0).
                Count();
            var expand_extent = extra_extent / expand_count;

            Update(visible_children, expand_extent);
        }

        public override void SetFrameSize(SizeF size)
        {
            base.SetFrameSize(size);
            Update();
        }
    }
}

