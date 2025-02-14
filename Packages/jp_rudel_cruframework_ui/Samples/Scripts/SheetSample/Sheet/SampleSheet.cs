

//////////////////////////////////
/// 表示するシートの処理
/// 基本的にはアプリレイヤーで共通化して使う想定
//////////////////////////////////

using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Sample
{
    public class SampleSheet : Sheet
    {
        [SerializeField]
        private CanvasGroup canvasGroup = null;


        /// <summary>
        /// 開くときにフェード処理等を行う
        /// </summary>
        protected override async UniTask OnOpen()
        {
            float alpha = 0;
            while(true)
            {
                alpha = Mathf.Min(1.0f, alpha + Time.deltaTime * 2.0f);
                canvasGroup.alpha = alpha;
                await UniTask.DelayFrame(1);
                if(alpha >= 1.0f)break;
            }
        }
        
        /// <summary>
        /// 開くときにフェード処理等を行う
        /// </summary>
        protected override async UniTask OnClose()
        {
            float alpha = 1.0f;
            while(true)
            {
                alpha = Mathf.Max(0, alpha - Time.deltaTime * 3.0f);
                canvasGroup.alpha = alpha;
                await UniTask.DelayFrame(1);
                if(alpha <= 0)break;
            }
        }
    }
}
