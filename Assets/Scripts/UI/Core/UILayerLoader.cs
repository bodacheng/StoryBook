using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

internal class UILayerLoader
{
    private static Transform _hanger;
    private static Transform _fullScreenHanger;
    
    public static void SetHanger(Transform hanger, Transform fullScreenHanger)
    {
        _hanger = hanger;
        _fullScreenHanger = fullScreenHanger;
    }
    
    private static readonly List<UILayer> Queues = new List<UILayer>();
    
    public static void Clear(string except = null)
    {
        var toRemove = new List<UILayer>();
        foreach (var queue in Queues)
        {
            if (except != queue.Index)
            {
                toRemove.Add(queue);
            }
        }
        
        foreach (var layer in toRemove)
        {
            Queues.Remove(layer);
            if (layer != null && layer.gameObject != null)
                Remove(layer.Index);
        }
    }
    
    static T Get<T>()
    {
        var target = Queues.Find(x => x.Index == typeof(T).Name);
        if (target != null)
            return (T) Convert.ChangeType(target, typeof(T));
        return default;
    }
    
    public static async UniTask<T> LoadAsync<T>(bool insertToTop = false, bool loadToFullScreen = false) where T : UILayer
    {
        var targetHanger = loadToFullScreen ? _fullScreenHanger : _hanger;
        if (targetHanger == null)
            return default;
        string className = typeof(T).Name;
        var existed = Get<T>();
        if (existed != null)
        {
            await existed.OnPreOpen();
            if (loadToFullScreen)
            {
                var target = existed as GameObject;
                if (insertToTop)
                {
                    target?.transform.SetAsLastSibling();
                }
                else
                {
                    target?.transform.SetAsFirstSibling();
                }
            }
            else
            {
                if (insertToTop)
                {
                    var target = existed as GameObject;
                    target?.transform.SetAsLastSibling();
                }
            }
            return existed;
        }
        
        var layer = await AddressableLogic.Load<UILayer>(className);
        await layer.OnPreOpen();
        layer.Index = className;
        var rt = layer.GetComponent<RectTransform>();
        
        rt.SetParent(targetHanger);
        rt.localPosition = Vector3.zero;
        
        if (loadToFullScreen)
        {
            if (insertToTop)
            {
                rt.SetAsLastSibling();
            }
            else
            {
                rt.SetAsFirstSibling();
            }
        }
        else
        {
            if (insertToTop)
            {
                rt.SetAsLastSibling();
            }
        }
        
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        Queues.Add(layer);
        var returnValue = (T) Convert.ChangeType(layer, typeof(T));
        
        return returnValue;
    }
    
    public static void Pop()
    {
        if (Queues.Count > 0)
        {
            var uiLayer = Queues[^1];
            if (uiLayer != null)
            {
                uiLayer.OnDestroy();
                Object.Destroy(uiLayer);
            }
            Queues.RemoveAt(Queues.Count - 1);
        }
    }

    public static async UniTask Remove<T>()
    {
        var layerName = typeof(T).Name;
        await Remove(layerName);
    }

    static async UniTask Remove(string index)
    {
        var toRemoveIndex = -1;
        for (var i = 0; i < Queues.Count; i++)
        {
            var uiLayer = Queues[i];
            if (uiLayer.Index == index)
            {
                toRemoveIndex = i;
            }
        }
        
        if (toRemoveIndex >= 0)
        {
            var layer = Queues[toRemoveIndex];
            if (layer != null)
            {
                await layer.OnPreClose();
                Object.Destroy(layer.gameObject);
            }
            
            Queues.RemoveAt(toRemoveIndex);
        }
    }
}
