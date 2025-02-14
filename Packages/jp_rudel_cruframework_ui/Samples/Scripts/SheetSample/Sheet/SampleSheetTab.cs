
//////////////////////////////////
/// シートを切り替えるタブ
/// 基本的にはアプリレイヤーで共通化して使う想定
//////////////////////////////////


using CruFramework.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Sample
{
    public class SampleSheetTab : SheetTab
    {
        [SerializeField]
        private Image background = null;
        
        /// <summary>
        /// シートが開いときにタブの色を変える
        /// </summary>
        protected override void OnOpenSheet()
        {
            background.color = Color.green;
        }

        /// <summary>
        /// シートが閉じた時にタブの色を変える
        /// </summary>
        protected override void OnCloseSheet()
        {
            background.color = Color.yellow;
        }
    }
}