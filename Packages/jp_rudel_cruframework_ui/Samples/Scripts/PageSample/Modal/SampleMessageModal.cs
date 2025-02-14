using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;

namespace CruFramework.Sample
{
    public class SampleMessageModal : Engine.UI.ModalWindow
    {
        [SerializeField]
        private TMPro.TMP_Text messageText = null;

        /// <summary>
        /// UGUI
        /// キャンセルボタン
        /// </summary>
        public void OnCanselButton()
        {
            // 閉じたときの値を設定
            SetCloseParameter(true);
            // 閉じる
            Close();
        }

        protected override UniTask OnPreOpen()
        {
            // 閉じたときに返す値
            SetCloseParameter(false);
            // 表示
            messageText.text = OpenArguments.ToString();
            return default;
        }
    }
}