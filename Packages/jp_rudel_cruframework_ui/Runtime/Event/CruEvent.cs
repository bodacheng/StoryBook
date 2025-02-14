using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace CruFramework.Engine
{
    [System.Serializable]
    public abstract class CruEventBase
    {
        public abstract CruEventTargetBase[] GetEventTargets();
        public abstract void AddEmpty();
        public abstract void RemoveAt(int index);
        public abstract int Count{get;}
    }
    
    [System.Serializable]
    public abstract class CruEventBase<T> : CruEventBase where T : CruEventTargetBase, new()
    {
        [SerializeField]
        protected List<T> targets = new List<T>();
        /// <summary>呼び出し対象</summary>
        public IReadOnlyList<T> Targets{get{return targets;}}

        /// <summary>イベント数</summary>
        public override int Count{get{return targets.Count;}}

        public sealed override CruEventTargetBase[] GetEventTargets()
        {
            return targets.ToArray();
        }

        /// <summary>からを追加</summary>
        public sealed override void AddEmpty()
        {
            targets.Add( new T() );
        }
        
        /// <summary>すべてのイベント削除</summary>
        public void RemoveAllListeners()
        {
            targets.Clear();
        }

        /// <summary>イベント削除</summary>
        public sealed override void RemoveAt(int index)
        {
            targets.RemoveAt(index);
        }

        /// <summary>イベント追加</summary>
        public void AddListener(object target, MethodInfo method, object[] arguments = null)
        {
            // 既に登録済み
            foreach(T t in targets)
            {
                if(t.IsEqual(target, method))return;
            }
            
            T add = new T();
            add.SetMethod(target, method);
            // 引数
            if(arguments != null)
            {
                for(int i=0;i<arguments.Length;i++)
                {
                    add.SetArgument(i, arguments[i]);
                }
            }
            targets.Add(add);
        }
    }

    [System.Serializable]
    public class CruEvent : CruEventBase<CruEventTarget>
    {
        public void AddListener(Action action)
        {
            AddListener(action.Target, action.Method);
        }
        
        /// <summary>呼び出し</summary>
        public void Invoke()
        {
            foreach(CruEventTarget t in targets)
            {
                t.Invoke();
            }
        }
    }
    
    [System.Serializable]
    public class CruEvent<T> : CruEventBase<CruEventTarget<T>>
    {
        public void AddListener(Action<T> action)
        {
            AddListener(action.Target, action.Method);
        }
        
        /// <summary>呼び出し</summary>
        public void Invoke()
        {
            foreach(CruEventTarget<T> t in targets)
            {
                t.Invoke();
            }
        }
        
        /// <summary>呼び出し</summary>
        public void Invoke(T value)
        {
            foreach(CruEventTarget<T> t in targets)
            {
                t.Invoke(value);
            }
        }
    }
}