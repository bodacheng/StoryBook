using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Engine.UI
{
	
	public enum PageResult
	{
		Success, Failed, Cancel
	}
	
	public enum PageFadeResult
	{
		None, Play
	}
	
	/// <summary>ページの状態</summary>
	public enum PageState
	{
		/// <summary>待機中</summary>
		Idle,
		/// <summary>開いた</summary>
		Opened,
		/// <summary>閉じた</summary>
		Closed
	}
	
	public abstract class Page : MonoBehaviour
	{
		
		[SerializeField]
		private bool isDestroyOnClosed = false;
		/// <summary>閉じるときにGameObjectを破棄する</summary>
		public bool IsDestroyOnClosed{get{return isDestroyOnClosed;}}
		
		private PageManager manager = null;
		/// <summary>ページマネージャー</summary>
		public PageManager Manager{get{return manager;}}
		
		private object openArguments = null;
		/// <summary>開いたときの引数</summary>
		public object OpenArguments{get{return openArguments;}}
		
		private PageTransitionType transitionType = PageTransitionType.OpenPage;
		/// <summary>繊維の種類</summary>
		public PageTransitionType TransitionType{get{return transitionType;}}
		
		internal PageState state = PageState.Idle;
		/// <summary>ページの状態</summary>
		public PageState State{get{return state;}}
		
		/// <summary>引数を登録</summary>
		public void SetOpenArguments(object args)
		{
			openArguments = args;
		}
		
		internal void SetPageManager(PageManager manager)
		{
			this.manager = manager;
		}
		
		internal void SetTransitionType(PageTransitionType type)
		{
			transitionType = type;
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnPreOpen()
		{
			return OnPreOpen();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnPreClose()
		{
			return OnPreClose();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnClosed()
		{
			return OnClosed();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnBeforeFadeOut()
		{
			return OnBeforeFadeOut();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnAfterFadeOut()
		{
			return OnAfterFadeOut();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnBeforeFadeIn()
		{
			return OnBeforeFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageResult> CallOnAfterFadeIn()
		{
			return OnAfterFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageFadeResult> CallOnFadeIn()
		{
			return OnFadeIn();
		}
		
		/// <summary>コールバック呼び出し</summary>
		internal virtual UniTask<PageFadeResult> CallOnFadeOut(Page closePage)
		{
			return OnFadeOut();
		}
		
		/// <summary>アクティブの変更</summary>
		internal virtual void CallSetActive(bool active)
		{
			gameObject.SetActive(active);
		}
		
		/// <summary>フェードアウト前</summary>
		protected virtual UniTask<PageResult> OnBeforeFadeOut()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>フェードアウト前</summary>
		protected virtual UniTask<PageResult> OnAfterFadeOut()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>フェードイン前</summary>
		protected virtual UniTask<PageResult> OnBeforeFadeIn()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>フェードイン後</summary>
		protected virtual UniTask<PageResult> OnAfterFadeIn()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>フェードイン</summary>
		protected virtual UniTask<PageFadeResult> OnFadeIn()
		{
			return new UniTask<PageFadeResult>(PageFadeResult.None);
		}
		
		/// <summary>フェードアウト</summary>
		protected virtual UniTask<PageFadeResult> OnFadeOut()
		{
			return new UniTask<PageFadeResult>(PageFadeResult.None);
		}
		
		/// <summary>ページを開く前の処理</summary>
		protected virtual UniTask<PageResult> OnPreOpen()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>ページを閉じる前の処理</summary>
		protected virtual UniTask<PageResult> OnPreClose()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
		
		/// <summary>ページを閉じたときの処理</summary>
		protected virtual UniTask<PageResult> OnClosed()
		{
			return new UniTask<PageResult>(PageResult.Success);
		}
	}
}