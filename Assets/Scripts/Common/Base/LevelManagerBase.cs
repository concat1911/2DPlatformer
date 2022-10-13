namespace ML
{
    using UnityEngine;

    public abstract class LevelLoader
    {
        protected string levelName;
        protected LoadAddress loader;
        public Transform levelParent;

        public virtual void LoadLevel()
        {
            loader.key = levelName;
            loader.StartLoading(OnLevelLoaded);
        }

        protected virtual void OnLevelLoaded(GameObject levelObj)
        {
            GameObject.Instantiate(levelObj, levelParent);
        }
    }
}
