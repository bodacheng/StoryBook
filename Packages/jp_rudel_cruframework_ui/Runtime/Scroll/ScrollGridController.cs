using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Engine.UI
{
	public class ScrollGridController : MonoBehaviour
	{
		
		public enum ScrollType
		{
			Item, Page
		}
		
		public enum HideMode
		{
			None, Auto
		}
		
		[SerializeField]
		private ScrollGrid targetScrollGrid = null;
		/// <summary>捜査対象のスクロール</summary>
		public ScrollGrid TargetScrollGrid{get{return targetScrollGrid;}}
		
		[SerializeField]
		private ScrollType type = ScrollType.Page;

		[SerializeField]
		private HideMode hideMode = HideMode.Auto;

		[SerializeField]
		private CruButton prevButton = null;
	
		[SerializeField]
		private CruButton nextButton = null;
		
		
		private void Start()
		{
			if(targetScrollGrid != null)
			{
				targetScrollGrid.OnItemRefresh += OnRefresh;
				targetScrollGrid.onValueChanged.AddListener(OnChangeScroll);
				targetScrollGrid.OnChangedPage += OnChangePage;
				OnChangePage(0);
				OnChangeScroll(Vector2.zero);
			}
		}

		private void OnDestroy()
		{ 
			if(targetScrollGrid != null)
			{
				targetScrollGrid.OnItemRefresh -= OnRefresh;
				targetScrollGrid.onValueChanged.RemoveListener(OnChangeScroll);
				targetScrollGrid.OnChangedPage -= OnChangePage;
			}
		}
		
		public void UpdateButtons()
		{
			// ページスクロール
			if(targetScrollGrid.IsPaging)
			{
				UpdateButtonsPage();
			}
			// フリースクロール
			else
			{
				UpdateButtonsScroll();
			}
		}

		private void UpdateButtonsScroll()
		{
			// ループ設定の場合は処理しない
			if (targetScrollGrid.IsLoop) return;
			// ページの場合処理しない
			if (targetScrollGrid.IsPaging) return;

			switch (hideMode)
			{

				case HideMode.None:
					break;

				case HideMode.Auto:
				{
					// View内にスクロール範囲が収まってる
					if (targetScrollGrid.GetViewPortSize() >= targetScrollGrid.GetContentPortSize())
					{
						prevButton.gameObject.SetActive(false);
						nextButton.gameObject.SetActive(false);
						break;
					}

					// アイテムのスクロール量
					float itemScrollSize = targetScrollGrid.GetItemScrollValueNormalized();
					// 全体のスクロール量
					float scrollValue = targetScrollGrid.GetScrollValueNormalized();
					// 最初のページの場合は矢印を消す
					prevButton.gameObject.SetActive(scrollValue > itemScrollSize * 0.5f);
					// 最後のページの場合は矢印を消す
					nextButton.gameObject.SetActive(scrollValue < 1.0f - itemScrollSize * 0.5f);
					break;
				}
			}
		}
		
		private void UpdateButtonsPage()
		{
			// ページでない場合処理しない
			if(targetScrollGrid.IsPaging == false)return;
			
			switch(hideMode)
			{
			
				case HideMode.None:
					break;
			
				case HideMode.Auto:
				{
					// ページが１つしかない場合は非表示に
					if(targetScrollGrid.PageCount <= 1)
					{
						prevButton.gameObject.SetActive(false);
						nextButton.gameObject.SetActive(false);
					}
					else
					{
						// ループ時は表示
						if(targetScrollGrid.IsLoop)
						{
							prevButton.gameObject.SetActive(true);
							nextButton.gameObject.SetActive(true);
							break;
						}
						// 最初のページの場合は矢印を消す
						prevButton.gameObject.SetActive(targetScrollGrid.CurrentPage != 0);
						// 最後のページの場合は矢印を消す
						nextButton.gameObject.SetActive(targetScrollGrid.CurrentPage != targetScrollGrid.PageCount-1);
					}
					break;
				}
			}
		}
		
		private void OnChangeScroll(Vector2 value)
		{
			UpdateButtonsScroll();
		}

		private void OnChangePage(int page)
		{
			UpdateButtonsPage();
		}
		
		private void OnRefresh()
		{
			UpdateButtons();
		}

		/// <summary>進む</summary>
		public void Next()
		{
			switch(type)
			{
				case ScrollType.Page:
					targetScrollGrid.NextPage();
					break;
				case ScrollType.Item:
					targetScrollGrid.NextItem(1);
					break;
			}
		}
		
		/// <summary>戻る</summary>
		public void Prev()
		{
			switch(type)
			{
				case ScrollType.Page:
					targetScrollGrid.PrevPage();
					break;
				case ScrollType.Item:
					targetScrollGrid.NextItem(-1);
					break;
			}
		}
	}
}


