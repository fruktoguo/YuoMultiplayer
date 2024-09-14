using DG.Tweening;
using ET;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class UIAnimaComponent : YuoComponent
    {
        public override string Name => "动画信息";

        public RectTransform rectTransform;

        public UISetting.UISate Sate;

        private float animatorDuration;

        [Sirenix.OdinInspector.ShowInInspector] [Sirenix.OdinInspector.ReadOnly]
        public float AnimaDuration
        {
            get
            {
                if (doTweenAnimation)
                {
                    return doTweenAnimation.duration;
                }

                return animatorDuration;
            }
            set
            {
                if (doTweenAnimation == null && animator == null)
                {
                    animatorDuration = value;
                }
                else
                {
                    "已有动画组件，无法手动设置动画时长".LogError();
                }
            }
        }

        private DOTweenAnimation doTweenAnimation;

        private Animator animator;

        public async ETTask Open()
        {
            if (AnimaDuration > 0.0001f)
            {
                if (doTweenAnimation)
                {
                    doTweenAnimation.DOPlayForward();
                }
                else if (animator)
                {
                    animator.Play("In");
                }

                Sate = UISetting.UISate.ShowAnima;
                await YuoWait.WaitTimeAsync(AnimaDuration);
            }

            Sate = UISetting.UISate.Show;
        }

        public async ETTask Close()
        {
            if (AnimaDuration > 0.0001f)
            {
                if (doTweenAnimation)
                {
                    doTweenAnimation.DOPlayBackwards();
                }
                else if (animator)
                {
                    animator.Play("Out");
                }

                Sate = UISetting.UISate.HideAnima;
                await YuoWait.WaitTimeAsync(AnimaDuration);
            }

            Sate = UISetting.UISate.Hide;
        }

        public void From(UISetting setting)
        {
            rectTransform = setting.transform as RectTransform;
            doTweenAnimation = setting.GetComponent<DOTweenAnimation>();
            animator = setting.GetComponent<Animator>();
            animatorDuration = setting.AnimatorLength;
        }
    }
}