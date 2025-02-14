
///////////////////////////////
// 表示するアイテムの処理
//////////////////////////////

using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;

namespace CruFramework.Sample
{
    public class SampleScrollDynamicItem : ScrollDynamicItem
    {
        
        [SerializeField]
        private TMPro.TMP_Text indexText = null;
        
        /// <summary>
        /// ScrollDynamic::SetItemsに渡した値が入ってるのでそれを使って表示させる
        /// </summary>
        protected override void OnSetView(object value)
        {
            int index = (int)value;
            indexText.text = index.ToString();
        }
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClick()
        {
            // RectTransformのサイズを変更する
            RectTransform t = (RectTransform)transform;
            t.sizeDelta = new Vector2(t.sizeDelta.x, t.sizeDelta.y <= 200.0f ? 400.0f : 200.0f);
            
            // 変更した場合はこのメソッドを呼んでスクロールサイズを再計算する
            RecalculateSize();
        }
    }
}