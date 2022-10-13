using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPoolItem : MonoBehaviour
{
    public IObjectPool<GameObject> pool;

    private void OnDisable()
    {
        pool.Release(gameObject);
    }
}
