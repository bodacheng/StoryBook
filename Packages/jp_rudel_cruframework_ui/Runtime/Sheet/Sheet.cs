using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Engine.UI
{

    public abstract class Sheet : MonoBehaviour
    {
        /// <summary>開く前の処理</summary>
        protected virtual UniTask OnPreOpen()
        {
            return default;
        }
        
        /// <summary>開いたときの処理/summary>
        protected virtual UniTask OnOpen()
        {
            return default;
        }
        
        /// <summary>閉じたときの処理/summary>
        protected virtual UniTask OnClose()
        {
            return default;
        }

        /// <summary>開く前の処理</summary>
        internal UniTask OnPreOpenInternal()
        {
            return OnPreOpen();
        }
        
        /// <summary>開いたときの処理/summary>
        internal UniTask OnOpenInternal()
        {
            return OnOpen();
        }
        
        /// <summary>閉じたときの処理/summary>
        internal UniTask OnCloseInternal()
        {
            return OnClose();
        }
        
    }
}
