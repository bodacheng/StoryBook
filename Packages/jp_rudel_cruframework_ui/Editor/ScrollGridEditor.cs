using CruFramework.Engine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Editor.UI
{

    [CustomEditor(typeof(ScrollGrid))]
    public class ScrollGridEditor : ScrollRectEditor
    {
        
        private enum ScrollDirection
        {
            Horizontal, Vertical
        }
        
        private bool marginFoldout = false;
        private int previewScrollItemCount = 10;
        
        [MenuItem("GameObject/CruFramework/UI/ScrollGrid/Horizontal")]
        public static void CreateHorizontalMenu()
        {
            CreateMenu(ScrollDirection.Horizontal);
        }
        
        [MenuItem("GameObject/CruFramework/UI/ScrollGrid/Vertical")]
        public static void CreateVerticalMenu()
        {
            CreateMenu(ScrollDirection.Vertical);
        }
        
        private static void CreateMenu(ScrollDirection direction)
        {
            
            Vector2 size = new Vector2(300.0f, 400.0f);
            
            // 生成位置
            GameObject parent = Selection.gameObjects.Length > 0 ? Selection.gameObjects[0] : null; 
            // GameObject生成
            RectTransform scrollObject = CreateGameObject(parent.transform, nameof(ScrollGrid));
            // レイヤー
            SetLayer(scrollObject, parent.layer);
            // ScrollGridをアタッチ
            ScrollGrid scrollGrid = scrollObject.gameObject.AddComponent<ScrollGrid>();
            // サイズを設定
            ((RectTransform)scrollGrid.transform).sizeDelta = size;
            // スクロール
            scrollGrid.Type = direction == ScrollDirection.Horizontal ? ScrollGrid.ScrollType.Horizontal : ScrollGrid.ScrollType.Vertical;
            scrollGrid.Alignment = direction == ScrollDirection.Horizontal ? ScrollGrid.ItemAlignment.Vertical : ScrollGrid.ItemAlignment.Horizontal;
            // 背景を追加
            Image background = scrollObject.gameObject.AddComponent<Image>();
            background.color = new Color(1.0f, 1.0f, 1.0f, 100.0f / 255.0f);
            
            // ViewPort
            RectTransform viewport = CreateGameObject(scrollGrid.transform, "Viewport");
            // Scrollに登録
            scrollGrid.viewport = viewport;
            
            // アンカー
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            // Pivot
            viewport.pivot = new Vector2(0, 1.0f);
            // マージン
            viewport.offsetMin = new Vector2(0, direction == ScrollDirection.Horizontal ? 20.0f : 0);
            viewport.offsetMax = new Vector2(direction == ScrollDirection.Vertical ? -20.0f : 0, 0);
            
            // マスク
            viewport.gameObject.AddComponent<Image>();
            Mask viewportMask = viewport.gameObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // ViewPort
            RectTransform content = CreateGameObject(viewport.transform, "Content");
            // Scrollに登録
            scrollGrid.content = content;
            
            // アンカー
            content.anchorMin = new Vector2(0, 1.0f); 
            content.anchorMax = new Vector2(0, 1.0f);
            // Pivot
            content.pivot = new Vector2(0, 1.0f);
            content.sizeDelta = Vector2.zero;
            content.localPosition = Vector3.zero;
            
            // スクロールバー
            RectTransform scrollbarObject = CreateGameObject(scrollGrid.transform, direction.ToString() + "Scrollbar");
                
            // Scrollbarをアタッチ
            Scrollbar scrollbar = scrollbarObject.gameObject.AddComponent<Scrollbar>();
            // Imageをアタッチ
            Image scrollbarImage = scrollbarObject.gameObject.AddComponent<Image>();
            scrollbarImage.color = new Color(1.0f, 1.0f, 1.0f, 120.0f / 255.0f);
            // スクロール方向
            scrollbar.direction = direction == ScrollDirection.Horizontal ? Scrollbar.Direction.LeftToRight : Scrollbar.Direction.BottomToTop;

            
            if(direction == ScrollDirection.Horizontal)
            {
                // スクロールに登録
                scrollGrid.horizontalScrollbar = scrollbar;
                
                RectTransform t = (RectTransform)scrollbar.transform;
                
                // アンカー
                t.anchorMin = Vector2.zero;
                t.anchorMax = new Vector2(1.0f, 0);
                // Pivot
                t.pivot = Vector2.zero;
                
                t.sizeDelta = new Vector2(0, 18.0f);
                //t.offsetMax = new Vector2(-20.0f, t.offsetMax.y);
            }
            else if(direction == ScrollDirection.Vertical)
            {
                // スクロールに登録
                scrollGrid.verticalScrollbar = scrollbar;
                
                RectTransform t = (RectTransform)scrollbar.transform;
                
                // アンカー
                t.anchorMin = new Vector2(1.0f, 0);
                t.anchorMax = new Vector2(1.0f, 1.0f);
                // Pivot
                t.pivot = Vector2.one;
                
                t.sizeDelta = new Vector2(18.0f, 0);
                //t.offsetMax = new Vector2(t.offsetMax.y, -20.0f);
            }
            
            // ScrollArea
            RectTransform scrollAreaObject = CreateGameObject(scrollbar.transform, "SlidingArea");
            // アンカー
            scrollAreaObject.anchorMin = Vector2.zero;
            scrollAreaObject.anchorMax = Vector2.one;
            // Pivot
            scrollAreaObject.pivot = new Vector2(0.5f, 0.5f);
            scrollAreaObject.offsetMin = new Vector2(10.0f, 10.0f);
            scrollAreaObject.offsetMax = new Vector2(-10.0f, -10.0f);
            
            // ハンドル
            RectTransform handleObject = CreateGameObject(scrollAreaObject.transform, "Handle");
            // イメージを追加
            Image handleImage = handleObject.gameObject.AddComponent<Image>();
            
            // Pivot
            handleObject.pivot = new Vector2(0.5f, 0.5f);
            handleObject.offsetMin = new Vector2(-10.0f, -10.0f);
            handleObject.offsetMax = new Vector2(10.0f, 10.0f);
            
            // スクロールバーにイメージを登録
            scrollbar.targetGraphic = handleImage;
            // スクロールバーにハンドル登録
            scrollbar.handleRect = handleObject;
            
            // サイズ
            scrollbar.size = 0.1f;

            // 生成したオブジェクトを選択
            Selection.activeGameObject = scrollGrid.gameObject;
        }
        

        private static RectTransform CreateGameObject(Transform parent, string name)
        {
            // ViewPort
            GameObject obj = new GameObject();
            
            RectTransform transform = obj.AddComponent<RectTransform>();
            // 名前
            obj.name = name;
            // 親を設定
            obj.transform.SetParent(parent);
            // 座標を初期化
            obj.transform.localPosition = Vector3.zero;
            // スケール初期化
            obj.transform.localScale = Vector3.one;
            
            return transform;
        }
        

        public override void OnInspectorGUI()
        {
            ScrollGrid scrollGrid = (ScrollGrid)target;
            
            EditorGUI.BeginChangeCheck();
            // Undo
            Undo.RecordObject(target, "ScrollGrid");
            
            // インタラクティブの設定
            scrollGrid.Interactable = EditorGUILayout.Toggle(nameof(scrollGrid.Interactable), scrollGrid.Interactable);
            // プレハブの設定
            scrollGrid.ItemPrefab = (ScrollGridItem)EditorGUILayout.ObjectField(nameof(scrollGrid.ItemPrefab), scrollGrid.ItemPrefab, typeof(ScrollGridItem), true);
            // アイテム並びの設定
            scrollGrid.Alignment = (ScrollGrid.ItemAlignment)EditorGUILayout.EnumPopup(nameof(scrollGrid.Alignment), scrollGrid.Alignment);
            // 並び順の反転
            scrollGrid.AlignmentReverse = EditorGUILayout.Toggle(nameof(scrollGrid.AlignmentReverse), scrollGrid.AlignmentReverse);
            // ループの設定
            scrollGrid.IsLoop = EditorGUILayout.Toggle(nameof(scrollGrid.IsLoop), scrollGrid.IsLoop);
            // タイプの設定
            scrollGrid.SelectMode = (ScrollGrid.SelectModeType)EditorGUILayout.EnumPopup(nameof(scrollGrid.SelectMode), scrollGrid.SelectMode);
            // タイプの設定
            scrollGrid.Type = (ScrollGrid.ScrollType)EditorGUILayout.EnumPopup(nameof(scrollGrid.Type), scrollGrid.Type);
            // マージン
            marginFoldout = EditorGUILayout.Foldout(marginFoldout, "Margin");
            if(marginFoldout)
            {
                EditorGUI.indentLevel++;
                scrollGrid.MarginLeft   = EditorGUILayout.FloatField(nameof(scrollGrid.MarginLeft), scrollGrid.MarginLeft);
                scrollGrid.MarginRight  = EditorGUILayout.FloatField(nameof(scrollGrid.MarginRight), scrollGrid.MarginRight);
                scrollGrid.MarginTop    = EditorGUILayout.FloatField(nameof(scrollGrid.MarginTop), scrollGrid.MarginTop);
                scrollGrid.MarginBottom = EditorGUILayout.FloatField(nameof(scrollGrid.MarginBottom), scrollGrid.MarginBottom);
                EditorGUI.indentLevel--;
            }
 
            //　ページの設定
            if(scrollGrid.IsPaging)
            {
                EditorGUI.indentLevel++;
                scrollGrid.PagingSnap = (ScrollGrid.PagingSnapType)EditorGUILayout.EnumPopup(nameof(scrollGrid.PagingSnap), scrollGrid.PagingSnap);
                scrollGrid.PagingAnimationDuration = EditorGUILayout.FloatField(nameof(scrollGrid.PagingAnimationDuration), scrollGrid.PagingAnimationDuration);
                scrollGrid.FixedPageItemCount = EditorGUILayout.IntField(nameof(scrollGrid.FixedPageItemCount), scrollGrid.FixedPageItemCount);
                EditorGUI.indentLevel--;
            }
            
            //　横スクロールの設定
            EditorGUILayout.LabelField("HorizontalSpace");
            EditorGUI.indentLevel++;
            {
                DrawGridInfoGUI(scrollGrid.HorizontalGridInfo);
            }
            EditorGUI.indentLevel--;
            
            //　縦スクロールの設定
            EditorGUILayout.LabelField("VerticalSpace");
            EditorGUI.indentLevel++;
            {
                DrawGridInfoGUI(scrollGrid.VerticalGridInfo);
            }
            EditorGUI.indentLevel--;
            
            // ScrollItemプレビュー
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Preview");
                previewScrollItemCount = EditorGUILayout.IntField(nameof(previewScrollItemCount), previewScrollItemCount);
                if (GUILayout.Button("Create Preview Objects"))
                {
                    CreatePreviewScrollItems();
                }
                if (GUILayout.Button("Delete Preview Objects"))
                {
                    DeletePreviewScrollItems();
                }
            }
            EditorGUILayout.EndVertical();

            // pjshin専用の設定
            EditorGUILayout.LabelField("==== PJShin ====");
            EditorGUI.indentLevel++;
            {
                scrollGrid.KeepCurrentPageAtRefresh = EditorGUILayout.Toggle(nameof(scrollGrid.KeepCurrentPageAtRefresh), scrollGrid.KeepCurrentPageAtRefresh);
            }
            EditorGUI.indentLevel--;
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("==== Unity ScrollRect ====");
            base.OnInspectorGUI();
        }

        private void CreatePreviewScrollItems()
        {
            ScrollGrid scrollGrid = (ScrollGrid)target;
            DeletePreviewScrollItems();
            for (int i = 0; i < previewScrollItemCount; i++)
            {
                var instance = Instantiate(scrollGrid.ItemPrefab, scrollGrid.content);
                instance.gameObject.SetActive(true);
                
                var previewObject = instance.gameObject.AddComponent<PreviewObject>();
                Vector3 p = scrollGrid.GetItemPositionByIndex(i);
                previewObject.RectTransform.localPosition = p;
            }
        }

        private void DeletePreviewScrollItems()
        {
            ScrollGrid scrollGrid = (ScrollGrid)target;
            var previewObjects = scrollGrid.content.GetComponentsInChildren<PreviewObject>(true);
            foreach (var item in previewObjects)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        
        private static void SetLayer(Transform target, int layer)
        {
            target.gameObject.layer = layer;
            
            foreach(Transform child in target)
            {
                SetLayer(child, layer);
            }
        }
        
        private void DrawGridInfoGUI(ScrollGrid.GridInfo grid)
        {
            grid.SpacingType = (ScrollGrid.SpacingType)EditorGUILayout.EnumPopup(nameof(grid.SpacingType), grid.SpacingType);
            if(grid.SpacingType == ScrollGrid.SpacingType.FixedSpace)
            {
                grid.Spacing = EditorGUILayout.FloatField(nameof(grid.Spacing), grid.Spacing);
            }
            
            if(grid.SpacingType == ScrollGrid.SpacingType.FixedItemCount)
            {
                grid.ItemCount = EditorGUILayout.IntField(nameof(grid.ItemCount), grid.ItemCount);
            }
        }

        [DrawGizmo(GizmoType.Selected, typeof(ScrollGrid))]
        private static void DrawGizmo(ScrollGrid scroll, GizmoType gizmoType)
        {
            // サイズを計算
            scroll.CalcItemSize();
            // 座標を取得
            int count = scroll.ItemCount.x * scroll.ItemCount.y;

            // ギズモの色
            Gizmos.color = Color.green;
            // Contentの行列
            Gizmos.matrix = scroll.content.localToWorldMatrix;

            for(int i=0;i<count;i++)
            {
                // 座標を取得
                Vector3 p = scroll.GetItemPositionByIndex(i);
                // ギズモ表示
                Gizmos.DrawWireCube(p, scroll.ItemSize);
            }
        }
    }
    
}
