using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class StoryDisplayLayerPrefabCreator : EditorWindow
{
    [MenuItem("Tools/StoryBook/Create StoryDisplayLayer Prefab")]
    public static void CreateStoryDisplayLayerPrefab()
    {
        // 创建主Canvas
        GameObject canvas = new GameObject("StoryDisplayLayer");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // 添加StoryDisplayLayer脚本
        canvas.AddComponent<StoryDisplayLayer>();
        
        // 创建背景
        Image background = canvas.AddComponent<Image>();
        background.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        
        // 创建Header Panel
        GameObject headerPanel = CreateHeaderPanel(canvas.transform);
        
        // 创建Scroll View
        GameObject scrollView = CreateScrollView(canvas.transform);
        
        // 创建Button Panel
        GameObject buttonPanel = CreateButtonPanel(canvas.transform);
        
        // 设置RectTransform
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // 保存为预制体
        string prefabPath = "Assets/Prefabs/UI/StoryDisplayLayer.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvas, prefabPath);
        
        // 清理场景中的临时对象
        DestroyImmediate(canvas);
        
        Debug.Log($"StoryDisplayLayer prefab created at: {prefabPath}");
    }
    
    private static GameObject CreateHeaderPanel(Transform parent)
    {
        GameObject headerPanel = new GameObject("HeaderPanel");
        headerPanel.transform.SetParent(parent);
        
        RectTransform rectTransform = headerPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, -100);
        rectTransform.offsetMax = new Vector2(0, 0);
        
        Image image = headerPanel.AddComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        // 创建标题文本
        GameObject titleText = CreateText("TitleText", headerPanel.transform, "故事标题", 24, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, -20), new Vector2(-200, -40));
        
        // 创建主题文本
        GameObject themeText = CreateText("ThemeText", headerPanel.transform, "主题: 冒险", 18, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, -50), new Vector2(-200, -70));
        
        // 创建页数文本
        GameObject pageCountText = CreateText("PageCountText", headerPanel.transform, "共 5 页", 16, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(20, -80), new Vector2(-200, -100));
        
        return headerPanel;
    }
    
    private static GameObject CreateScrollView(Transform parent)
    {
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(parent);
        
        RectTransform rectTransform = scrollView.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 60);
        rectTransform.offsetMax = new Vector2(0, -60);
        
        Image image = scrollView.AddComponent<Image>();
        image.color = Color.white;
        
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
        // 创建Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = Color.white;
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        contentRect.pivot = new Vector2(0.5f, 1);
        
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        
        return scrollView;
    }
    
    private static GameObject CreateButtonPanel(Transform parent)
    {
        GameObject buttonPanel = new GameObject("ButtonPanel");
        buttonPanel.transform.SetParent(parent);
        
        RectTransform rectTransform = buttonPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 60);
        
        Image image = buttonPanel.AddComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        HorizontalLayoutGroup layoutGroup = buttonPanel.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.padding = new RectOffset(20, 20, 10, 10);
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        
        // 创建返回按钮
        GameObject backButton = CreateButton("BackButton", buttonPanel.transform, "返回", new Vector2(100, 40));
        
        // 创建保存按钮
        GameObject saveButton = CreateButton("SaveButton", buttonPanel.transform, "保存", new Vector2(100, 40));
        
        // 创建分享按钮
        GameObject shareButton = CreateButton("ShareButton", buttonPanel.transform, "分享", new Vector2(100, 40));
        
        return buttonPanel;
    }
    
    private static GameObject CreateText(string name, Transform parent, string text, int fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.black;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        return textObj;
    }
    
    private static GameObject CreateButton(string name, Transform parent, string text, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // 创建按钮文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        return buttonObj;
    }
}
