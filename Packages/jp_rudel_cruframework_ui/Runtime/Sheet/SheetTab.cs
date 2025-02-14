using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Engine.UI
{
    public abstract class SheetTab : MonoBehaviour
    {
        [SerializeField]
        private Sheet sheet = null;
        [SerializeField]
        private bool isFirstSheet = false;
        
        private SheetManager sheetManager = null;
        
        protected virtual void OnOpenSheet(){}
        protected virtual void OnCloseSheet(){}
        
        private void Awake()
        {
            // 所属シート
            sheetManager = gameObject.GetComponentInParent<SheetManager>();
            // タブを登録
            sheetManager.RegisterTab(this);
            // 初期選択
            if(isFirstSheet)
            {
                SelectTab();
            }
            else
            {
                if(sheet != null)
                {
                    sheet.gameObject.SetActive(false);
                }
            }
            
            // ボタンが有る場合はイベント登録
            Button button = gameObject.GetComponent<Button>();
            if(button != null)
            {
                button.onClick.AddListener(SelectTab);
            }
        }
        
        internal void OnRegister()
        {
            OnCloseSheet();
        }
        
        /// <summary>シートを閉じる</summary>
        internal async UniTask OnCloseSheetAsync()
        {
            // シート指定がない
            if(sheet == null)
            {
                // 閉じる通知
                OnCloseSheet();
                return;
            }
            
            // OnClose
            await sheet.OnCloseInternal();
            // 閉じる通知
            OnCloseSheet();
            // アクティブ
            sheet.gameObject.SetActive(false);
        }
        
        /// <summary>シートを開く</summary>
        internal async UniTask OnOpenSheetAsync()
        {
            // シート指定がない
            if(sheet == null)
            {
                // 開いた通知
                OnOpenSheet();
                return;
            }
            
            // OnPreOpen
            await sheet.OnPreOpenInternal();
            // アクティブ
            sheet.gameObject.SetActive(true);
            // 開いた通知
            OnOpenSheet();
            // OnOpen
            await sheet.OnOpenInternal();
        }

        private void OnDestroy()
        {
            sheetManager.UnRegisterTab(this);
        }

        /// <summary>タブを選択</summary>
        public void SelectTab()
        {
            sheetManager.SelectTab(this);
        }
    }
}