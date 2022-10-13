namespace ML
{
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public abstract class UIManagerBase : MonoBehaviour
    {
        protected Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public virtual void Show()
        {
            animator.Play(UIAnimCommon.ShowHashed);
        }

        public virtual void Hide()
        {
            animator.Play(UIAnimCommon.HideHashed);
        }

        /// <summary>
        /// Call on animation event at frame that define end animation
        /// </summary>
        public virtual void CompleteAnimation()
        {

        }
    }

    public static class UIAnimCommon
    {
        public static readonly int ShowHashed       = Animator.StringToHash("Show");
        public static readonly int HideHashed       = Animator.StringToHash("Hide");
    }
}
