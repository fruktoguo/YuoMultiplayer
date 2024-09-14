using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_HoverMessageItemComponent
    {
    }

    public class ViewHoverMessageItemCreateSystem : YuoSystem<View_HoverMessageItemComponent>, IUICreate
    {
        public override string Group => "UI/HoverMessageItem";

        protected override void Run(View_HoverMessageItemComponent view)
        {
            view.FindAll();
            var anima = view.AddComponent<UIAnimaComponent>();
            anima.AnimaDuration = 0.2f;
        }
    }

    public class ViewHoverMessageItemOpenSystem : YuoSystem<View_HoverMessageItemComponent>, IUIOpen
    {
        public override string Group => "UI/HoverMessageItem";

        protected override void Run(View_HoverMessageItemComponent view)
        {
            view.rectTransform.SetSizeX(800);
        }
    }

    public class ViewHoverMessageItemOpenAnimaSystem : YuoSystem<View_HoverMessageItemComponent, UIAnimaComponent>,
        IUIOpen
    {
        public override string Group => "UI/HoverMessageItem";

        protected override void Run(View_HoverMessageItemComponent view, UIAnimaComponent anima)
        {
            0f.To(1, anima.AnimaDuration, x =>
            {
                view.MainCanvasGroup.alpha = x;
                view.rectTransform.SetSizeX(x * 600 + 200);
            });
        }
    }

    public class ViewHoverMessageItemCloseAnimaSystem : YuoSystem<View_HoverMessageItemComponent, UIAnimaComponent>,
        IUIClose
    {
        public override string Group => "UI/HoverMessageItem";

        protected override void Run(View_HoverMessageItemComponent view, UIAnimaComponent anima)
        {
            1f.To(0, anima.AnimaDuration, x =>
            {
                view.MainCanvasGroup.alpha = x;
                view.rectTransform.SetSizeX(x * 600 + 200);
            });
        }
    }
}