using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Engine.UI
{
    public abstract class ScrollDynamicItem : MonoBehaviour
    { 
        public RectTransform UITransform{get{return (RectTransform)transform;}}

        private int typeId = -1;
        /// <summary>種別Id</summary>
        public int TypeId{get{return typeId;}}
        
        private int viewIndex = -1;
        /// <summary>Viewの位置</summary>
        public int ViewIndex{get{return viewIndex;}}
        
        private ScrollDynamic scroll = null;
        /// <summary>親のスクロール</summary>
        public ScrollDynamic ParentScrollDynamic{get{return scroll;}}
        
        private bool isRecalculateSize = true;
        
        internal void Initialize(ScrollDynamic scrollDynamic, int typeId)
        {
            scroll = scrollDynamic;
            this.typeId = typeId;
        }

        internal void SetViewIndex(int index)
        {
            viewIndex = index;
        }
        
        internal void InitializeView(object value)
        {
            // Viewの初期化
            SetView(value);
            // サイズを変更
            if(scroll != null && viewIndex >= 0)
            {
                scroll.OnChangeSize(viewIndex, false);
            }
        }

        internal void SetView(object value)
        {
            OnSetView(value);
        }
        
        private void OnRectTransformDimensionsChange()
        {
            //RecalculateSize();
        }
        
        protected void RecalculateSize()
        {
            isRecalculateSize = true;
        }
        
        protected void OnEnable()
        {
            isRecalculateSize = true;
        }
        
        /// <summary>再計算</summary>
        public void ForceRecalculate()
        {
            ExecuteRecalculate();
        }
        
        private void ExecuteRecalculate()
        {
            if(isRecalculateSize)
            {
                if(scroll != null && viewIndex >= 0)
                {
                    scroll.OnChangeSize(viewIndex, true);
                }
                isRecalculateSize = false;
            }
        }

        private void LateUpdate()
        {
            ExecuteRecalculate();
        }

        protected abstract void OnSetView(object value);
    }
}