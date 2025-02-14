using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CruFramework.Engine.UI
{
	public abstract class CruButton : UnityEngine.UI.Button
	{
		public enum LongTapTriggerType
		{
			None,
			Once,
			Repeat
		}
	
		[Serializable]
		public class ButtonLongTapEvent : UnityEvent {}
        
		[SerializeField]
		private LongTapTriggerType longTapTriggerMode = LongTapTriggerType.Once;
		/// <summary>長押しのトリガー</summary>
		public LongTapTriggerType LongTapTriggerMode{get{return longTapTriggerMode;}set{longTapTriggerMode = value;}}
		
		[SerializeField]
		private ButtonLongTapEvent onLongTap = new ButtonLongTapEvent();
		/// <summary>長押しイベント</summary>
		public ButtonLongTapEvent OnLongTap{get{return onLongTap;}}
        
		[SerializeField]
		private float longTapTriggerTime = 0.2f;
		/// <summary>長押しがトリガーする時間</summary>
		public float LongTapTriggerTime{get{return longTapTriggerTime;}set{longTapTriggerTime = value;}}
		
		[SerializeField]
		private float longTapTriggerRepeatInterval = 0.1f;
		/// <summary>長押しが繰り返しトリガーする時間</summary>
		public float LongTapTriggerRepeatInterval{get{return longTapTriggerRepeatInterval;}set{longTapTriggerRepeatInterval = value;}}
		
		[SerializeField]
		private float clickTriggerInterval = 0.0f;
		/// <summary>Clickのインターバル</summary>
		public float ClickTriggerInterval{get{return clickTriggerInterval;}set{clickTriggerInterval = value;}}
		
		
		[SerializeField]
		private CruEvent onClickEx = new CruEvent();
		/// <summary>クリック時のイベント</summary>
		public CruEvent OnClickEx{get{return onClickEx;}}
		
		[SerializeField]
		private CruEvent onLongTapEx = new CruEvent();
		/// <summary>ロングタップ時のイベント</summary>
		public CruEvent OnLongTapEx{get{return onLongTapEx;}}
		
		[SerializeField]
		private bool isTriggerOnce = false;
		/// <summary>一度のみイベントが発火する</summary>
		public bool IsTriggerOnce{get{return isTriggerOnce;}set{isTriggerOnce = value;}}
        
		// 長押しイベントがトリガーしたか
		private bool isLongTapTrigger = false;
		// 押している時間
		private float longTapTimer = 0;
		// 押しているか？
		private bool isPointerDown = false;
		// 押下間隔
		private float clickIntervalTimer = 0;
		
		private bool triggerOnce = false;
		
		protected virtual void OnTriggerLongTap(){}

		public override void OnPointerDown(PointerEventData e)
		{
			base.OnPointerDown(e);
			if(IsInteractable())
			{
				longTapTimer = 0;
				isPointerDown = true;
				isLongTapTrigger = false;
			}
		}
        
		public override void OnPointerUp(PointerEventData e)
		{
			base.OnPointerUp(e);
			isPointerDown = false;
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if(isTriggerOnce && triggerOnce)return;
			triggerOnce = true;
			base.OnPointerClick(eventData);
			
			if(eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
			{
				onClickEx.Invoke();
			}
			
			if(IsInteractable() && clickIntervalTimer <= 0)
			{
				// ボタン押下のインターバル加算
				clickIntervalTimer += clickTriggerInterval;
				// インターバルが設定されてる場合はinteractableを無効
				if(clickIntervalTimer > 0)
				{
					interactable = false;
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			triggerOnce = false;
		}
		
		protected virtual void LateUpdate()
		{
			if(clickIntervalTimer > 0)
			{
				clickIntervalTimer -= Time.deltaTime;
				// インターバルが終わったらinteractableを有効
				if(clickIntervalTimer <= 0)
				{
					interactable = true;
				}
			}
			
			if(isPointerDown)
			{
				bool isTrigger = false;
				
				switch(longTapTriggerMode)
				{
					case LongTapTriggerType.Once:
					{
						// すでに発火済み
						if(isLongTapTrigger)break;
						longTapTimer += Time.deltaTime;
						if(longTapTimer >= longTapTriggerTime)
						{
							isTrigger = true;
						}
						break;
					}
					
					case LongTapTriggerType.Repeat:
					{
						longTapTimer += Time.deltaTime;
						// 初回
						if(isLongTapTrigger == false)
						{
							if(longTapTimer >= longTapTriggerTime)
							{
								longTapTimer = 0;
								isTrigger = true;
							}
						}
						// 2回目以降
						else
						{
							if(longTapTimer >= longTapTriggerRepeatInterval)
							{
								longTapTimer -= longTapTriggerRepeatInterval;
								isTrigger = true;
							}
						}
						break;
					}
				}
				
				if(isTrigger)
				{
					isLongTapTrigger = true;
					onLongTap.Invoke();
					onLongTapEx.Invoke();
					OnTriggerLongTap();
				}
			}
		}

		public void ResetClickEvents()
		{
			// Hierarchyから登録したイベントはRemoveListenerで削除できないので根本から作り直し
			onClick = new ButtonClickedEvent();
			onClickEx.RemoveAllListeners();
		}
		
		public void ResetLongTapEvents()
		{
			onLongTap = new ButtonLongTapEvent();
			onLongTapEx.RemoveAllListeners();
		}
		
		/// <summary>クリックイベント数</summary>
		public int GetClickEventCount()
		{
			return onClick.GetPersistentEventCount() + onClickEx.Count;
		}
		
		/// <summary>ロングタップイベント数</summary>
		public int GetLongTapEventCount()
		{
			return onLongTap.GetPersistentEventCount() + onLongTapEx.Count;
		}
		
		/// <summary>ボタン押下カウントダウンをリセット</summary>
		public void SetClickIntervalTimer(float time)
		{
			clickIntervalTimer = time;
			interactable = time <= 0;
		}
	}
}