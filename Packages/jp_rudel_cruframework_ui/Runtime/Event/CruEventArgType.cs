using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Engine
{
    [System.Serializable]
    public abstract class CruEventTargetArg
    {
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }
    
    [System.Serializable]
    public class CruEventTargetArg<T> : CruEventTargetArg
    {
        [SerializeField]
        private T value = default;
        /// <summary>値</summary>
        public T Value{get{return value;}set{this.value = value;}}

        /// <summary>値の取得</summary>
        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            this.value = (T)value;
        }
    }

    [System.Serializable]
    public class CruEventTargetArgInt : CruEventTargetArg<int>
    {
    }

    [System.Serializable]
    public class CruEventTargetArgString : CruEventTargetArg<string>
    {
    }
    
    [System.Serializable]
    public class CruEventTargetArgFloat : CruEventTargetArg<float>
    {
    }
    
    [System.Serializable]
    public class CruEventTargetArgBool : CruEventTargetArg<bool>
    {
    }
    
    [System.Serializable]
    public class CruEventTargetArgLong : CruEventTargetArg<long>
    {
    }
}