using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CruFramework.Engine.UI
{
    
    public enum ModalOptions
    {
        None = 0,
        /// <summary>最前面のモーダルを維持したまま開く</summary>
        KeepFrontModal = 1 << 0,
    }
    
    public enum ModalLoadState
    {
        Begin, End
    }
    
    public enum ModalState
    {
        Idle, Opened, Closed, Removed, Deactive, OpenError
    }
    
    public abstract class ModalManager : MonoBehaviour
    {
        protected struct ModalData
        {
            public ModalWindow modalWindow;
            
            public ModalData(ModalWindow modalWindow)
            {
                this.modalWindow = modalWindow;
            }
        }
        
        [SerializeField]
        protected GameObject modalBackground = null;
        [SerializeField]
        protected GameObject modalRoot = null;
        
        private Action<ModalLoadState> onLoading = null;
        /// <summary>読み込み中の変化時のコールバック</summary>
        public Action<ModalLoadState> OnLoading{get{return onLoading;}set{onLoading = value;}}

        // モーダルに割り当てるId
        protected int allocateModalId = 0;
        // 開いているモーダル
        protected Dictionary<int, ModalData> modalList = new Dictionary<int, ModalData>();

        protected List<int> modalStack = new List<int>();
        /// <summary>モーダルのスタック</summary>
        public IReadOnlyList<int> ModalStack{get{return modalStack;}}
        
        /// <summary>モーダルが閉じたときの通知</summary>
        internal void OnCloseModal(ModalWindow modalWindow, Action OnCompleted)
        {
            CloseModalAsync(modalWindow, OnCompleted).Forget();
        }
        
        /// <summary>モーダルが閉じたときの通知</summary>
        internal UniTask OnCloseModalAsync(ModalWindow modalWindow)
        {
            return CloseModalAsync(modalWindow, null);
        }
        
        /// <summary>モーダルウィンドウを取得</summary>
        public ModalWindow GetModalWindow(int id)
        {
            if(modalList.TryGetValue(id, out ModalData data))
            {
                return data.modalWindow;
            }
            return null;
        }

        /// <summary>一番上のモーダル取得</summary>
        public ModalWindow GetTopModalWindow()
        {
            // モーダルなし
            if(modalStack.Count <= 0)return null;
            // 一番上のモーダル
            int id = modalStack[modalStack.Count-1];
            return modalList[id].modalWindow;
        }
        
        /// <summary>一番上のモーダルを閉じる</summary>
        public void CloseTopModalWindow()
        {
            ModalWindow topModalWindow = GetTopModalWindow();
            // モーダルなし
            if(topModalWindow == null)return;
            // 閉じる
            topModalWindow.Close();
        }

        /// <summary>スタックから削除する</summary>
        private void RemoveStack(ModalWindow modalWindow, bool changeState = true)
        {
            // ステート変更
            if(changeState)
            {
                modalWindow.state = ModalState.Removed;
            }
            // リストから削除
            modalList.Remove(modalWindow.ModalId);
            modalStack.Remove(modalWindow.ModalId);
            // 削除
            GameObject.Destroy(modalWindow.gameObject);
        }
        
        private void CheckModalList()
        {
            // すべて閉じた
            if(modalList.Count <= 0)
            {
                // 背景を閉じる
                if(modalBackground != null)
                {
                    modalBackground.gameObject.SetActive(false);
                }

                // 最後のモーダルが閉じた通知
                OnCloseLastModalWindow();
            }
        }
        
        protected void CallOnLoading(ModalLoadState state)
        {
            if(onLoading != null)
            {
                onLoading.Invoke(state);
            }
        }
        
        /// <summary>モーダルウィンドウを閉じて破棄する</summary>
        public virtual async UniTask CloseModalAsync(ModalWindow modalWindow, Action OnCompleted)
        {
            // 読み込み開始
            CallOnLoading(ModalLoadState.Begin);
            // 一番上のモーダル？
            bool isTopModal = GetTopModalWindow() == modalWindow;
            // 閉じる通知
            await modalWindow.OnCloseInternal();
            // スタックから削除
            RemoveStack(modalWindow, false);
            
            // 一番上のモーダルを閉じた場合は次のモーダルを開く
            if(isTopModal)
            {
                if( (modalWindow.Options & ModalOptions.KeepFrontModal) == ModalOptions.None)
                {
                    ModalWindow topModalWindow = GetTopModalWindow();
                    if(topModalWindow != null)
                    {
                        // ステート変更
                        topModalWindow.state = ModalState.Opened;
                        // アクティブ
                        topModalWindow.gameObject.SetActive(true);
                        await topModalWindow.OnActiveInternal();
                    }
                }
            }
            
            // モーダルリストのチェック
            CheckModalList();
            
            // 閉じたときの通知
            if(OnCompleted != null)
            {
                OnCompleted();
            }
            // 閉じたあとの通知
            await modalWindow.OnClosedInternal();
            
            // 読み込み中表示
            CallOnLoading(ModalLoadState.End);
            
            // ステートを変更
            modalWindow.state = ModalState.Closed;
        }
        
        /// <summary>手前のモーダルをスタックから消す。一番上のモーダルは除く</summary>
        public void RemoveTopModalsIgnoreTop(Func<ModalWindow, bool> getCloseFlag)
        {
            RemoveTopModals(getCloseFlag, 1);
        }
        
        /// <summary>手前のモーダルをスタックから消す</summary>
        public void RemoveTopModals(Func<ModalWindow, bool> getCloseFlag, int ignoreCount = 0)
        {
            IReadOnlyList<int> ids = ModalStack;
            // 開始位置
            int startIndex = ids.Count - 1 - ignoreCount;
            // 範囲外
            if(startIndex < 0 || startIndex >= ids.Count)return;
            
            for(int i = startIndex;i >= 0;i--)
            {
                // 一番手前のモーダルを取得
                ModalWindow modal = GetModalWindow(ids[i]);
                // 取得失敗
                if(modal == null)break;
                // 閉じるかチェック
                bool isClose = getCloseFlag(modal);
                // 閉じる場合
                if(isClose)
                {
                    RemoveStack(modal);
                }
                else
                {
                    break;
                }
            }
                        
            // モーダルリストのチェック
            CheckModalList();
        }

        public void RemoveModals(Func<ModalWindow, bool> getCloseFlag)
        {
            IReadOnlyList<int> ids = ModalStack;
            for(int i = ids.Count-1;i >= 0;i--)
            {
                // モーダルを取得
                ModalWindow modal = GetModalWindow(ids[i]);
                // 取得失敗
                if(modal == null)
                {
                    continue;
                }
                // 閉じるかチェック
                bool isClose = getCloseFlag(modal);
                // 閉じる場合
                if(isClose)
                {
                    RemoveStack(modal);
                }
            }
                        
            // モーダルリストのチェック
            CheckModalList();
        }

        /// <summary>最後のモーダルが閉じた時の通知</summary>
        protected virtual void OnCloseLastModalWindow()
        {
        }
    }
    
    public abstract class ModalManager<T> : ModalManager where T : System.Enum
    {

        private bool isRunOpen = false;

        /// <summary>ページの読み込み</summary>
        protected abstract UniTask<ModalWindow> OnLoadModalResource(T modal);
        
        public void RemoveAllModalWindow()
        {
            RemoveTopModals((m)=>true);
            isRunOpen = false;
        }
        
        /// <summary>モーダルウィンドウを開く</summary>
        public void OpenModal(T modal, object args, ModalOptions options = ModalOptions.None)
        {
            OpenModalAsync(modal, args, this.GetCancellationTokenOnDestroy(), options).Forget();
        }
        
        /// <summary>モーダルウィンドウを開く</summary>
        public UniTask<ModalWindow> OpenModalAsync(T modal, object args, ModalOptions options = ModalOptions.None)
        {
            return OpenModalAsync(modal, args, this.GetCancellationTokenOnDestroy(), options);
        }
       
        /// <summary>モーダルウィンドウを開く</summary>
        public virtual async UniTask<ModalWindow> OpenModalAsync(T modal, object args, CancellationToken token, ModalOptions options = ModalOptions.None)
        {
            // 読み込み開始
            CallOnLoading(ModalLoadState.Begin);
            
            // 同時に開く処理は許容しない
            await UniTask.WaitWhile(()=>isRunOpen);
            // 開く処理中に
            isRunOpen = true;
            
            // 既にモーダルを開いている場合は閉じる
            if( (options & ModalOptions.KeepFrontModal) == ModalOptions.None)
            {
                ModalWindow topModalWindow = GetTopModalWindow();
                if(topModalWindow != null) 
                {
                    // 閉じる通知
                    await topModalWindow.OnCloseInternal();
                    // ステート変更
                    topModalWindow.state = ModalState.Deactive;
                    // アクティブを切る
                    topModalWindow.gameObject.SetActive(false);
                }
            }

            // Modalの生成
            ModalWindow modalTemp = await OnLoadModalResource(modal);
            ModalWindow modalWindow = GameObject.Instantiate<ModalWindow>(modalTemp, modalRoot == null ? transform : modalRoot.transform);
            // Id
            int id = ++allocateModalId;
            modalWindow.Manager = this;
            modalWindow.Options = options;
            modalWindow.SetId(id);
            modalWindow.SetArguments(args);
            
            // リストに追加
            modalList.Add(id, new ModalData(modalWindow) );
            // リストに追加
            modalStack.Add(id);
            
            // 背景をアクティブ化
            if(modalBackground != null)
            {
                modalBackground.SetActive(true);
            }
            
            try
            {
                // モーダルを開くときの処理
                await modalWindow.OnPreOpenInternal();
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                if(modalWindow != null)
                {
                    await modalWindow.CloseAsync();
                    // ステート変更
                    modalWindow.state = ModalState.OpenError;
                }
                // 開く処理終わり
                isRunOpen = false;
                // 読み込み終了
                CallOnLoading(ModalLoadState.End);
                return modalWindow;
            }

            // アクティブ化
            modalWindow.gameObject.SetActive(true);
            // モーダルに通知
            await modalWindow.OnActiveInternal();
            
            // 開く処理終わり
            isRunOpen = false;
            // 読み込み終了
            CallOnLoading(ModalLoadState.End);
            
            // ステートを変更
            modalWindow.state = ModalState.Opened;
            
            // 開いたモーダルを返す
            return modalWindow;
        }
    }
}