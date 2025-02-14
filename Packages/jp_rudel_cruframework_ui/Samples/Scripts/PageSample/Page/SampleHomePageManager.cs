

#if UNITY_EDITOR


using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Sample
{
    
    public enum HomePageType
    {
        Top, Sub
    }
    
    /// <summary>
    /// ホームページ
    /// カテゴリ等で分割したい場合はPageTypeを定義してPageManagerを継承する
    /// </summary>
    public class SampleHomePageManager : Engine.UI.PageManager<HomePageType>
    {
        
        [SerializeField]
        private Image fadeImage = null;
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OpenTopPage()
        {
            OpenPage(HomePageType.Top, true, null);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OpenSubPage()
        {
            OpenPage(HomePageType.Sub, true, null);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OpenTitlePage()
        {
            SampleRootPageManager.Instance.OpenPage(SampleRootPageType.Title, true, null);
        }
        
        /////////////////////////////////////////
        // ページマネージャーのイベント
        // 上から呼び出される順
        /////////////////////////////////////////
        
        /// <summary>
        /// ページを開く前の処理
        /// API等で画面に必要な情報を取得する
        /// </summary>
        protected override async UniTask<PageResult> OnPreOpen()
        {
            // 引数が必要な場合は OpenArguments をキャストして取得
            // int args = (int)OpenArguments;

            // 最初の少カテゴリのページを開く
            // ページイベントで少カテゴリのページを開けるタイミングはOnPreOpenのみ
            await OpenPageAsync(HomePageType.Top, true, null);
            
            
            // ページを開くのに成功
            return PageResult.Success;
            
            // エラー等でページを開くのに失敗
            // この場合マネージャー側のページを開く処理が中断されて今のページにとどまる
            // return PageResult.Failed;
        }


        /// <summary>
        /// フェードアウトエフェクトが終わったあとの処理
        /// 画面暗転中
        /// </summary>
        protected override UniTask<PageResult> OnAfterFadeOut()
        {
            return new UniTask<PageResult>(PageResult.Success);
        }

        /// <summary>
        /// フェードインエフェクトが終わったあとの処理
        /// ページが完全に表示されてからアニメーション再生等
        /// </summary>
        protected override UniTask<PageResult> OnAfterFadeIn()
        {
            return new UniTask<PageResult>(PageResult.Success);
        }


        /// <summary>
        /// ページが閉じられる前の処理
        /// </summary>
        protected override async UniTask<PageResult> OnPreClose()
        {
            // ページを離れようとしたときに確認モーダルを表示させてからページ遷移を行う
            // 編集中のデータがある場合に確認モーダルを表示させてからページ遷移する場合などに使う
            
            // モーダルを開く
            Engine.UI.ModalWindow modal = await SampleModalManager.Instance.OpenModalAsync( SampleModalType.Message, "Back to the title?" );
            // 閉じるまで待つ
            // このときにモーダルからパラメータを受け取ることが可能（モーダル側のSetParameterで登録した値が帰ってくる
            bool isCansel = (bool)await modal.WaitCloseAsync();
            
            // キャンセルされた場合
            if(isCansel)
            {
                // PageResult.Cancelを返すことでPageManagerのOpen処理が中断される
                return PageResult.Cancel;
            }
            
            // そのままページ遷移
            return PageResult.Success;
        }

        /// <summary>
        /// ページが閉じられた時の処理
        /// </summary>
        protected override UniTask<PageResult> OnClosed()
        {
            
            return new UniTask<PageResult>(PageResult.Success);
        }

        /////////////////////////////////////////
        // ここからは基本的にアプリレイヤーで共通化する
        /////////////////////////////////////////
        
        

        /// <summary>
        /// 小カテゴリページのフェードアウト演出
        /// </summary>
        protected override async UniTask<PageFadeResult> OnFadeOut()
        {
            fadeImage.gameObject.SetActive(true);
            float alpha = 0;
            while(true)
            {  
                alpha = Mathf.Min(1, alpha + Time.deltaTime * 2.0f);
                fadeImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
                await UniTask.DelayFrame(1);
                if(alpha >= 1.0f)break;
            }
            
            return PageFadeResult.Play;
        }

        
        /// <summary>
        /// 小カテゴリページのフェードイン演出
        /// </summary>
        protected override async UniTask<PageFadeResult> OnFadeIn()
        {
            await UniTask.Delay(500);
            
            float alpha = 1.0f;
            while(true)
            {  
                alpha = Mathf.Max(0, alpha - Time.deltaTime * 2.0f);
                fadeImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
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
        protected override UniTask<Engine.UI.Page> OnLoadPageObject(HomePageType page)
        {
            return new UniTask<Engine.UI.Page>( transform.Find(page.ToString()).GetComponent<Engine.UI.Page>() );
        }
    }
}

#endif // UNITY_EDITOR