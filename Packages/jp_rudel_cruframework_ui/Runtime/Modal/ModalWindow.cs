using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CruFramework.Engine.UI
{
    public abstract class ModalWindow : MonoBehaviour
    {
        
        private ModalManager modalManager = null;
        /// <summary>マネージャー</summary>
        public ModalManager Manager{get{return modalManager;}internal set{modalManager=value;}}
        
        private int modalId = 0;
        /// <summary>Id</summary>
        public int ModalId{get{return modalId;}}
        
        private object openArguments = null;
        /// <summary>引数</summary>
        public object OpenArguments{get{return openArguments;}}
        
        private ModalOptions options = ModalOptions.None;
        /// <summary>オプション</summary>
        public  ModalOptions Options{get{return options;}internal set{options = value;}}
        
        internal ModalState state = ModalState.Idle;
        /// <summary>ステート</summary>
        public ModalState State{get{return state;}}
        
        // 閉じたときに返すパラメータ
        private object closedParameter = null;
        
        /// <summary>Id</summary>
        internal void SetId(int id)
        {
            modalId = id;
        }
        
        internal void SetArguments(object args)
        {
            openArguments = args;
        }
        
        protected virtual UniTask OnOpen()
        {
            return default;
        }
        
        protected virtual UniTask OnPreOpen()
        {
            return default;
        }
        
        protected virtual UniTask OnPreClose()
        {
            return default;
        }
        
        protected virtual UniTask OnClosed()
        {
            return default;
        }

        internal async UniTask OnActiveInternal()
        {
            await OnOpen();
        }

        internal async UniTask OnPreOpenInternal()
        {
            await OnPreOpen();
        }
        
        internal async UniTask OnCloseInternal()
        {
            await OnPreClose();
        }
        
        internal async UniTask OnClosedInternal()
        {
            await OnClosed();
        }
        
        /// <summary>閉じるときに渡すパラメータをセット</summary>
        public void SetCloseParameter(object value)
        {
            closedParameter = value;
        }
        
        /// <summary>閉じる</summary>
        public void Close(Action onCompleted)
        {
            modalManager.OnCloseModal(this, onCompleted);
        }
        
        /// <summary>閉じる</summary>
        public void Close()
        {
            modalManager.OnCloseModal(this, null);
        }
        
        /// <summary>閉じる</summary>
        public async UniTask<object> CloseAsync()
        {
            await modalManager.OnCloseModalAsync(this);
            return closedParameter;
        }
        
        /// <summary>閉じるまで待機</summary>
        public async UniTask<object> WaitCloseAsync( CancellationToken cancellationToken = default )
        {
            await UniTask.WaitWhile(()=>state != ModalState.Closed && state != ModalState.Removed, PlayerLoopTiming.Update, cancellationToken);
            return closedParameter;
        }
    }
}