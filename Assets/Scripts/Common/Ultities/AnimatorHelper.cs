namespace ML
{
    using UnityEngine;

    public static class AnimatorHelper
    {
        public static readonly int InitHashed = Animator.StringToHash("Init");
        public static readonly int ShowHashed = Animator.StringToHash("Show");
        public static readonly int HideHashed = Animator.StringToHash("Hide");
        public static readonly int DefHashed  = Animator.StringToHash("Def");
        public static readonly int InHahsed  = Animator.StringToHash("In");
        public static readonly int OutHashed = Animator.StringToHash("Out");
    }
}