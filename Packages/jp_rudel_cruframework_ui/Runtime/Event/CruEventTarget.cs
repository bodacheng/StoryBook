using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using JetBrains.Annotations;

namespace CruFramework.Engine
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CruEventTargetAttribute : System.Attribute
    {
    }
    
    [System.Serializable]
    public class CruEventTarget : CruEventTargetBase
    {
        public void SetAction(Action action)
        {
            SetMethod(action.Target, action.Method);
        }
    }
    
    [System.Serializable]
    public class CruEventTarget<T> : CruEventTargetBase
    {
        public void SetAction(Action<T> action)
        {
            SetMethod(action.Target, action.Method);
        }
    }
    

    
    
    [System.Serializable]
    public abstract class CruEventTargetBase
    {
        
        [SerializeField]
        private Component targetComponent = null;
        /// <summary>イベントを飛ばす先</summary>
        public Component TargetComponent{get{return targetComponent;}}
        
        [SerializeField]
        private string methodName = string.Empty;
        /// <summary>メソッド名</summary>
        public string MethodName{get{return methodName;}}
        
        
        [SerializeReference]
        private CruEventTargetArg[] arguments = new CruEventTargetArg[0];
        
        // メソッド
        private  MethodInfo method = null;
        // 対象
        private object target = null;
        
        // 呼び出し時の引数
        private object[] invokeArguments = null;
        
        public bool IsEqual(object target, MethodInfo method)
        {
            return this.target == target && this.method == method;
        }
        
        public MethodInfo GetComponentMethodInfo()
        {
            if(targetComponent == null || string.IsNullOrEmpty(methodName))
            {
                return null;
            }
            
            return CruEventUtility.GetMethod(targetComponent.GetType(), methodName);
        }
        
        private void CheckMethod()
        {
            if(method != null)return;
            SetMethod(targetComponent, GetComponentMethodInfo());
        }
        
        public void SetMethod(object target, MethodInfo method)
        {
            // コンポーネントの場合
            targetComponent = target as Component;
            // メソッド
            this.method = method;
            // 呼び出し対象
            this.target = target;

            
            // 引数
            if(method != null)
            {
                // メソッド名
                methodName = method.Name;
                
                ParameterInfo[] parameters = method.GetParameters();
                arguments = new CruEventTargetArg[parameters.Length];
                for(int i=0;i<arguments.Length;i++)
                {
                    arguments[i] = (CruEventTargetArg)Activator.CreateInstance(CruEventUtility.GetArgumentType(parameters[i].ParameterType));
                }
                
                // 引数
                invokeArguments = new object[parameters.Length];
                
                for(int i=0;i<invokeArguments.Length;i++)
                {
                    invokeArguments[i] = arguments[i].GetValue();
                }
            }
        }
        
        public void SetArgument(int index, object value)
        {
            invokeArguments[index] = value;
            arguments[index].SetValue(value);
        }
        
        public void Invoke()
        {
            CheckMethod();
            Invoke(invokeArguments);
        }
        
        public void Invoke(params object[] args)
        {
            CheckMethod();
            if(method != null)
            {
                method.Invoke(target, args);
            }
        }
    }
}