using UnityEngine;
using UnityEngine.Pool;

public class GOPoolItem : MonoBehaviour
{
    public IObjectPool<GameObject> pool;

    private void OnDisable()
    {
        pool.Release(gameObject);
    }
}
