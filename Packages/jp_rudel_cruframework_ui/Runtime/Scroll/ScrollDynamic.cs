
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Engine.UI
{
    public class ScrollDynamic : ScrollRect
    {
        private class ItemCache
        {
            public List<ScrollDynamicItem> items = new List<ScrollDynamicItem>();
        }
        
        private class ItemInfo
        {
            public int itemIndex = 0;
            public int viewIndex = 0;
            public ScrollDynamicItem item = null;
            public Vector2 position = Vector2.zero;
            public Vector2 size = Vector2.zero;
            
            public ItemInfo(ScrollDynamicItem item, int index, int viewIndex, Vector2 position, Vector2 size)
            {
                this.item = item;
                itemIndex = index;
                this.position = position;
                this.size = size;
                this.viewIndex = viewIndex;
            }
        }
        
        [SerializeField]
        private ScrollDynamicItemSelector itemSelector = null;
        /// <summary>アイテムセレクタ</summary>
        public ScrollDynamicItemSelector ItemSelector{get{return itemSelector;}set{itemSelector = value;}}
        
        [SerializeField]
        private float spacing = 2.0f;
        /// <summary>スペース</summary>
        public float Spacing{get{return spacing;}set{spacing = value;}}
        
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
        
        private IList itemList = null;
        /// <summary>アイテムリスト</summary>
        public IList ItemList{get{return itemList;}}
        
        /// <summary>アイテムのキャッシュ</summary>
        private Dictionary<int, ItemCache> itemCaches = new Dictionary<int, ItemCache>();
        /// <summary>アイテムの種類別Id</summary>
        private Dictionary<ScrollDynamicItem, int> itemTypeIds = new Dictionary<ScrollDynamicItem, int>();
        
        // 表示しているアイテムリスト
        private List<ItemInfo> viewItemList = new List<ItemInfo>();
        
        private Vector2 latestScrollValue = Vector2.zero;
        
        private int beginViewItemListIndex = 0;
        private int endViewItemListIndex = 0;
        
        /// <summary>アイテムの登録</summary>
        public void SetItems(IList items)
        {
            itemList = items;
            Refresh();
        }
        
        /// <summary>表示の更新</summary>
        public void Refresh()
        {
            // キャッシュを削除
            foreach(KeyValuePair<int, ItemCache> cache in itemCaches)
            {
                foreach(ScrollDynamicItem item in cache.Value.items)
                {
                    GameObject.Destroy(item.gameObject);
                }
                cache.Value.items.Clear();
            }
            
            itemCaches.Clear();
            itemTypeIds.Clear();

            // 表示リストクリア
            viewItemList.Clear();
            
            // スクロール量をリセット
            horizontalNormalizedPosition = 0;
            verticalNormalizedPosition = 1.0f;
            
            latestScrollValue = Vector2.zero;

            // contentのRectTransform
            RectTransform contentTransform = (RectTransform)content.transform;
            // スクロール位置初期化
            contentTransform.anchoredPosition = Vector2.zero;
            // Contentサイズ初期化
            contentTransform.sizeDelta = CalcContentSize();
            
            // 変数初期化
            beginViewItemListIndex = 0;
            endViewItemListIndex = 0;
            
            // スクロール時のイベント登録
            onValueChanged.RemoveListener(OnScrollValueChanged);
            onValueChanged.AddListener( OnScrollValueChanged );
            
            // 初期アイテム生成
            CreateFirstViewItems();
        }
        
        
        private void CreateFirstViewItems()
        {
            if(vertical)
            {
                float height = 0;
                // 現在のアイテムのサイズ
                foreach(ItemInfo currentItem in viewItemList)
                {
                    height += currentItem.item.UITransform.rect.height + spacing;
                }
                
                // 初期生成
                while(true)
                {
                    // Viewのサイズを超えるかすべてのアイテムを生成した
                    if(height >= viewRect.rect.height || viewItemList.Count >= itemList.Count)
                    {
                        break;
                    }
                    
                    // アイテム生成
                    ScrollDynamicItem item = CreateNewItem();
                    // 高さ
                    height += item.UITransform.rect.height + spacing;
                }
            }
            else if(horizontal)
            {
                float width = 0;
                
                // 現在のアイテムのサイズ
                foreach(ItemInfo currentItem in viewItemList)
                {
                    width += currentItem.item.UITransform.rect.height + spacing;
                }
                
                // 初期生成
                while(true)
                {
                    // Viewのサイズを超えるかすべてのアイテムを生成した
                    if(width >= viewRect.rect.width || viewItemList.Count >= itemList.Count)
                    {
                        break;
                    }
                    
                    // アイテム生成
                    ScrollDynamicItem item = CreateNewItem();
                    // 幅
                    width += item.UITransform.rect.width + spacing;
                    
                }
            }
        }
        
        /// <summary>
        /// アイテムの追加
        /// SetItemsで渡すデータがListである必要がある
        /// </summary>
        public void AddItem(object item)
        {
            // リストに追加
            itemList.Add(item);
            // 最初のアイテムの場合は初期化
            if(itemList.Count == 1)
            {
                CreateNewItem();
            }
            // スクロール更新
            else 
            {
                CheckScrollItem(latestScrollValue, new Vector2(1.0f, 1.0f));   
            }
        }
        
        /// <summary>
        /// アイテムの追加
        /// SetItemsで渡すデータがListである必要がある
        /// </summary>
        public void AddItemRange(IList items)
        {
            // リストに追加
            foreach(object value in items)
            {
                itemList.Add(value);
            }
            
            // アイテムの生成
            CreateFirstViewItems();
            // スクロールのチェック
            CheckScrollItem(latestScrollValue, new Vector2(1.0f, 1.0f));   
        }
        
        /// <summary>指定位置までスクロール</summary>
        public void ScrollTo(float normalizedPosition, bool immediatelyCallEvent = false)
        {
            if(vertical)
            {
                verticalNormalizedPosition = normalizedPosition;
            }
            else if(horizontal)
            {
                horizontalNormalizedPosition = normalizedPosition;
            }
            
            if(immediatelyCallEvent)
            {
                // イベントを手動で呼び出しておく
                OnScrollValueChanged(new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition));
            }
        }
        
        /// <summary>最初の位置までスクロール</summary>
        public void ScrollToFirst(bool immediatelyCallEvent = false)
        {
            // アイテムなし
            if(itemList.Count <= 0)return;
            
            if(vertical)
            {
                ScrollTo(1.0f, immediatelyCallEvent);
            }
            else if(horizontal)
            {
                ScrollTo(0, immediatelyCallEvent);
            }
        }
        
        /// <summary>終わりまでスクロール</summary>
        public void ScrollToEnd(bool immediatelyCallEvent = false)
        {
            // アイテムなし
            if(itemList.Count <= 0)return;
            
            // 領域の計算
            ForceRecalculate();

            
            if(vertical)
            {
                ScrollTo(0, immediatelyCallEvent);
            }
            else if(horizontal)
            {
                ScrollTo(1.0f, immediatelyCallEvent);
            }
        }
        
        /// <summary>強制的にすべて読み込む</summary>
        public void ForceLoadAll()
        {
            // サイズ計算
            ForceRecalculate();
            // ビューポートのサイズ
            Vector2 viewportSize = viewport.rect.size;
            
            if(vertical)
            {
                // 初期生成
                while(true)
                {
                    // Viewのサイズを超えるかすべてのアイテムを生成した
                    if(viewItemList.Count >= itemList.Count)
                    {
                        break;
                    }
                    
                    // アイテム生成
                    ScrollDynamicItem item = CreateNewItem(false);
                    item.ForceRecalculate();

                    // 範囲外の場合は非表示に
                    if(item.UITransform.anchoredPosition.y + spacing < -viewportSize.y)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            else if(horizontal)
            {
                // 初期生成
                while(true)
                {
                    // Viewのサイズを超えるかすべてのアイテムを生成した
                    if(viewItemList.Count >= itemList.Count)
                    {
                        break;
                    }
                    
                    // アイテム生成
                    ScrollDynamicItem item = CreateNewItem(false);
                    item.ForceRecalculate();
                    
                    // 範囲外の場合は非表示に
                    if(item.UITransform.anchoredPosition.x + spacing > viewportSize.x)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        /// <summary>すべてのビューの更新</summary>
        public void RefreshItemView()
        {
            for(int i=beginViewItemListIndex;i<=endViewItemListIndex;i++)
            {
                ItemInfo item = viewItemList[i];
                // ビューの更新
                item.item.SetView(itemList[item.itemIndex]);
            }                
        }
        
        
        /// <summary>Contentのサイズ取得</summary>
        private Vector2 CalcContentSize()
        {
            RectTransform contentTransform = (RectTransform)content.transform;
            Vector2 contentSize = contentTransform.sizeDelta;
            if(horizontal)
            {
                contentSize.x = marginLeft + marginRight;
            }
            if(vertical)
            {
                contentSize.y = marginTop + marginBottom;
            }
            return contentSize;
        }
        
        /// <summary>TransformのAnchorとPivotの初期化</summary>
        private void InitializeAnchorAndPivot(RectTransform target)
        {
            // アンカーとピボットを変更
            if(vertical)
            {
                // Pivotを上基準に
                target.pivot = new Vector2(0.5f, 1.0f);
                // Anchorを上基準に
                target.anchorMin = new Vector2(0.5f, 1.0f);
                target.anchorMax = new Vector2(0.5f, 1.0f);
            }
            else if(horizontal)
            {
                // Pivotを左基準に
                target.pivot = new Vector2(0, 0.5f);
                // Anchorを左基準に
                target.anchorMin = new Vector2(0, 0.5f);
                target.anchorMax = new Vector2(0, 0.5f);
            }
        }
        
        /// <summary>アイテム座標の設定</summary>
        private void SetItemPosition(RectTransform target, float position)
        {
            // 座標の調整
            Vector2 viewportSize = viewport.rect.size;
            
            if(vertical)
            {
                target.localPosition = new Vector3(viewportSize.x * 0.5f, position, 0);
            }
            else if(horizontal)
            {
                target.localPosition = new Vector3(position, -viewportSize.y * 0.5f, 0);
            }
        }
        
        private ScrollDynamicItem GetItem(int itemIndex, float position, int viewIndex, bool isActive)
        {
            object itemListObject = itemList[itemIndex];
            // セレクターから取得
            ScrollDynamicItem itemObject = itemSelector.GetItem(itemListObject);
            // TypeId
            if(itemTypeIds.TryGetValue(itemObject, out int typeId) == false)
            {
                // Idを発行
                typeId = itemTypeIds.Count;
                itemTypeIds.Add(itemObject, typeId);
                // キャッシュを生成
                itemCaches.Add(typeId, new ItemCache());
            }
            
            ScrollDynamicItem targetItem = null;
            
            // キャッシュから取得
            ItemCache itemCache = itemCaches[typeId];
            foreach(ScrollDynamicItem item in itemCache.items)
            {
                if(item.gameObject.activeSelf == false)
                {
                    targetItem = item;
                    break;
                }
            }
            
            // キャッシュにない場合は生成
            if(targetItem == null)
            {
                // 生成
                targetItem = GameObject.Instantiate<ScrollDynamicItem>(itemObject, content);
                // アクティブは一旦切っておく
                targetItem.gameObject.SetActive(false);
                // 初期化
                targetItem.Initialize(this, typeId);
                
                // アンカーとピボットを変更
                InitializeAnchorAndPivot(targetItem.UITransform);

                // キャッシュに追加
                itemCache.items.Add(targetItem);
                
#if UNITY_EDITOR
                targetItem.gameObject.name = itemObject.name + "_" + viewItemList.Count;
#endif
            }
            

            // 座標の調整
            SetItemPosition(targetItem.UITransform, position);
            
            // 表示リストに追加
            if(viewItemList.Count <= viewIndex)
            {
                Vector2 itemSize = itemObject.UITransform.sizeDelta;
                
                viewItemList.Add( new ItemInfo(targetItem, itemIndex, viewIndex, targetItem.UITransform.localPosition, itemSize));
                
                if(vertical)
                {
                    float height = (itemSize.y + spacing);
                    // コンテントのサイズ
                    RectTransform contentTransform = (RectTransform)content.transform;
                    contentTransform.sizeDelta += new Vector2(0, height);
                }
                else if(horizontal)
                {
                    float width = (itemSize.x + spacing);
                    // コンテントのサイズ
                    RectTransform contentTransform = (RectTransform)content.transform;
                    contentTransform.sizeDelta += new Vector2(width, 0);
                }
            }

            // サイズを変更
            targetItem.UITransform.sizeDelta = viewItemList[viewIndex].size;
            // 参照するアイテムを変更
            viewItemList[viewIndex].item = targetItem;
            
            if(isActive)
            {
                targetItem.gameObject.SetActive(true); 
            }

            // Index
            targetItem.SetViewIndex(viewIndex);
            // ビューの更新
            targetItem.InitializeView(itemListObject);


            return targetItem;
        }
        
        private void CheckVerticalScrollItem(Vector2 scrollNormalzed, Vector2 scrollDirection)
        {
            // スクロール量
            float scrollValue = (1.0f - scrollNormalzed.y) * (content.sizeDelta.y - viewport.rect.height);

            if(scrollDirection.y < 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[endViewItemListIndex];
                    
                    float top = item.position.y;
                    float bottom = top - item.size.y;
                    // 範囲外のものをアクティブOff
                    if(endViewItemListIndex > 0 && top < -(scrollValue + viewport.rect.height + spacing))
                    {
                        item.item.gameObject.SetActive(false);
                        endViewItemListIndex--;
                        continue;
                    }

                    break;
                }
            }
            
            if(scrollDirection.y > 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[beginViewItemListIndex];
                    
                    float top = item.position.y;
                    float bottom = top - item.size.y;
                    
                    // 範囲外のものをアクティブOff
                    if(beginViewItemListIndex < viewItemList.Count - 1 && bottom > -(scrollValue - spacing))
                    {
                        item.item.gameObject.SetActive(false);
                        beginViewItemListIndex++;
                        continue;
                    }
                    break;
                }
            }

            if(scrollDirection.y < 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[beginViewItemListIndex];
                    
                    float top = item.position.y;
                    float bottom = top - item.size.y;
                    // ひとつ上のアクティブ
                    if(beginViewItemListIndex > 0)
                    {
                        if(top <= -scrollValue)
                        {
                            ItemInfo topItem = viewItemList[beginViewItemListIndex-1];
                            
                            float y1 = topItem.position.y + spacing;
                            GetItem(topItem.itemIndex, topItem.position.y, topItem.viewIndex, y1 >= -(scrollValue + viewport.rect.height));
                            beginViewItemListIndex--;
                            continue;
                        }
                    }
                    break;
                }
            }
            
            if(scrollDirection.y > 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[endViewItemListIndex];
                    
                    float top = item.position.y;
                    float bottom = top - item.size.y;
                    // ひとつ下のアクティブ
                    if(endViewItemListIndex < viewItemList.Count-1)
                    {
                        if(bottom >= -(scrollValue + viewport.rect.height))
                        {
                            ItemInfo bottomItem = viewItemList[endViewItemListIndex+1];
                            
                            float y2 = bottomItem.position.y - bottomItem.size.y - spacing;
                            GetItem(bottomItem.itemIndex, bottomItem.position.y, bottomItem.viewIndex, y2 <= -scrollValue);
                            endViewItemListIndex++;
                            continue;
                        }
                    }

                    break;
                }
            }
        }
        
        private void CheckHorizontalScrollItem(Vector2 scrollNormalzed, Vector2 scrollDirection)
        {
            // スクロール量
            float scrollValue = scrollNormalzed.x * (content.sizeDelta.x - viewport.rect.width);

            //if(scrollValue < 0)return;

            if(scrollDirection.x < 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[beginViewItemListIndex];
                    
                    float left = item.position.x;
                    float right = left + item.size.x;
                    // 範囲外のものをアクティブOff
                    if(beginViewItemListIndex < viewItemList.Count - 1 && right < (scrollValue - spacing))
                    {
                        item.item.gameObject.SetActive(false);
                        beginViewItemListIndex++;
                        continue;
                    }

                    break;
                }
            }

            if(scrollDirection.x > 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[endViewItemListIndex];
                    
                    float left = item.position.x;
                    float right = left - item.size.x;
                    
                    // 範囲外のものをアクティブOff
                    if(endViewItemListIndex > 0 && left > (scrollValue + viewport.rect.width + spacing))
                    {
                        item.item.gameObject.SetActive(false);
                        endViewItemListIndex--;
                        continue;
                    }
                    break;
                }
            }
            
            if(scrollDirection.x < 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[endViewItemListIndex];
                    
                    float left = item.position.x;
                    float right = left + item.size.x;
                    // ひとつ右のアクティブ
                    if(endViewItemListIndex < viewItemList.Count-1)
                    {
                        if(right <= (scrollValue + viewport.rect.width))
                        {
                            ItemInfo rightItem = viewItemList[endViewItemListIndex+1];
                            
                            float x1 = rightItem.position.x + rightItem.size.x + spacing;
                            GetItem(rightItem.itemIndex, rightItem.position.x, rightItem.viewIndex, x1 >= scrollValue);
                            endViewItemListIndex++;
                            continue;
                        }
                    }
                    break;
                }
            }

            if(scrollDirection.x > 0)
            {
                while(true)
                {
                    ItemInfo item = viewItemList[beginViewItemListIndex];
                    
                    float left = item.position.x;
                    float right = left - item.size.x;
                    // ひとつ左アクティブ
                    if(beginViewItemListIndex > 0)
                    {
                        if(left >= scrollValue)
                        {
                            ItemInfo leftItem = viewItemList[beginViewItemListIndex-1];
                            
                            float x2 = leftItem.position.x - spacing;
                            
                            GetItem(leftItem.itemIndex, leftItem.position.x, leftItem.viewIndex, x2 <= scrollValue + viewport.rect.width);
                            beginViewItemListIndex--;
                            continue;
                        }
                    }

                    break;
                }
            }
        }
        
        private ScrollDynamicItem CreateNewItem(bool checkEndViewItemListIndex = true)
        {
            if(checkEndViewItemListIndex)
            {
                endViewItemListIndex = viewItemList.Count;
            }
            
            if(vertical)
            {
                ItemInfo bottomItem = viewItemList.Count > 0 ? viewItemList[viewItemList.Count-1] : null;
                float position = bottomItem == null ? -(spacing + marginTop) : (bottomItem.position.y - bottomItem.size.y - spacing);
                return GetItem(viewItemList.Count, position, viewItemList.Count, true);
            }
            if(horizontal)
            {
                ItemInfo rightItem = viewItemList.Count > 0 ? viewItemList[viewItemList.Count-1] : null;
                float position = rightItem == null ? (spacing + marginLeft) : (rightItem.position.x + rightItem.size.x + spacing);
                return GetItem(viewItemList.Count, position, viewItemList.Count, true);
            }
            
            return null;
        }
        
        private void CheckScrollItem(Vector2 scrollValue, Vector2 scrollDirection)
        {
            if(vertical)
            {
                CheckVerticalScrollItem(scrollValue, scrollDirection);
                
                // 次のアイテムを生成
                if(scrollValue.y <= 0)
                {
                    // 未精製のアイテムが有る
                    if(viewItemList.Count < itemList.Count)
                    {
                        CreateNewItem();
                    }
                }
            }
            else if(horizontal)
            {
                CheckHorizontalScrollItem(scrollValue, scrollDirection);
            
                // 次のアイテムを生成
                if(scrollValue.x >= 1.0f)
                {
                    // 未精製のアイテムが有る
                    if(viewItemList.Count < itemList.Count)
                    {
                        CreateNewItem();
                    }
                }
            }   
        }
        
        private void OnScrollValueChanged(Vector2 scrollValue)
        {
            if(viewItemList.Count == 0)return;
            
            // スクロール方向
            Vector2 scrollDirection = latestScrollValue - scrollValue;
            // スクロール位置を保持しておく
            latestScrollValue = scrollValue;
            // アイテムのチェック
            CheckScrollItem(scrollValue, scrollDirection);
        }
        
        /// <summary>再計算</summary>
        public void ForceRecalculate()
        {
            foreach(ItemInfo item in viewItemList)
            {
                item.item.ForceRecalculate();
            }
        }
        
        /// <summary>サイズ変更時</summary>
        internal void OnChangeSize(int index, bool checkScroll)
        {
            // 変更されたアイテム
            ItemInfo item = viewItemList[index];
            // 差分
            Vector2 diff = item.item.UITransform.sizeDelta - item.size;
            // データ変更
            item.size = item.item.UITransform.sizeDelta;
            // スクロールエリアのサイズ
            content.sizeDelta += diff;

            // 座標調整
            for(int i=index+1;i<viewItemList.Count;i++)
            {
                ItemInfo viewItem = viewItemList[i];
                    
                viewItem.position += new Vector2(diff.x, -diff.y);
                if(viewItem.viewIndex == viewItem.item.ViewIndex)
                {
                    viewItem.item.UITransform.localPosition = (Vector3)viewItem.position;
                }
            }
            
            // スクロールをチェック
            if(checkScroll)
            {
                if(vertical)
                {
                    CheckVerticalScrollItem(latestScrollValue, -diff.normalized);
                }
                else if(horizontal)
                {
                    CheckHorizontalScrollItem(latestScrollValue, diff.normalized);
                }
            }
        }
        
        
#if UNITY_EDITOR
        
        // 非再生時にUnityEditor上でレイアウトが確認できるように処理
        
        private void Update()
        {
            // 非再生時のみ
            if(Application.isPlaying)return;
            // contentがnull
            if(content == null)return;
            
            // 表示位置
            float position = 0;
            // マージン
            if(horizontal)
            {
                position = marginLeft;
            }
            else if(vertical)
            {
                position = marginTop;
            }
            
            // content内の子オブジェクトの座標を調整
            foreach(Transform child in content.transform)
            {
                // Item取得
                ScrollDynamicItem item = child.GetComponent<ScrollDynamicItem>();
                // アイテムなし
                if(item == null || item.gameObject.activeSelf == false)continue;
                // RectTransform
                RectTransform t = (RectTransform)item.transform;
                // アンカーを初期化
                InitializeAnchorAndPivot(t);
                // 座標をセット
                SetItemPosition(t, -position);
                // スペースを計算
                position += spacing + t.sizeDelta.y;
            }
        }
#endif
    }
}