
using UnityEngine;

#if UNITY_EDITOR

namespace CruFramework.Sample
{
    /// <summary>
    /// タイトルページ
    /// </summary>
    public class SampleTitlePageManager : Engine.UI.Page
    {
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnTapStart()
        {
            // ホームページを開く
            SampleRootPageManager.Instance.OpenPage( SampleRootPageType.Home, true, null );
        }
    }
}

#endif // UNITY_EDITOR