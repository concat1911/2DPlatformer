using UnityEngine;

public class RendererEvent : MonoBehaviour
{
    public System.Action OnRenderVisible;
    public System.Action OnRenderInvisible;

    private void OnBecameVisible()
    {
        OnRenderVisible?.Invoke();
    }

    private void OnBecameInvisible()
    {
        OnRenderInvisible?.Invoke();
    }
}
