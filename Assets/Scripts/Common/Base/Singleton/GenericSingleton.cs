using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour
	where T : Component
{
	public static T instance { get; private set; }

    [SerializeField] bool NoDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if( instance == null )
        {
            instance = this as T;

            if( NoDestroyOnLoad )
            {
                DontDestroyOnLoad(gameObject);
            }
        }else
        {
            Destroy(gameObject);
        }
    }
}