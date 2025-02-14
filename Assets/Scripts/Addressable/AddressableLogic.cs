using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableLogic
{
    private static readonly IDictionary<string, List<string>> KeyExists = new Dictionary<string, List<string>>();
    
    public static async UniTask CheckExistedKey(string tag)
    {
        if (KeyExists.ContainsKey(tag))
        {
            return;
        }
        KeyExists.Add(tag, new List<string>());
        var locationHandle = Addressables.LoadResourceLocationsAsync(tag);
        await locationHandle.Task;
        if (locationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var weapon in locationHandle.Result)
            {
                if (!KeyExists[tag].Contains(weapon.PrimaryKey))
                {
                    KeyExists[tag].Add(weapon.PrimaryKey);
                }
            }
        }
        else
        {
            Debug.Log("error");
        }
        Addressables.Release(locationHandle);
    }
    
    public static bool CheckKeyExist(string tag, string primaryKey)
    {
        return KeyExists.ContainsKey(tag) && KeyExists[tag].Contains(primaryKey);
    }
    
    public static async UniTask<T> Load<T>(string key, GameObject memoryReleaseTarget = null, CancellationTokenSource cancellationTokenSource = null)
    {
        var handle = Addressables.InstantiateAsync(key);
        if (cancellationTokenSource != null)
        {
            await handle.ToUniTask(cancellationToken: cancellationTokenSource.Token);
        }
        else
        {
            await handle.Task;
        }
        
        if (handle.IsValid() && handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Failed to load : {key}");
            Addressables.ReleaseInstance(handle);
            return default;
        }
        
        if (!handle.IsValid())
        {
            return default;
        }
        var handleResult = handle.Result; // インスタンス化されたもの
        
        if (memoryReleaseTarget == null)
        {
            handleResult.AddOnDestroyCallback( () =>
            {
                Addressables.ReleaseInstance(handle);
            });
        }
        else
        {
            memoryReleaseTarget.AddOnDestroyCallback( () =>
            {
                Addressables.ReleaseInstance(handle);
            });
        }
        
        var returnValue = handleResult.GetComponent<T>();
        return returnValue;
    }
}
