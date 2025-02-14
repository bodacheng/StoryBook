using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CruFramework.Engine.UI
{
    public abstract class ScrollGridItem : MonoBehaviour
    {
        // RectTransform
        public RectTransform UITransform{get{return (RectTransform)transform;}}
        // Size
        public Vector2 Size{get{return UITransform.rect.size;}}
        
        private ScrollGrid scrollGrid = null;
        /// <summary>親のスクロール</summary>
        public ScrollGrid ParentScrollGrid{get{return scrollGrid;}}
        
        private int itemId = -1;
        /// <summary>Id</summary>
        public int ItemId{get{return itemId;}}
        
        /// <summary>イベントの発火</summary>
        protected void TriggerEvent(object value)
        {
            scrollGrid.TriggerItemEvent(this, value);
        }
        
        public void SelectItem()
        {
            scrollGrid.SelectItem(this);
        }
        
        /// <summary>ScrollGridの登録</summary>
        internal void SetScrollGrid(ScrollGrid scrollGrid)
        {
            this.scrollGrid = scrollGrid;
        }
        
        /// <summary>Viewの設定</summary>
        protected abstract void OnSetView(object value);
        
        /// <summary>Viewの設定</summary>
        internal void SetView(object value, int id)
        {
            itemId = id;
            OnSetView(value);
        }
        
        /// <summary>選択解除</summary>
        protected virtual void OnDeselectItem(){}
        
        /// <summary>選択解除</summary>
        internal void Deselect()
        {
            OnDeselectItem();
            scrollGrid.TriggerDeselectedItemEvent(this);
        }
        
        /// <summary>選択</summary>
        protected virtual void OnSelectedItem(){}
        
        /// <summary>選択</summary>
        internal void Selected()
        {
            OnSelectedItem();
            scrollGrid.TriggerSelectedItemEvent(this);
        }
    }
}