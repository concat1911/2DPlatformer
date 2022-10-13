using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class LoadAddress
{
    public string key;
    public string label;
    AsyncOperationHandle<GameObject> opHandle;

    public IEnumerator StartLoading(System.Action<GameObject> OnFinish)
    {
        opHandle = Addressables.LoadAssetAsync<GameObject>(key);
        yield return opHandle;

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            OnFinish.Invoke(opHandle.Result);
            Dispose();
        }
    }

    void Dispose()
    {
        Addressables.Release(opHandle);
    }
}