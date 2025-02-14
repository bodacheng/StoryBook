using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Engine.UI
{
    public abstract class SheetManager : MonoBehaviour
    {
        private List<SheetTab> tabList = new List<SheetTab>();
        /// <summary>登録されているタブ</summary>
        public IReadOnlyList<SheetTab> TabList{get{return tabList;}}
        
        private SheetTab selectedTab = null;
        /// <summary>選択中のタブ</summary>
        public SheetTab SelectedTab{get{return selectedTab;}}
        
        // 開く処理中
        private bool isRunOpenTask = false;
        
        /// <summary>タブの選択</summary>
        public void SelectTab(SheetTab tab)
        {
            OpenSheetAsync(tab).Forget();
        }
        
        /// <summary>タブの選択</summary>
        public UniTask SelectTabAsync(SheetTab tab)
        {
            return OpenSheetAsync(tab);
        }
        
        /// <summary>シートを開く</summary>
        private async UniTask OpenSheetAsync(SheetTab tab)
        {
            // 同じタブは開けない
            if(selectedTab == tab)return;
            // 開く処理中
            if(isRunOpenTask)return;
            // 開く処理中に
            isRunOpenTask = true;
            // 額ているシートを閉じる
            if(selectedTab != null)
            {
                await selectedTab.OnCloseSheetAsync();
            }
            // 選択中のタブ
            selectedTab = tab;
            // シートを開く
            await selectedTab.OnOpenSheetAsync();
            
            // 開く処理終了
            isRunOpenTask = false;
        }
        
        /// <summary>タブの登録</summary>
        internal void RegisterTab(SheetTab tab)
        {
            tabList.Add(tab);
            tab.OnRegister();
        }
        
        /// <summary>タブの登録</summary>
        internal void UnRegisterTab(SheetTab tab)
        {
            tabList.Remove(tab);
        }
    }
}