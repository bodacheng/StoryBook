using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;


    public class ScrollGrid : ScrollRect
    {
        
        private const float DragSnapDeadZone = 10.0f;
        
        public enum SelectModeType
        {
            /// <summary>選択できない</summary>
            None,
            /// <summary>一つだけ指定</summary>
            Single,
            /// <summary>複数選択</summary>
            Multiple,
        }

        public enum ScrollType
        {
            Horizontal, Vertical,
            HorizontalPage, VerticalPage,
        }
        
        public enum SpacingType
        {
            Auto, FixedSpace, FixedItemCount
        }
        
        public enum PagingSnapType
        {
            Nearest, DragDirection
        }
        
        public enum ItemAlignment
        {
            Horizontal, Vertical
        }
        
        private class ItemInfo
        {
            public ScrollGridItem item = null;
            public int listIndex = 0;
            public bool dirty = false;
        }
        
        [System.Serializable]
        public class GridInfo
        {
            [SerializeField]
            public SpacingType SpacingType = SpacingType.Auto;
            [SerializeField]
            public float Spacing = 2.0f;
            [SerializeField]
            public int ItemCount = 1;
        }
        
        [SerializeField]
        private bool interactable = true;
        /// <summary></summary>
        public bool Interactable{get{return interactable;}set{interactable = value;}}

       
        [SerializeField] private ScrollGridItem itemPrefab = null;
        /// <summary>表示する要素プレハブ</summary>
        public ScrollGridItem ItemPrefab{get{return itemPrefab;}set{itemPrefab = value;}}
        
        [SerializeField]
        private ScrollType type = ScrollType.Horizontal;
        /// <summary>スクロール方向</summary>
        public ScrollType Type{get{return type;}set{type = value;}}
        
        [SerializeField]
        private SelectModeType selectMode = SelectModeType.None;
        /// <summary>選択モード</summary>
        public SelectModeType SelectMode{get{return selectMode;}set{selectMode = value;}}


        [SerializeField]
        private ItemAlignment alignment = ItemAlignment.Horizontal;
        /// <summary>表示する要素の並び順</summary>
        public ItemAlignment Alignment{get{return alignment;}set{alignment = value;}}
        
        [SerializeField]
        private bool alignmentReverse = false;
        /// <summary>並び順の反転</summary>
        public bool AlignmentReverse{get{return alignmentReverse;}set{alignmentReverse = value;}}
        
        [SerializeField]
        private bool isLoop = false;
        /// <summary>コンテンツのループ</summary>
        public bool IsLoop{get{return isLoop;}set{isLoop = value;}}
        
        
        [SerializeField]
        private PagingSnapType pagingSnap = PagingSnapType.Nearest;
        /// <summary>ページのスナップ</summary>
        public PagingSnapType PagingSnap{get{return pagingSnap;}set{pagingSnap = value;}}
        
        [SerializeField]
        private float pagingAnimationDuration = 0.1f;
        /// <summary>ページングのアニメーション時間</summary>
        public float PagingAnimationDuration{get{return pagingAnimationDuration;}set{pagingAnimationDuration = value;}}
        
        [SerializeField]
        private int fixedPageItemCount = 0;
        /// <summary>ページのアイテム数</summary>
        public int FixedPageItemCount{get{return fixedPageItemCount;}set{fixedPageItemCount = value;}}
        
        [SerializeField]
        private GridInfo horizontalGridInfo = new GridInfo();
        /// <summary>横並びのグリッド</summary>
        public GridInfo HorizontalGridInfo{get{return horizontalGridInfo;}set{horizontalGridInfo = value;}}

        [SerializeField]
        private GridInfo verticalGridInfo = new GridInfo();
        /// <summary>縦並びのグリッド</summary>
        public GridInfo VerticalGridInfo{get{return verticalGridInfo;}set{verticalGridInfo = value;}}
        
        [SerializeField]
        private float marginLeft = 0;
        /// <summary>マージン</summary>
        public float MarginLeft{get{return marginLeft;}set{marginLeft = value;}}
        
        [SerializeField]
        private float marginRight = 0;
        /// <summary>マージン</summary>
        public float MarginRight{get{return marginRight;}set{marginRight = value;}}
        
        [SerializeField]
        private float marginBottom = 0;
        /// <summary>マージン</summary>
        public float MarginBottom{get{return marginBottom;}set{marginBottom = value;}}
        
        [SerializeField]
        private float marginTop = 0;
        /// <summary>マージン</summary>
        public float MarginTop{get{return marginTop;}set{marginTop = value;}}
        
        
        private object commonItemValue = null;
        /// <summary>共通データ</summary>
        public object CommonItemValue{get{return commonItemValue;}set{commonItemValue = value;}}
        
        /// <summary>ページ</summary>
        public bool IsPaging{get{return type == ScrollType.HorizontalPage || type == ScrollType.VerticalPage;}} 
        
        /// <summary>横スクロールできるか</summary>
        public bool CanHorizontalScroll{get{return type == ScrollType.Horizontal || type == ScrollType.HorizontalPage;}}
        /// <summary>縦スクロールできるか</summary>
        public bool CanVerticalScroll{get{return type == ScrollType.Vertical || type == ScrollType.VerticalPage;}}
        
        private int itemListOffset = 0;
        /// <summary>参照するアイテムリストのOffset</summary>
        public int ItemListOffset{get{return itemListOffset;}set{itemListOffset = value;}}
        
        private int currentPage = 0;
        /// <summary>表示中のページ</summary>
        public int CurrentPage{get{return CalcCurrentPage(currentPage);}}
        
        private List<int> selectedItemIndexList = new List<int>();
        /// <summary>選択中のアイテム</summary>
        public IReadOnlyList<int> SelectedItemIndexList{get{return selectedItemIndexList;}}

        // 計算用のページ数
        private int computePageCount = 0;
        
        private int pageCount = 0;
        /// <summary>ページ数</summary>
        public int PageCount{get{return pageCount;}}
        
        /// <summary>ページングアニメーション中</summary>
        public bool IsPagingAnimation{get{return pagingAnimationTime > 0;}}
        
        // アイテム
        private IList srcItems = null;
        private List<object> items = new List<object>();
        // スペース
        private Vector2 itemSpace = Vector2.zero;

        private Vector2 itemSize = Vector2.zero;
        /// <summary>サイズ</summary>
        public Vector2 ItemSize{get{return itemSize;}}
        
        private Vector2Int itemCount = Vector2Int.zero;
        /// <summary>アイテム数</summary>
        public Vector2Int ItemCount{get{return itemCount;}}

        private Vector2Int itemViewCount = Vector2Int.zero;
        // アイテムの行数
        private Vector2Int itemLineCount = Vector2Int.zero;
        
        // 現在表示中のアイテム位置
        private int currentViewItemIndex = 0;
        
        // アイテム情報
        private ItemInfo[] itemInfos = null;
        
        //　ページングのアニメーション位置
        private float pagingAnimationFrom = 0;
        private float pagingAnimationTo = 0;
        private float pagingAnimationTime = 0;
        
        // ドラッグ開始位置
        private Vector2 dragBeginPosition = Vector2.zero;
        
        /// <summary>アイテムのイベント</summary>
        public event Action<ScrollGridItem, object> OnItemEvent = null;
        /// <summary>アイテムの選択イベント</summary>
        public event Action<ScrollGridItem> OnSelectedItemEvent = null;
        /// <summary>アイテムの選択解除イベント</summary>
        public event Action<ScrollGridItem> OnDeselectedEvent = null;

        
        /// <summary>ページが変わったとき</summary>
        public event Action<int> OnChangedPage = null;
        
        /// <summary>ページ切り替えのアニメーションが始まった時</summary>
        public event Action<bool> OnBeginSlideAnimation = null;
        /// <summary>ページ切り替えのアニメーションが終わった時</summary>
        public event Action OnEndSlideAnimation = null;
        /// <summary>スナップ処理開始時</summary>
        public event Action OnBeginPageSnap = null;
        
        /// <summary>ドラッグ処理開始時</summary>
        public event Action OnBeginDragEvent = null;
        /// <summary>ドラッグ処理終了時</summary>
        public event Action OnEndDragEvent = null;
        
        /// <summary>リフレッシュ時</summary>
        public event Action OnItemRefresh = null;
        
        /// <summary>アニメーション再生中</summary>
        public bool IsPlayingAnimation{get{return IsPagingAnimation || pagingAnimationTime > 0;}}

        #region pjshin
        [SerializeField] private bool keepCurrentPageAtRefresh = false;
        public bool KeepCurrentPageAtRefresh{get{return keepCurrentPageAtRefresh;}set{keepCurrentPageAtRefresh = value;}}
        #endregion

        /// <summary>計算用のマージン</summary>
        private float GetCalcMarginLeft()
        {
            if(IsPaging)return 0;
            return marginLeft;
        }
        
        /// <summary>計算用のマージン</summary>
        private float GetCalcMarginRight()
        {
            if(IsPaging)return 0;
            return marginRight;
        }
        
        /// <summary>計算用のマージン</summary>
        private float GetCalcMarginBottom()
        {
            if(IsPaging)return 0;
            return marginBottom;
        }
        
        /// <summary>計算用のマージン</summary>
        private float GetCalcMarginTop()
        {
            if(IsPaging)return 0;
            return marginTop;
        }
        
        /// <summary>アイテムリストの登録</summary>
        public void SetItems(IList list)
        {
            srcItems = list;
            Refresh();
        }
        
        /// <summary>リストの更新</summary>
        public void Refresh(bool isResetScrollPosition = true)
        {
            // 選択を解除
            DeselectAllItems();
            // スクロール位置を取得
            float scrollPosition = GetScrollValueNormalized();
            // アニメーション初期化
            pagingAnimationTime = 0;
            
            // 生成済みのオブジェクトを破棄
            if(itemInfos != null)
            {
                foreach(ItemInfo item in itemInfos)
                {
                    GameObject.Destroy(item.item.gameObject);
                }
                itemInfos = null;
            }
            
            // スクロール方向
            horizontal = CanHorizontalScroll;
            vertical   = CanVerticalScroll;
            
            // スクロール時のイベント登録
            onValueChanged.RemoveListener(OnScrollValueChanged);
            onValueChanged.AddListener( OnScrollValueChanged );

            // アイテムサイズを計算
            CalcItemSize();
            
            // アイテムリスト生成
            items.Clear();
            
            if(srcItems.Count > 0)
            {
                while(true)
                {
                    foreach(object item in srcItems)
                    {
                        items.Add(item);                
                    }
                    
                    // ループ設定の場合は１ページ分うまるまでリストを複製する
                    if(isLoop == false || items.Count > itemViewCount.x * itemViewCount.y)
                    {
                        break;
                    }
                }
            }
            
            if (!keepCurrentPageAtRefresh)
            {
                // ページ数初期化
                currentPage = 0;
            }

            int pageItemViewCount = (itemViewCount.x * itemViewCount.y);
            // ページ数
            int pageViewCount = (items.Count - 1) / pageItemViewCount + 1;
            
            // 全体のページ数
            if(IsPaging)
            {
                // 自動計算
                if(fixedPageItemCount <= 0)
                {
                    pageCount = (srcItems.Count - 1) / pageItemViewCount + 1;;
                    computePageCount = (items.Count - 1) / pageItemViewCount + 1;
                }
                // 指定
                else
                {
                    switch(type)
                    {
                        case ScrollType.Vertical:
                        case ScrollType.VerticalPage:
                            computePageCount = (items.Count - 1) / (itemViewCount.x * fixedPageItemCount) + 1;
                            pageCount = (srcItems.Count - 1) / (itemViewCount.x * fixedPageItemCount) + 1;
                            break;
                        case ScrollType.Horizontal:
                        case ScrollType.HorizontalPage:
                            computePageCount = (items.Count - 1) / (itemViewCount.y * fixedPageItemCount) + 1;
                            pageCount = (srcItems.Count - 1) / (itemViewCount.y * fixedPageItemCount) + 1;
                            break;
                    }
                }
            }
            else
            {
                pageCount = 0;
            }
            
            // ループ設定の時は無限スクロールに変更
            if(isLoop)
            {
                movementType = MovementType.Unrestricted;
            }
            
            // アイテムの表示行数
            switch(type)
            {
                case ScrollType.Horizontal:
                {
                    switch(alignment)
                    {
                        case ItemAlignment.Horizontal:
                            itemLineCount.x = items.Count / pageItemViewCount * itemViewCount.x + (items.Count % pageItemViewCount >= itemViewCount.x ? itemViewCount.x : items.Count % pageItemViewCount);
                            itemLineCount.y = itemCount.y;
                            break;
                        case ItemAlignment.Vertical:
                            itemLineCount.x = items.Count / itemCount.y + (items.Count % itemCount.y > 0 ? 1 : 0);
                            itemLineCount.y = itemCount.y;
                            break;
                    }
                    break;
                }
                
                case ScrollType.HorizontalPage:
                {
                    if(fixedPageItemCount <= 0)
                    {
                        itemLineCount.x = pageViewCount * itemViewCount.x;
                    }
                    else
                    {
                        itemLineCount.x = (items.Count / pageItemViewCount) * itemViewCount.x + ((items.Count % pageItemViewCount) >= itemViewCount.x ? itemViewCount.x : (items.Count % pageItemViewCount));
                    }
                    itemLineCount.y = itemCount.y;
                    break;
                }

                case ScrollType.Vertical:
                {
                    switch(alignment)
                    {
                        case ItemAlignment.Horizontal:
                            itemLineCount.x = itemCount.x;
                            itemLineCount.y = items.Count / itemCount.x + (items.Count % itemCount.x > 0 ? 1 : 0);
                            break;
                        case ItemAlignment.Vertical:
                            itemLineCount.x = itemCount.x;
                            itemLineCount.y = items.Count / pageItemViewCount * itemViewCount.y + (items.Count % pageItemViewCount >= itemViewCount.y ? itemViewCount.y : items.Count % pageItemViewCount);
                            break;
                    }

                    break;
                }
                
                case ScrollType.VerticalPage:
                {
                    itemLineCount.x = itemCount.x;

                    if(fixedPageItemCount <= 0)
                    {
                        itemLineCount.y = pageViewCount * itemViewCount.y;
                    }
                    else
                    {
                        itemLineCount.y = (items.Count / pageItemViewCount) * itemViewCount.y + ((items.Count % pageItemViewCount) >= itemViewCount.y ? itemViewCount.y : (items.Count % pageItemViewCount));
                    }
                    
                    break;
                }
            }
            
            // ScrollRectのContentサイズ計算
            content.sizeDelta = new Vector2(itemSize.x * itemLineCount.x + itemSpace.x * (itemLineCount.x + 1) + GetCalcMarginLeft() + GetCalcMarginRight(), itemSize.y * itemLineCount.y  + itemSpace.y * (itemLineCount.y  + 1) + GetCalcMarginBottom() + GetCalcMarginTop());
            
            // 配列の生成
            itemInfos = new ItemInfo[itemCount.x * itemCount.y];
            // 一度に表示する分のアイテムを生成
            for(int i=0;i<itemInfos.Length;i++)
            {
                ItemInfo item = new ItemInfo();
                
                // プレハブのインスタンス
                item.item = GameObject.Instantiate<ScrollGridItem>(itemPrefab, content);
                // ScrollGridを登録
                item.item.SetScrollGrid(this);
                // index
                int listIndex = i;
                int infoIndex = i;
                
                switch(type)
                {
                    case ScrollType.Horizontal:
                    case ScrollType.HorizontalPage:
                    {
                        switch(alignment)
                        {
                            case ItemAlignment.Horizontal:
                                listIndex = i % itemViewCount.y * itemViewCount.x + i / pageItemViewCount * pageItemViewCount + i % pageItemViewCount / itemViewCount.y;
                                infoIndex = i;
                                break;
                            case ItemAlignment.Vertical:
                                listIndex = i;
                                infoIndex = i;
                                break;
                        }
                        break;
                    }
                    case ScrollType.Vertical:
                    case ScrollType.VerticalPage:
                    {
                        switch(alignment)
                        {
                            case ItemAlignment.Horizontal:
                                listIndex = i;
                                infoIndex = i;
                                break;
                            case ItemAlignment.Vertical:
                                
                                listIndex = i % itemViewCount.x * itemViewCount.y + i / pageItemViewCount * pageItemViewCount + i % pageItemViewCount / itemViewCount.x;
                                infoIndex = i;
                                break;
                        }
                        break;
                    }
                }

                // Index
                item.listIndex = listIndex;
                // ItemInfo
                itemInfos[infoIndex] = item;

                // 非表示にしておく
                item.item.gameObject.SetActive(false);
            }
            
            // アイテムの座標を再計算
            RecalculationItemPosition();
            
            // 表示位置を初期化
            content.transform.localPosition = Vector3.zero;
            horizontalNormalizedPosition = 0;
            verticalNormalizedPosition = 1.0f;
            currentViewItemIndex = 0;
            
            if (isResetScrollPosition == false)
            {
                // スクロール位置を戻す
                SetScrollValueNormalized(scrollPosition, true);
            }

            int viewItemCount = itemCount.x * itemCount.y;
            // ファーストビュー分表示
            if(isLoop)
            {
                for(int i=0;i<itemInfos.Length;i++)
                {
                    SetItemView(itemInfos[i], itemInfos[i].listIndex);
                }
            }
            else
            {
                for(int i=0;i<items.Count && i < viewItemCount;i++)
                {
                    SetItemView(itemInfos[i], itemInfos[i].listIndex);
                }
            }
            
            // 通知
            if(OnItemRefresh != null)
            {
                OnItemRefresh();
            }
        }
        
        /// <summary>スクロール位置の設定</summary>
        public void SetScrollValueNormalized(float value, bool immediatelyCallEvent = false)
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                case ScrollType.Horizontal:
                    horizontalNormalizedPosition = value;
                    break;
        
                case ScrollType.VerticalPage:
                case ScrollType.Vertical:
                    verticalNormalizedPosition = value;
                    break;
            }
            
            // 即座にイベント呼び出し
            if(immediatelyCallEvent)
            {
                // イベントを手動で呼び出しておく
                OnScrollValueChanged(new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition));
            }
        }
        
        /// <summary>アイテムのサイズを計算</summary>
        public void CalcItemSize()
        {
            if(itemPrefab == null)return;
            
            // 表示領域のサイズ
            Vector2 viewportSize = viewport.rect.size;
            
            // アイテムのサイズ
            itemSize.x = itemPrefab.Size.x * itemPrefab.UITransform.localScale.x;
            itemSize.y = itemPrefab.Size.y * itemPrefab.UITransform.localScale.y;
            
            // 表示するアイテム個数
            itemViewCount.x = GetItemCount(horizontalGridInfo, viewportSize.x - (CanHorizontalScroll == false ? (GetCalcMarginLeft() + GetCalcMarginRight()) : 0), itemSize.x, out itemSpace.x);
            itemViewCount.y = GetItemCount(verticalGridInfo, viewportSize.y - (CanVerticalScroll == false ? (GetCalcMarginBottom() + GetCalcMarginTop()) : 0), itemSize.y, out itemSpace.y);
            
            // 必要なアイテム個数
            int addItemCount = 1 + (isLoop ? 1 : 0);
            itemCount.x = itemViewCount.x + (CanHorizontalScroll ? addItemCount : 0);
            itemCount.y = itemViewCount.y + (CanVerticalScroll   ? addItemCount : 0);
        }
        
        /// <summary>表示の更新</summary>
        public void RefreshItemView(bool includeInactive = false)
        {
            if(itemInfos == null)return;
            foreach(ItemInfo item in itemInfos)
            {
                if(item.item.gameObject.activeInHierarchy || includeInactive)
                {
                    SetItemView(item, item.listIndex);
                }
            }
        }

        public List<ScrollGridItem> GetItemViews(bool includeInactive = false)
        {
            var ret = new List<ScrollGridItem>();
            foreach(ItemInfo item in itemInfos)
            {
                if(item.item.gameObject.activeInHierarchy || includeInactive)
                {
                    ret.Add(item.item);
                }
            }

            return ret;
        }

        /// <summary>表示位置</summary>
        public Vector3 GetItemPositionByIndex(int index)
        {
            int x = 0;
            int y = 0;
            
            // Pivot
            Vector2 itemPivot = itemPrefab.UITransform.pivot;
            
            switch(type)
            {
                case ScrollType.Horizontal:
                case ScrollType.HorizontalPage:
                {
                    x = index / itemCount.y;
                    y = index % itemCount.y;
                    break;
                }
                    
                case ScrollType.Vertical:
                case ScrollType.VerticalPage:
                {
                    x = index % itemCount.x;
                    y = index / itemCount.x;
                    break;
                }
            }
            
            // 座標を計算
            float px = x * itemSize.x + (x + 1) * itemSpace.x + GetCalcMarginLeft();
            float py = y * itemSize.y + (y + 1) * itemSpace.y + GetCalcMarginTop();
            // Pivotを考慮
            px += itemSize.x * itemPivot.x;
            py += itemSize.y * (1.0f - itemPivot.y);
            return new Vector3(px, -py, 0);
        }
        
        private void RecalculationItemPosition()
        {
            for(int i=0;i<itemInfos.Length;i++)
            {
                ItemInfo item = itemInfos[i];
                item.item.UITransform.localPosition = GetItemPositionByIndex(i);
            }
        }
        
        public void NextItem(int moveCount)
        {
            // スクロール時間
            pagingAnimationTime = pagingAnimationDuration;
            
            // アニメーション開始点
            switch(type)
            {
                case ScrollType.Horizontal:
                    pagingAnimationFrom = horizontalNormalizedPosition;
                    break;
                case ScrollType.Vertical:
                    pagingAnimationFrom = verticalNormalizedPosition;
                    break;
            }

            // スクロール量
            float value = GetItemScrollValueNormalized();
            // アニメーション終着点
            pagingAnimationTo = ((int)( (pagingAnimationFrom + value * 0.5f) / value) + moveCount) * value;
        }
        
        /// <summary>次のページ</summary>
        public void NextPage()
        {
            SetPage(currentPage + 1);
        }
        
        /// <summary>前のページ</summary>
        public void PrevPage()
        {
            SetPage(currentPage - 1);
        }
        
        public void PrevPageWithoutOnBeginSlideAnimation()
        {
            SetPage(currentPage - 1, true, false);
        }
        
        private float GetPageScrollValueNormalized()
        {
            if(fixedPageItemCount <= 0)
            {
                if(computePageCount == 1)return 1.0f;
                return (1.0f / (float)(computePageCount - 1));
            }

            switch(type)
            {
                case ScrollType.HorizontalPage:
                    if(computePageCount == itemViewCount.x)return 1.0f;
                    return (1.0f / (float)(computePageCount - itemViewCount.x));
                case ScrollType.VerticalPage:
                    if(computePageCount == itemViewCount.y)return 1.0f;
                    return (1.0f / (float)(computePageCount - itemViewCount.y));
            }
            
            return 0;
        }
        
        /// <summary>アイテム１つ分のスクロール量</summary>
        public float GetItemScrollValueNormalized()
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                case ScrollType.Horizontal:
                    if(itemViewCount.x >= itemLineCount.x)return 1.0f;
                    return 1.0f / (itemLineCount.x - itemViewCount.x);
                 
                case ScrollType.VerticalPage:   
                case ScrollType.Vertical:
                    if(itemViewCount.y >= itemLineCount.y)return 1.0f;
                    return 1.0f / (itemLineCount.y - itemViewCount.y);
            }
            
            return 0;
        }
        
        /// <summary>スクロール量</summary>
        public float GetScrollValueNormalized()
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                case ScrollType.Horizontal:
                    return horizontalNormalizedPosition;

                case ScrollType.VerticalPage:
                case ScrollType.Vertical:
                    return verticalNormalizedPosition;
            }
            
            return 0;
        }
        
        public float GetViewPortSize()
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                case ScrollType.Horizontal:
                    return viewport.rect.width;

                case ScrollType.VerticalPage:
                case ScrollType.Vertical:
                    return viewport.rect.height;
            }
            
            return 0;
        }
        
        public float GetContentPortSize()
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                case ScrollType.Horizontal:
                    return content.rect.width;

                case ScrollType.VerticalPage:
                case ScrollType.Vertical:
                    return content.rect.height;
            }
            
            return 0;
        }

        /// <summary>現在のページを取得</summary>
        private int CalcCurrentPage(int page)
        {
            if(isLoop)
            {
                if(page >= PageCount)
                {
                    page = page % PageCount;
                }
                else if(page < 0)
                {
                    page = (PageCount - 1) + (page + 1) % PageCount;
                }
            }
            
            return page;
        }
        
        /// <summary>ページセット</summary>
        public void SetPage(int page, bool isAnimation = true, bool invokeOnBeginSlideAnimation = true)
        {
            // 範囲外
            if(isLoop == false && (page < 0 || page >= PageCount) )return;
            
            // ページ登録
            var isSamePage = currentPage == page;
            currentPage = page;
            
            if(isAnimation)
            {
                // スクロール時間
                pagingAnimationTime = pagingAnimationDuration;
                // アニメーション終着点
                pagingAnimationTo = page * GetPageScrollValueNormalized();
                
                // アニメーション開始点
                switch(type)
                {
                    case ScrollType.HorizontalPage:
                        pagingAnimationFrom = horizontalNormalizedPosition;
                        break;
                    case ScrollType.VerticalPage:
                        pagingAnimationFrom = verticalNormalizedPosition;
                        break;
                }
                
                if(invokeOnBeginSlideAnimation && OnBeginSlideAnimation != null)
                {
                    OnBeginSlideAnimation(isSamePage);
                }
            }
            else
            {
                // アニメーション開始点
                switch(type)
                {
                    case ScrollType.HorizontalPage:
                    {
                        float current = horizontalNormalizedPosition;
                        horizontalNormalizedPosition = page * GetPageScrollValueNormalized();
                        // 前回と同じ値になるとコールバックが呼ばれないので手動で呼び出す
                        OnScrollValueChanged(new Vector2(horizontalNormalizedPosition - current, 0));
                        break;
                    }
                    case ScrollType.VerticalPage:
                    {
                        float current = verticalNormalizedPosition;
                        verticalNormalizedPosition = page * GetPageScrollValueNormalized();
                        // 前回と同じ値になるとコールバックが呼ばれないので手動で呼び出す
                        OnScrollValueChanged(new Vector2(0, verticalNormalizedPosition - current));
                        break;
                    }
                }
                
                // ページ変更通知
                if(OnChangedPage != null)
                {
                    OnChangedPage(currentPage);
                }
            }
        }
        
        public ScrollGridItem GetItem(object key)
        {
            foreach(ItemInfo item in itemInfos)
            {
                int index = ToItemListIndex(item.listIndex);
                if(index < 0 || index >= items.Count)continue;
                if(items[index].Equals(key))
                {
                    return item.item;
                }
            }
            return null;
        }
        
        /// <summary>アイテムリスト取得</summary>
        public IList GetItems()
        {
            return items;
        }
        
        /// <summary>アイテムのイベントを発火</summary>
        internal void TriggerItemEvent(ScrollGridItem item, object value)
        {
            if(OnItemEvent != null)
            {
                OnItemEvent(item, value);
            }
        }
        
        /// <summary>アイテムの選択イベントを発火</summary>
        internal void TriggerSelectedItemEvent(ScrollGridItem item)
        {
            if(OnSelectedItemEvent != null)
            {
                OnSelectedItemEvent(item);
            }
        }
        
        /// <summary>アイテムの選択解除イベントを発火</summary>
        internal void TriggerDeselectedItemEvent(ScrollGridItem item)
        {
            if(OnDeselectedEvent != null)
            {
                OnDeselectedEvent(item);
            }
        }
        
        private void MoveItemHorizontal(int lineCount)
        {
            // 移動方向
            int sign = lineCount > 0 ? 1 : -1;
            // 絶対値
            lineCount = Mathf.Abs(lineCount);
            
            for(int i=0;i<lineCount;i++)
            {
                for(int n=0;n<itemCount.y;n++)
                {
                    // アイテム情報Index
                    int currentItemIndex = currentViewItemIndex < 0 ? currentViewItemIndex % itemCount.x : currentViewItemIndex; 
                    int infoIndex = (currentItemIndex + itemCount.x + i * sign + (sign < 0 ? itemCount.x-1 : 0) ) * itemCount.y + n;
                    // アイテム情報
                    var itemIndex = Math.Clamp(infoIndex % itemInfos.Length, 0, itemInfos.Length - 1);
                    ItemInfo item = itemInfos[itemIndex];
                    // Index
                    item.listIndex += GetScrollItemIndexOffset(currentItemIndex + i * sign, sign);
                    // 座標調整
                    item.item.UITransform.localPosition += new Vector3(itemCount.x * (itemSize.x + itemSpace.x) * sign, 0, 0);
                    // 変更フラグ
                    item.dirty = true;
                }
            }
            
            // Indexを記録
            currentViewItemIndex += lineCount * sign;
            // 表示を更新
            UpdateItemView();
        }
        
        private void OnHorizontalScroll(float value)
        {
            // スクロール量から今のアイテム表示位置を計算する
            int index = (int)( (value - GetCalcMarginLeft()) / (itemSize.x + itemSpace.x));
            if(value < 0)index--;
            
            // スクロール位置が変わった
            if(index == currentViewItemIndex)return;
            
            // 移動した量
            int line = index - currentViewItemIndex;
            // 移動させる
            MoveItemHorizontal(line);
        }
        
        private void MoveItemVertical(int lineCount)
        {
            // 移動方向
            int sign = lineCount > 0 ? 1 : -1;
            // 絶対値
            lineCount = Mathf.Abs(lineCount);

            for(int i=0;i<lineCount;i++)
            {
                for(int n=0;n<itemCount.x;n++)
                {
                    // アイテム情報Index
                    int currentItemIndex = currentViewItemIndex < 0 ? currentViewItemIndex % itemCount.y : currentViewItemIndex; 
                    // アイテム情報Index
                    int infoIndex = (currentItemIndex + itemCount.y + i * sign + (sign < 0 ? itemCount.y-1 : 0) ) * itemCount.x + n;
                    // アイテム情報
                    var itemIndex = Math.Clamp(infoIndex % itemInfos.Length, 0, itemInfos.Length - 1);
                    ItemInfo item = itemInfos[itemIndex];
                    // Index
                    item.listIndex += GetScrollItemIndexOffset(currentItemIndex + i * sign, sign);
                    // 座標調整
                    item.item.UITransform.localPosition += new Vector3(0, itemCount.y * (itemSize.y + itemSpace.y) * -sign, 0);
                    // 変更フラグ
                    item.dirty = true;
                }
            }
            // Indexを記録
            currentViewItemIndex += lineCount * sign;
            // 表示を更新
            UpdateItemView();
        }
        
        private void OnVerticalScroll(float value)
        {
            // スクロール量から今のアイテム表示位置を計算する
            int index = (int)( (value - GetCalcMarginTop()) / (itemSize.y + itemSpace.y));
            if(value < 0)index--;
            // スクロール位置が変わった
            if(index == currentViewItemIndex)return;
            
            // 移動した量
            int line = index - currentViewItemIndex;
            // アイテムの移動
            MoveItemVertical(line);

        }
        
        /// <summary>スクロールしたとき</summary>
        private void OnScrollValueChanged(Vector2 e)
        {
            switch(type)
            {
                case ScrollType.Horizontal:
                case ScrollType.HorizontalPage:
                    OnHorizontalScroll( e.x * (content.rect.size.x - viewport.rect.size.x) );
                    break;
                case ScrollType.Vertical:
                case ScrollType.VerticalPage:
                    OnVerticalScroll( (1.0f - e.y) * (content.rect.size.y - viewport.rect.size.y) );
                    break;
            }
        }

        private void PagingSnapNearest()
        {
            // 1ページのスクロール量
            float v = GetPageScrollValueNormalized();
            
            switch(type)
            {
                case ScrollType.HorizontalPage:
                {
                    float p = (horizontalNormalizedPosition + (v * 0.5f)) / v;
                    SetPage( (int)p - (p < 0 ? 1 : 0) );
                    break;
                }
                case ScrollType.VerticalPage:
                {
                    float p = (verticalNormalizedPosition + (v * 0.5f)) / v;
                    SetPage( (int)p - (p < 0 ? 1 : 0) );
                    break;
                }
            }
            
            if(OnBeginPageSnap != null)
            {
                OnBeginPageSnap();
            }
        }
        
        private void PagingSnapDragDirection(Vector2 beginPos, Vector2 endPos)
        {
            switch(type)
            {
                case ScrollType.HorizontalPage:
                {
                    // ドラッグ方向
                    float x = beginPos.x - endPos.x;
                    // デッドゾーン
                    if(Mathf.Abs(x) <= DragSnapDeadZone)
                    {
                        SetPage(currentPage);
                    }
                    else
                    {
                        SetPage( currentPage + (int)(x / Mathf.Abs(x))) ;
                        
                        if(OnBeginPageSnap != null)
                        {
                            OnBeginPageSnap();
                        }
                        
                    }
                    break;
                }
                case ScrollType.VerticalPage:
                {
                    // ドラッグ方向
                    float y = beginPos.y - endPos.y;
                    // デッドゾーン
                    if(Mathf.Abs(y) <= DragSnapDeadZone)
                    {
                        SetPage(currentPage);
                    }
                    else
                    {
                        SetPage( currentPage + (int)(y / Mathf.Abs(y))) ;
                        
                        if(OnBeginPageSnap != null)
                        {
                            OnBeginPageSnap();
                        }
                        
                    }
                    break;
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            
            // アニメーション中
            if(IsPagingAnimation)return;
            // ページタイプかつ１つしかない場合はスクロール不可
            if(IsPaging && PageCount <= 1)return;
            
            switch(pagingSnap)
            {
                case PagingSnapType.Nearest:
                    PagingSnapNearest();
                    break;
                case PagingSnapType.DragDirection:
                    PagingSnapDragDirection(dragBeginPosition, eventData.position);
                    break;
            }

            
            base.OnEndDrag(eventData);
            
            // イベント
            if(OnEndDragEvent != null)
            {
                OnEndDragEvent.Invoke();
            }
        }
        
        /// <summary>選択中のアイテムを解除</summary>
        public void DeselectAllItems()
        {
            if(selectedItemIndexList.Count > 0)
            {
                foreach(ItemInfo itemInfo in itemInfos)
                {
                    if(selectedItemIndexList.Contains(itemInfo.item.ItemId))
                    {
                        itemInfo.item.Deselect();
                    }
                }
                selectedItemIndexList.Clear();
            }
        }
        
        /// <summary>アイテムの選択</summary>
        public void SelectItem(ScrollGridItem item)
        {
            if(item == null)
            {
                return;
            }
            SelectItem(item.ItemId);
        }
        
        /// <summary>アイテムの選択</summary>
        public void SelectItem(int index)
        {
            switch(selectMode)
            {
                // 選択しない
                case SelectModeType.None:
                    return;
                
                case SelectModeType.Single:
                { 
                    // 選択中のものを解除
                    DeselectAllItems();
                    // リストに追加
                    selectedItemIndexList.Add(index);

                    foreach(ItemInfo item in itemInfos)
                    {
                        if(item.listIndex == index)
                        {
                            item.item.Selected();
                        }
                    }
                    
                    break;
                }
                
                case SelectModeType.Multiple:
                {
                    // 選択？
                    bool isSelect = selectedItemIndexList.Contains(index) == false;
                    
                    if(isSelect)
                    {
                        // リストに追加
                        selectedItemIndexList.Add(index);
                    }
                    else
                    {
                        // リストから削除
                        selectedItemIndexList.Remove(index);
                    }
                    
                    foreach(ItemInfo item in itemInfos)
                    {
                        if(item.listIndex == index)
                        {
                            if(isSelect)
                            {
                                item.item.Selected();
                            }
                            else
                            {
                                item.item.Deselect();
                            }
                        }
                    }
                    
                    break;
                }
            }
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if(interactable == false)return;
            // アニメーション中
            if(IsPagingAnimation)return;
            // ループかつ１つしかない場合はスクロール不可
            if(isLoop && PageCount <= 1)return;

            dragBeginPosition = eventData.position;
            base.OnBeginDrag(eventData);
            
            // イベント
            if(OnBeginDragEvent != null)
            {
                OnBeginDragEvent.Invoke();
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if(interactable == false)return;
            if(IsPagingAnimation)return;
            base.OnDrag(eventData);
        }
        
        private int GetScrollItemIndexOffset(int index, int sign)
        {
            switch(type)
            {
                case ScrollType.Horizontal:
                case ScrollType.HorizontalPage:
                {
                    // listのIndex計算
                    switch(alignment)
                    {
                        case ItemAlignment.Horizontal:
                        {
                            
                            // 移動数
                            int v = ((itemViewCount.x * itemViewCount.y) * (itemCount.x / itemViewCount.x) + itemCount.x % itemViewCount.x);
                            // 計算する位置
                            int target = sign > 0 ? index : index + itemCount.x - 1;
                            
                            // ページをまたぐとき
                            int page = (target / itemViewCount.x) + (target < 0 ? -1 : 0);
                            int nextPage = (target + itemCount.x * sign) / itemViewCount.x + ((target + itemCount.x * sign) < 0 ? -1 : 0);
                            
                            if( Mathf.Abs(nextPage - page) > 1)
                            {
                                v += (itemViewCount.x * (itemViewCount.y - 1));
                            }
                            
                            return v * sign;
                        }
                        case ItemAlignment.Vertical:
                        {
                            return (itemCount.x * itemCount.y) * sign;
                        }
                    }
                    
                    break;
                }
                
                case ScrollType.Vertical:
                case ScrollType.VerticalPage:
                {
                    
                    // listのIndex計算
                    switch(alignment)
                    {
                        case ItemAlignment.Horizontal:
                        {
                            return (itemCount.x * itemCount.y) * sign;
                        }
                        case ItemAlignment.Vertical:
                        {
                            // 移動数
                            int v = ((itemViewCount.x * itemViewCount.y) * (itemCount.y / itemViewCount.y) + itemCount.y % itemViewCount.y);
                            // 計算する位置
                            int target = sign > 0 ? index : index + itemCount.y - 1;
                            
                            // ページをまたぐとき
                            int page = (target / itemViewCount.y) + (target < 0 ? -1 : 0);
                            int nextPage = (target + itemCount.y * sign) / itemViewCount.y + ((target + itemCount.y * sign) < 0 ? -1 : 0);
                            
                            if( Mathf.Abs(nextPage - page) > 1)
                            {
                                v += (itemViewCount.y * (itemViewCount.x - 1));
                            }
                            
                            return v * sign;
                        }
                    }
                    
                    break;
                }
            }
            return 0;
        }
        
        private void UpdateItemView()
        {
            foreach(ItemInfo item in itemInfos)
            {
                if(item.dirty == false)continue;
                SetItemView(item, item.listIndex);
                item.dirty = false;
            }
        }
        
        private int ToItemListIndex(int index)
        {
            index += itemListOffset;
            // ループ時
            if(isLoop)
            {
                if(index >= items.Count)
                {
                    index = index % items.Count;
                }
                else if(index < 0)
                {
                    index = items.Count - 1 + (index + 1) % items.Count;
                }
            }
            
            // index反転
            if(alignmentReverse)
            {
                index = items.Count - index - 1;
            }
            
            return index;
        }
        
        /// <summary>アイテムの表示</summary>
        private void SetItemView(ItemInfo itemInfo, int itemIndex)
        {
            itemIndex = ToItemListIndex(itemIndex);
            
            // リスト外の場合は表示を消す
            if(itemIndex < 0 || itemIndex >= items.Count)
            {
                itemInfo.item.gameObject.SetActive(false);
                return;
            }

            // 表示通知
            itemInfo.item.SetView( items[itemIndex], itemIndex );
            
            // 選択状態
            if(selectMode != SelectModeType.None)
            {
                if(selectedItemIndexList.Contains(itemIndex))
                {
                    itemInfo.item.Selected();
                }
                else
                {
                    itemInfo.item.Deselect();
                }
            }
            
            // 表示On
            itemInfo.item.gameObject.SetActive(true);
        }
        
        /// <summary>表示するアイテム数の計算</summary>
        private int GetItemCount(GridInfo grid, float viewSize, float itemSize, out float space)
        {
            // スペース計算方法
            SpacingType scapingType = IsPaging && grid.SpacingType == SpacingType.FixedSpace ? SpacingType.Auto : grid.SpacingType;
            
            switch(scapingType)
            {
                case SpacingType.Auto:
                {
                    int itemCount = (int)(viewSize / itemSize);
                    space = (float)(viewSize - (itemSize * itemCount)) / (float)(itemCount + 1);
                    return Mathf.Max(1, itemCount);
                }
                case SpacingType.FixedSpace:
                {
                    space = grid.Spacing;
                    int itemCount = (int)(viewSize / (itemSize + space)) + 1;
                    return Mathf.Max(1, itemCount);
                }
                case SpacingType.FixedItemCount:
                {
                    int itemCount = grid.ItemCount;
                    space = (float)(viewSize - (itemSize * itemCount)) / (float)(itemCount + 1);
                    return Mathf.Max(1, itemCount);
                }
            }
            
            space = 0;
            return 0;
        }
        
        private void UpdatePagingAnimation()
        {
            if(pagingAnimationTime <= 0)return;
            // 時間
            pagingAnimationTime -= Time.deltaTime;
            // 0未満にはしない
            if(pagingAnimationTime <= 0)
            {
                pagingAnimationTime = 0;
                
                // アニメーション終わった通知
                if(OnEndSlideAnimation != null)
                {
                    OnEndSlideAnimation();
                }
                
                // ページ変更通知
                if(OnChangedPage != null && IsPaging)
                {
                    OnChangedPage( CalcCurrentPage(currentPage) );
                }
            }
            
            switch(type)
            {
                case ScrollType.Horizontal:
                case ScrollType.HorizontalPage:
                    horizontalNormalizedPosition = GetPageScrollValue(pagingAnimationFrom, pagingAnimationTo, pagingAnimationDuration, pagingAnimationDuration - pagingAnimationTime);
                    break;
                    
                case ScrollType.Vertical:
                case ScrollType.VerticalPage:
                    verticalNormalizedPosition = GetPageScrollValue(pagingAnimationFrom, 1.0f - pagingAnimationTo, pagingAnimationDuration, pagingAnimationDuration - pagingAnimationTime);
                    break;
            }
        }
        
        private float GetPageScrollValue(float from, float to, float duration, float currentTime)
        {
            float t = currentTime / duration - 1.0f;
            return from + (to - from) * (t * t * t + 1.0f);
        }
        
        /// <summary>ItemIndexの位置までスクロール</summary>
        public void ScrollToItemIndex(int index)
        {
            // アイテムの座標を取得
            Vector3 itemPosition = GetItemPositionByIndex(index);
            itemPosition.x -= (itemSpace.x + itemSize.x * 0.5f);
            itemPosition.y += (itemSpace.y + itemSize.y * 0.5f);
            Vector2 size = content.rect.size - viewport.rect.size;
            
            switch(type)
            {
                case ScrollType.Vertical:
                    verticalNormalizedPosition = Mathf.Clamp(1.0f + (itemPosition.y / size.y), 0, 1.0f);
                    break;
                case ScrollType.Horizontal:
                    horizontalNormalizedPosition = Mathf.Clamp((itemPosition.x / size.x), 0, 1.0f);
                    break;
            }
        }
        
        protected override void LateUpdate()
        {
            base.LateUpdate();
            UpdatePagingAnimation();
        }

    }

