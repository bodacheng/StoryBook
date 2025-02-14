

//////////////////////////////////////////////////////////
// サンプル用のルートページマネージャー
// ＊実装はアプリレイヤー側にマネージャーがあるのでそちらを使ってください
//////////////////////////////////////////////////////////


#if UNITY_EDITOR

using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Sample
{
    /// <summary>ルートページのタイプを定義</summary>
    public enum SampleRootPageType
    {
        Title, Home
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class SampleRootPageManager : Engine.UI.PageManager<SampleRootPageType>
    {
        private static SampleRootPageManager instance = null;
        /// <summary>Instance</summary>
        public static SampleRootPageManager Instance{get{return instance;}}

        [SerializeField]
        private Image fadeImage = null;
        
        private void Awake()
        {
            instance = this;
            
            // 最初のシーンを開く
            OpenPage(SampleRootPageType.Title, false, null);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        
        /// <summary>適当にフェード</summary>
        protected override async UniTask<PageFadeResult> OnFadeOut()
        {
            fadeImage.gameObject.SetActive(true);
            float alpha = 0;
            while(true)
            {  
                alpha = Mathf.Min(1, alpha + Time.deltaTime);
                fadeImage.color = new Color(0, 0, 0, alpha);
                await UniTask.DelayFrame(1);
                if(alpha >= 1.0f)break;
            }
            
            return PageFadeResult.Play;
        }

        /// <summary>適当にフェード</summary>
        protected override async UniTask<PageFadeResult> OnFadeIn()
        {
            await UniTask.Delay(500);
            
            float alpha = 1.0f;
            while(true)
            {  
                alpha = Mathf.Max(0, alpha - Time.deltaTime);
                fadeImage.color = new Color(0, 0, 0, alpha);
                await UniTask.DelayFrame(1);
                if(alpha <= 0)break;
            }
            fadeImage.gameObject.SetActive(false);
            return PageFadeResult.Play;
        }

        /// <summary>
        /// ページオブジェクトの読み込み
        /// 本実装ではアセットバンドル等からプレハブを読み込む
        /// </summary>
        protected override UniTask<Engine.UI.Page> OnLoadPageObject(SampleRootPageType page)
        {
            return new UniTask<Engine.UI.Page>( transform.Find(page.ToString()).GetComponent<Engine.UI.Page>() );
        }
    }
}

#endif // UNITY_EDITOR