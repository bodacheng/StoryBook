
#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;

namespace CruFramework.Sample
{
    public class TextSample : MonoBehaviour
    {
        [SerializeField]
        private CruTextUGUI formatText = null;
        [SerializeField]
        private CruTextUGUI replaceText = null;


        private void Awake()
        {
            
            /////////////////////////////////////////////
            /// ルビ
            /// インスペクタのIsEnableRubyを有効にする必要がある
            /////////////////////////////////////////////

            
            /////////////////////////////////////////////
            /// フォーマット
            /////////////////////////////////////////////
            
            // インスペクタで設定したフォーマット文字列に対して値を流し込む
            // formatText == value = {0}
            formatText.SetFormatValues(12345);
            
            
            /////////////////////////////////////////////
            /// 置換処理
            /// インスペクタのIsEnableReplaceを有効にする必要がある
            /////////////////////////////////////////////
            
            // 文字列の置換を登録
            // CruTextUGUI.SetGlobalReplaceではすべてのテキストに適用される 個別のコンポーネントに適用する場合はSetLocalReplaceを使う
            // すでに読み込まれているTextに対しては効果なし
            // replaceText = {ReplaceTest:key=123,id=534}
            CruTextUGUI.SetGlobalReplace("ReplaceTest", (args)=>
            {
                return (int.Parse(args["key"]) * int.Parse(args["id"])).ToString();
            });
            
            // UpdateTextを呼ぶことで適用できる
            replaceText.UpdateText();
        }
    }
}

#endif