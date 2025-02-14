using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Engine.UI
{
	
	public enum PageTransitionType
	{
		OpenPage, BackPage, NextPage
	}
	
	public abstract class PageManager : Page
	{
		protected Page currentPageObject = null;
		/// <summary>現在開いているページ</summary>
		public Page CurrentPageObject{get{return currentPageObject;}}
		
		private ulong openId = 0;
		/// <summary>OpenしたId（回数）</summary>
		internal ulong OpenId{get{return openId;}set{openId = value;}}


		internal override void CallSetActive(bool active)
		{
			base.CallSetActive(active);
			
			if(currentPageObject != null)
			{
				currentPageObject.CallSetActive(active);
			}
		}
		
		internal override async UniTask<PageResult> CallOnPreOpen()
		{
			Page page = currentPageObject;
			// 現在のId
			ulong currentId = openId;
			// 自身を開く
			PageResult result = await base.CallOnPreOpen();
			// 失敗
			if(result != PageResult.Success)return result;
			// 子ページなし
			if(page == null)return PageResult.Success;
			// 新しいページが開かれていた場合はここでは開かない
			if(currentId != openId)
			{
				return result;
			}
			
			// 遷移タイプを子供にも設定
			page.SetTransitionType(TransitionType);
			// 子供を開く
			result = await page.CallOnPreOpen();
			// ステートの変更
			page.state = PageState.Opened;
			
			return result;
		}

		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult> CallOnPreClose()
		{
			// 子供を閉じる
			if(currentPageObject != null)
			{
				PageResult result = await currentPageObject.CallOnPreClose();
				// 失敗
				if(result != PageResult.Success)return result;
			}
			// 自身を閉じる
			return await base.CallOnPreClose();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult> CallOnClosed()
		{
			if(currentPageObject != null)
			{
				// 子供を閉じる
				PageResult childResult = await currentPageObject.CallOnClosed();
				// 失敗
				if(childResult != PageResult.Success)return childResult;
				// ステート変更
				currentPageObject.state = PageState.Closed;
			}
			// 自身を閉じる
			PageResult result  = await base.CallOnClosed();
			// ステート変更
			state = PageState.Closed;
			return result;
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult> CallOnBeforeFadeOut()
		{
			// 自身
			PageResult result = await base.CallOnBeforeFadeOut();
			if(result != PageResult.Success)return result;
			
			if(currentPageObject == null)return PageResult.Success;
			// 子供
			return await currentPageObject.CallOnBeforeFadeOut();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult>  CallOnAfterFadeOut()
		{
			// 自身
			PageResult result = await base.CallOnAfterFadeOut();
			if(result != PageResult.Success)return result;
			
			if(currentPageObject == null)return PageResult.Success;
			// 子供
			return await currentPageObject.CallOnAfterFadeOut();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult> CallOnBeforeFadeIn()
		{
			// 自身
			PageResult result = await base.CallOnBeforeFadeIn();
			if(result != PageResult.Success)return result;
			
			if(currentPageObject == null)return PageResult.Success;
			// 子供
			return await currentPageObject.CallOnBeforeFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageResult> CallOnAfterFadeIn()
		{
			// 自身
			PageResult result = await base.CallOnAfterFadeIn();
			if(result != PageResult.Success)return result;
			
			if(currentPageObject == null)return PageResult.Success;
			// 子供
			return await currentPageObject.CallOnAfterFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageFadeResult> CallOnFadeIn()
		{
			// 自身
			PageFadeResult result = await base.CallOnFadeIn();
			// 再生した場合は返す
			if(result == PageFadeResult.Play)return result;
			
			if(currentPageObject == null)return PageFadeResult.None;
			// 子供
			return await currentPageObject.CallOnFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal override async UniTask<PageFadeResult> CallOnFadeOut(Page closePage)
		{
			// 自身
			PageFadeResult result = await base.CallOnFadeOut(closePage);
			// 再生した場合は返す
			if(result == PageFadeResult.Play)return result;
			// 子供
			if(closePage != null)
			{
				if(closePage is PageManager pageManager)
				{
					return await pageManager.CallOnFadeOut(pageManager.currentPageObject);
				}
				return await closePage.CallOnFadeOut(null);
			}
			return PageFadeResult.None;
		}
		
		/// <summary>戻る</summary>
		public virtual UniTask<PageResult> BackPageAsync(){return new UniTask<PageResult>(PageResult.Failed);}
		/// <summary>進む</summary>
		public virtual UniTask<PageResult> NextPageAsync(){return new UniTask<PageResult>(PageResult.Failed);}
		
		/// <summary>ページスタックの削除</summary>
		public virtual void ClearPageStackAll(){ }
		
		public virtual bool IsOpenState{get{return false;}}
		
	}
	
	public abstract class PageManager<T> : PageManager where T : Enum
	{
		/// <summary>ページスタック</summary>
		private class PageStackData
		{
			public object args = null;
			public T page;
			
			public PageStackData(T page, object args)
			{
				this.page = page;
				this.args = args;
			}
		}
		
		
		private T currentPageType = default;
		/// <summary>現在のページ</summary>
		public T CurrentPageType{get{return currentPageType;}}
		
		private T loadingPageType = default;
		/// <summary>読込中のページ</summary>
		public T LoadingPageType{get{return loadingPageType;}}
		
		
		private  bool isOpenState = false;
		/// <summary>開く処理中</summary>
		public override bool IsOpenState{get{return isOpenState;}}

		/// <summary>ページのキャッシュ</summary>
		private Dictionary<T, Page> pageObjectCache = new Dictionary<T, Page>();
		/// ページスタック
		private List<PageStackData> pageStack = new List<PageStackData>();
		
		// ページスタック位置
		private int pageStackIndex = 0;
		
		
		
		
		/// <summary>ページオブジェクトの読み込み</summary>
		protected abstract UniTask<Page> OnLoadPageObject(T page);
		
		/// <summary>ページが破棄されたときに呼び出す</summary>
		protected virtual void OnDestroyPageObject(T page){}
		/// <summary>子ページが閉じたときに呼ばれる</summary>
		protected virtual UniTask<PageResult> OnCloseChildPage(T page){return new UniTask<PageResult>(PageResult.Success);}
		/// <summary>子ページが開いたときに呼ばれる</summary>
		protected virtual UniTask<PageResult> OnOpenChildPage(T page){return new UniTask<PageResult>(PageResult.Success);}
		
		/// <summary>子ページが開いたときに呼ばれる</summary>
		protected virtual UniTask<PageResult> OnPreOpenChildPage(T page){return new UniTask<PageResult>(PageResult.Success);}
		
		/// <summary>スタックを追加</summary>
		public void AddPageStack(T page, object args)
		{
			pageStack.Add(new PageStackData(page, args));
			pageStackIndex++;
		}
		
		/// <summary>ページスタックの削除</summary>
		public void ClearPageStack()
		{
			pageStack.Clear();
			pageStackIndex = 0;
		}
		
		/// <summary>子ページを含むすべてのスタックを削除</summary>
		public override void ClearPageStackAll()
		{
			// スタックの削除
			ClearPageStack();
			// 子ページも削除
			foreach(Page page in pageObjectCache.Values)
			{
				if(page is PageManager m)
				{
					m.ClearPageStackAll();
				}
			}
		}
		
		/// <summary>PageObjectの取得</summary>
		public Page GetPageObject(T page)
		{
			if(pageObjectCache.TryGetValue(page, out Page result))
			{
				return result;
			}
			return null;
		}
		
		/// <summary>前のページへ</summary>
		public void BackPage()
		{
			BackPageAsync().Forget();
		}
		
		/// <summary>前のページへ</summary>
		public override async UniTask<PageResult> BackPageAsync()
		{
			// 子ページ
			if(currentPageObject is PageManager m)
			{
				// 子ページを戻す
				PageResult childResult = await m.BackPageAsync();
				// 子ページの戻るに成功した場合は返す
				if(childResult == PageResult.Success)return PageResult.Success;
			}
			
			// 前のページがない
			if(pageStackIndex <= 1)
			{
				return PageResult.Cancel;
			}
			
			// ページ移動したのでIndexを移動
			pageStackIndex--;
			// 前
			PageStackData stackData = pageStack[pageStackIndex-1];
			// ページを開く
			PageResult result = await OpenPageAsync(stackData.page, false, stackData.args, PageTransitionType.BackPage);
			// 成功以外
			if(result != PageResult.Success)
			{
				pageStackIndex++;
				return result;
			}


			return result;
		}
		
		/// <summary>次のページへ</summary>
		public void NextPage()
		{
			NextPageAsync().Forget();
		}
		
		/// <summary>次のページへ</summary>
		public override async UniTask<PageResult> NextPageAsync()
		{
			// 子ページ
			if(currentPageObject is PageManager m)
			{
				// 子ページを戻す
				PageResult childResult = await m.NextPageAsync();
				// 子ページの戻るに成功した場合は返す
				if(childResult == PageResult.Success)return PageResult.Success;
			}
			
			// 次のページがない
			if(pageStackIndex >= pageStack.Count)
			{
				return PageResult.Cancel;
			}

			// 次
			PageStackData stackData = pageStack[pageStackIndex];
			// ページ移動したのでIndexを移動
			pageStackIndex++;
			// ページを開く
			PageResult result = await OpenPageAsync(stackData.page, false, stackData.args, PageTransitionType.NextPage);
			// 成功以外
			if(result != PageResult.Success)
			{
				pageStackIndex--;
				return result;
			}

			return result;
		}
		
		/// <summary>現在開いてるページを閉じる</summary>
		public void CloseCurrentPage()
		{
			CloseCurrentPageAsync().Forget();
		}
		
		/// <summary>現在開いてるページを閉じる</summary>
		public async UniTask<PageResult> CloseCurrentPageAsync()
		{
			// ページを開いていない
			if(currentPageObject == null)return PageResult.Success;
			// 閉じる前の処理
			PageResult result = await currentPageObject.CallOnPreClose();
			// 閉じの失敗
			if(result != PageResult.Success)return result;
			// 閉じる処理
			result = await currentPageObject.CallOnClosed();
			if(result != PageResult.Success)return result;
			
			// ステート変更
			currentPageObject.state = PageState.Closed;
			// アクティブをOff
			currentPageObject.CallSetActive(false);
			// 現在のページをNull
			currentPageObject = null;
			
				
			return result;
		}

		/// <summary>キャッシュしているページオブジェクトを削除</summary>
		public void ClearPageObjectCache()
		{
			foreach(KeyValuePair<T, Page> pageObject in pageObjectCache)
			{
				GameObject.Destroy(pageObject.Value.gameObject);
				// ページが破棄された通知
				OnDestroyPageObject(pageObject.Key);
			}
			// リストの初期化
			pageObjectCache.Clear();
			// スタックの削除
			ClearPageStack();
		}
		
		
		/// <summary>ページの読み込み</summary>
		private async UniTask<Page> LoadPageObject(T page)
		{
			// ページを呼び出す
			if(pageObjectCache.TryGetValue(page, out Page pageObject) == false)
			{
				// ページの生成
				pageObject = await OnLoadPageObject(page);
				// マネージャー登録
				pageObject.SetPageManager(this);
				// キャッシュに登録
				pageObjectCache.Add(page, pageObject);
				// アクティブを切る
				pageObject.gameObject.SetActive(false);
			}
			
			return pageObject;
		}
		
		/// <summary>スタックの削除</summary>
		public void RemovePageStack(T page)
		{
			for(int i=pageStack.Count-1; i>=0; i--)
			{
				if(pageStack[i].page.Equals(page))
				{
					if(pageStackIndex > i)pageStackIndex--;
					pageStack.RemoveAt(i);
					break;
				}
			}
		}
		
		/// <summary>ページを開く</summary>
		public void OpenPage(T page, bool isAddPageStack, object args)
		{
			OpenPageAsync(page, isAddPageStack, args, PageTransitionType.OpenPage).Forget();
		}
		
		/// <summary>ページを開く</summary>
		public UniTask<PageResult> OpenPageAsync(T page, bool isAddPageStack, object args)
		{
			return OpenPageAsync(page, isAddPageStack, args, PageTransitionType.OpenPage);
		}
		
		/// <summary>ページを開く</summary>
		protected virtual async UniTask<PageResult> OpenPageAsync(T page, bool isAddPageStack, object args, PageTransitionType transitionType)
		{
			// 開く処理中にする
			isOpenState = true;
			// ページを開く
			PageResult result = await OpenPageAsyncCore(page, isAddPageStack, args, transitionType);
			
			// 処理中を解除
			isOpenState = false;
			
			return result;
		}

		/// <summary>ページを開く</summary>
		private async UniTask<PageResult> OpenPageAsyncCore(T page, bool isAddPageStack, object args, PageTransitionType transitionType)
		{
			// Id
			OpenId++;
			// この処理のId
			ulong currentOpenId = OpenId;
			
			// 同じページを開こうとしてる
			bool isSameOpenPage = false;
			// 同じページに移動しようとした場合
			if(currentPageObject != null && currentPageType.Equals(page))
			{
				isSameOpenPage = true;
			} 
			
			
			// 結果
			PageResult result = PageResult.Success;
			// 閉じるページ
			Page closePage = isSameOpenPage ? null : currentPageObject;
			
			// 閉じるページタイプ
			T closePageType = currentPageType;
			// 開いているページを閉じる
			if(closePage != null && closePage.state == PageState.Opened)
			{
				// 閉じる前の処理
				PageResult closeResult = await closePage.CallOnPreClose();
				// 失敗
				if(closeResult != PageResult.Success)
				{
					return closeResult;
				}
			}
			
			// 読込中のページ
			loadingPageType = page;
			
			// Openした場合不要なスタックを削除
			if(transitionType == PageTransitionType.OpenPage)
			{ 
				// 不要なスタックを削除
				pageStack.RemoveRange(pageStackIndex, pageStack.Count - pageStackIndex);
				pageStackIndex = pageStack.Count;
			}
			
			// 現在のページを保持していおく
			Page currentPageObjectTemp = currentPageObject;
			// ページ
			currentPageObject = await LoadPageObject(page);
			// 引数を登録
			currentPageObject.SetOpenArguments(args);
			// 遷移タイプ
			currentPageObject.SetTransitionType(transitionType);
			
			if(currentPageObject != null)
			{
				
				// Openした場合不要なスタックを削除
				if(transitionType == PageTransitionType.OpenPage)
				{ 
					if(currentPageObject is PageManager pagemanager)
					{
						pagemanager.ClearPageStackAll();
					}
				}
				
				// 通知
				result = await OnPreOpenChildPage(page);
				if(result != PageResult.Success)return result;
				
				// 開く前の処理
				result = await currentPageObject.CallOnPreOpen();
				// 成功以外
				if(result != PageResult.Success)
				{
					// 開いている途中のページを非アクティブに
					currentPageObjectTemp.gameObject.SetActive(false);
					// 現在のページ設定を戻す
					if(currentOpenId == OpenId)
					{
						currentPageObject = currentPageObjectTemp;
					}
					
					// ステート変更
					closePage.state = PageState.Idle;
					
					return result;
				}
				
				// ステート変更
				currentPageObject.state = PageState.Opened;

				// 親が開く処理中の場合はあとの処理は親に任せる
				if(Manager != null && Manager.IsOpenState)
				{
					currentPageType = page;
					// スタックに追加
					if(isAddPageStack)
					{
						AddPageStack(page, args);
					}

					return result;
				}
				
				// フェードアウト前
				result = await currentPageObject.CallOnBeforeFadeOut();
				if(result != PageResult.Success)return result;
			}
			
			// 前のページを閉じる
			if(closePage != null && closePage.state == PageState.Opened)
			{
				// フェードアウト
				await CallOnFadeOut(closePage);
				// 子ページを閉じた通知
				await OnCloseChildPage(closePageType);
			
				result = await closePage.CallOnClosed();
				if(result != PageResult.Success)return result;
				// アクティブをOff
				closePage.CallSetActive(false);
				// ページの破棄
				if(closePage.IsDestroyOnClosed)
				{
					// GameObject
					GameObject.Destroy(closePage.gameObject);
					// 通知
					OnDestroyPageObject(closePageType);
					// キャッシュから削除
					pageObjectCache.Remove(closePageType);
				}
			}
			
			// 現在のページ
			currentPageType = page;
			
			// 子ページを開いた通知
			await OnOpenChildPage(currentPageType);

			if(currentPageObject != null)
			{
				// アクティブをOn
				currentPageObject.CallSetActive(true);
				
				// フェードアウト後
				result = await currentPageObject.CallOnAfterFadeOut();
				if(result != PageResult.Success)return result;
				
				// フェードイン
				result = await currentPageObject.CallOnBeforeFadeIn();
				if(result != PageResult.Success)return result;
			}
			
			// フェードイン
			await CallOnFadeIn();
			
			if(currentPageObject != null)
			{
				// フェードイン
				result = await currentPageObject.CallOnAfterFadeIn();
				if(result != PageResult.Success)return result;
			}
			
			// スタックに追加
			if(isAddPageStack)
			{
				AddPageStack(page, args);
			}
			
			return result;
		}
	}
}