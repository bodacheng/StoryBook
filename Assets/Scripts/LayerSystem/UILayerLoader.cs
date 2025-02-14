using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

internal static class UILayerLoader
{
    private static Transform _hanger;
    private static Transform _fullScreenHanger;
    public static void SetHanger(Transform target, Transform fullScreenHanger)
    {
        _hanger = target;
        _fullScreenHanger = fullScreenHanger;
    }

    private static RectTransform effectBg;
    public static void SetEffectBg(RectTransform _effectBg)
    {
        effectBg = _effectBg;
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

    static UILayer _Get(string key)
    {
        return Queues.Find(x => x.Index == key);
    }
    
    public static T Get<T>()
    {
        var target = Queues.Find(x => x.Index == typeof(T).Name);
        if (target != null)
            return (T) Convert.ChangeType(target, typeof(T));
        else
        {
            return default;
        }
    }
    
    public static async UniTask<T> LoadAsync<T>(bool insertToTop = false, string key = null, bool loadToFullScreen = false) where T : UILayer
    {
        Transform targetHanger = loadToFullScreen ? _fullScreenHanger : _hanger;
        if (targetHanger == null)
            return default;
        string className = typeof(T).Name;
        var layerName = key != null ? key : className;
        var existed = Get<T>();
        if (existed != null)
        {
            await existed.OnPreOpen();
            if (loadToFullScreen)
            {
                var target = existed as GameObject;
                if (insertToTop)
                {
                    target?.transform.SetAsLastSibling();// 在_hanger（saferect）之下
                }
                else
                {
                    target?.transform.SetAsFirstSibling();// 在_hanger（saferect）之上
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
        
        var uiLayerPrefab = await AddressableLogic.LoadTOnObject<UILayer>(layerName);
        
        var t = GameObject.Instantiate(uiLayerPrefab);
        await t.OnPreOpen();
        t.Index = className;
        t.transform.SetParent(targetHanger);
        t.transform.localPosition = Vector3.zero;
        
        if (loadToFullScreen)
        {
            if (insertToTop)
            {
                t.transform.SetAsLastSibling();// 在_hanger（saferect）之下
            }
            else
            {
                t.transform.SetAsFirstSibling();// 在_hanger（saferect）之上
            }
        }
        else
        {
            if (insertToTop)
            {
                t.transform.SetAsLastSibling();// 在_hanger（saferect）之内
            }
        }
        
        var rt = t.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        Queues.Add(t);
        var returnValue = (T) Convert.ChangeType(t, typeof(T));
        
        if (effectBg != null)
        {
            effectBg.transform.SetParent(targetHanger);
            effectBg.transform.SetAsLastSibling();
        }
        
        return returnValue;
    }
    
    public static void Pop()
    {
        if (Queues.Count > 0)
        {
            var uiLayer = Queues[Queues.Count - 1];
            if (uiLayer != null)
            {
                uiLayer.OnDestroy();
                GameObject.Destroy(uiLayer);
            }
            Queues.RemoveAt(Queues.Count - 1);
        }
    }

    public static void Remove<T>()
    {
        string layerName = typeof(T).Name;
        Remove(layerName);
    }

    static void Remove(string index)
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
                GameObject.Destroy(layer.gameObject);
            Queues.RemoveAt(toRemoveIndex);
        }
    }
}
