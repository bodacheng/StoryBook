using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressableLogic
{
    private static readonly IDictionary<string, long> Sizes = new Dictionary<string, long>();
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
            Debug.Log(" error ");
        }
        Addressables.Release(locationHandle);
    }

    public static bool CheckKeyExist(string tag, string primaryKey)
    {
        if (!KeyExists.ContainsKey(tag))
        {
            return false;
        }
        return KeyExists[tag].Contains(primaryKey);
    }
    
    public static async UniTask Essentials()
    {
        await UniTask.WhenAll(new List<UniTask>()
        {
            CheckExistedKey("weapon"),
            CheckExistedKey("effect"),
            CheckExistedKey("unit_image")
        });
    }
    
    public static async UniTask<GameObject> LoadObject(string prefabPathName, Vector3 pos = new Vector3())
    {
        var handle = Addressables.InstantiateAsync(prefabPathName, pos, Quaternion.identity);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Failed to load : {prefabPathName}");
            Addressables.ReleaseInstance(handle);
            return default;
        }
        else
        {
            var _object = handle.Result; // インスタンス化されたもの
            _object.AddOnDestroyCallback( () =>
            {
                Addressables.ReleaseInstance(handle);
            });
            return _object;
        }
    }
    
    public static async UniTask<T> LoadTOnObject<T>(string prefabPathName)
    {
        var handle = Addressables.InstantiateAsync(prefabPathName);
        await handle.Task;
        if (handle.IsValid() && handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Failed to load : {prefabPathName}");
            Addressables.ReleaseInstance(handle);
            return default;
        }
        else
        {
            if (!handle.IsValid())
            {
                return default;
            }
            var _object = handle.Result; // インスタンス化されたもの
            _object.AddOnDestroyCallback( () =>
            {
                Addressables.ReleaseInstance(handle);
            });
            var returnValue = _object.GetComponent<T>();
            return returnValue;
        }
    }
    
    public static async UniTask<T> LoadTOnObject<T>(string prefabPathName, GameObject memoryReleaseTarget = null, CancellationTokenSource _cancellationTokenSource = null)
    {
        try
        {
            var handle = Addressables.InstantiateAsync(prefabPathName);
            if (_cancellationTokenSource != null)
            {
                await handle.ToUniTask(cancellationToken: _cancellationTokenSource.Token);
            }
            else
            {
                await handle.Task;
            }
            
            if (handle.IsValid() && handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load : {prefabPathName}");
                Addressables.ReleaseInstance(handle);
                return default;
            }
            else
            {
                var _object = handle.Result; // インスタンス化されたもの
                if (memoryReleaseTarget == null)
                {
                    _object.AddOnDestroyCallback( () =>
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
                var returnValue = _object.GetComponent<T>();
                return returnValue;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return default;
    }

    private static readonly List<AsyncOperationHandle> LoadingHandlerList = new List<AsyncOperationHandle>();
    
    public static async UniTask<T> LoadT<T>(string prefabPathName, GameObject memoryReleaseTarget = null)
    {
        var handle = Addressables.LoadAssetAsync<T>(prefabPathName);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Failed to load : {prefabPathName}");
            Addressables.Release(handle);
            return default;
        }
        else
        {
            if (memoryReleaseTarget == null)
            {
                LoadingHandlerList.Add(handle);
            }
            else
            {
                memoryReleaseTarget.AddOnDestroyCallback( () =>
                {
                    Addressables.ReleaseInstance(handle);
                });
            }
            return handle.Result;
        }
    }
    
    public static async UniTask<T> LoadT<T>(IResourceLocation location, GameObject memoryReleaseTarget = null)
    {
        var handle = Addressables.LoadAssetAsync<T>(location);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Failed to load : {location}");
            Addressables.Release(handle);
            return default;
        }
        else
        {
            if (memoryReleaseTarget == null)
            {
                LoadingHandlerList.Add(handle);
            }
            else
            {
                memoryReleaseTarget.AddOnDestroyCallback( () =>
                {
                    Addressables.ReleaseInstance(handle);
                });
            }
            return handle.Result;
        }
    }
    
    public static void ReleaseAsyncOperationHandles()
    {
        foreach (var handle in LoadingHandlerList)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        LoadingHandlerList.Clear();
    }
}
