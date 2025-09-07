using UnityEngine;
using UnityEngine.UI;

public static class PosCal
{
    private static Canvas _canvas;
    private static RectTransform _safeAreaRect;
    static CanvasScaler CanvasScaler => _canvas.GetComponent<CanvasScaler>();
    public static float CanvasWidth => _canvas.GetComponent<RectTransform>().rect.width;
    public static float CanvasHeight => _canvas.GetComponent<RectTransform>().rect.height;
    
    public static (float, float) GetSafeAreaWidthAndHeightInCanvas()
    {
        Rect safeArea = Screen.safeArea;
        
        // 获取 Canvas 缩放因子
        float scaleFactor = _canvas.scaleFactor;

        // 将 safeArea 的宽高转换为 Canvas 的坐标
        float safeAreaWidthInCanvas = safeArea.width / scaleFactor;
        float safeAreaHeightInCanvas = safeArea.height / scaleFactor;
        return (safeAreaWidthInCanvas, safeAreaHeightInCanvas);
    }
    
    /// <summary>
    /// 这个函数在目前所用的地方为什么能得到正确的值我们压根不理解。主要不理解rect.transform.position到底是什么
    /// </summary>
    /// <param name="refC"></param>
    /// <param name="rect"></param>
    /// <param name="zOffset"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPos(Camera refC, RectTransform rect, float zOffset)
    {
        var rectPos = rect.transform.position;
        var trueAnchorPos = new Vector2(rectPos.x, rectPos.y);
        var worldPos = refC.ScreenToWorldPoint(trueAnchorPos);
        worldPos = new Vector3(worldPos.x, worldPos.y, refC.transform.position.z + zOffset);
        return worldPos;
    }
    
    /// 这个是一律和ConvertAnchorPos配合使用，rectPos指的是UI元素在Canvas内的anchoredPosition，它并不等同于Screen Position。
    /// 前者的最大值是Canvas的长和宽，后者的最大值是设备分辨率
    /// </param>
    /// <param name="rectPos"> 这个值是UI元素的坐标，指的应该是我们希望把某个特效给定到的位置 </param>
    /// <param name="zOffset"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPos(Camera refC, Vector3 rectPos, float zOffset)
    {
        var screenPos = new Vector2(Screen.width * rectPos.x/ CanvasWidth, Screen.height * rectPos.y/ CanvasHeight);
        var worldPos = refC.ScreenToWorldPoint(screenPos);
        worldPos = new Vector3(worldPos.x, worldPos.y, refC.transform.position.z + zOffset);
        return worldPos;
    }
    
    public static Vector2 CalculateAnchoredPositionInNewAnchor(RectTransform uiElement, Vector2 targetAnchor, RectTransform refParent)
    {
        Vector2 parentSize = refParent.rect.size;
        
        // Calculate the current position in the parent RectTransform space.
        Vector2 currentPositionInParentSpace = uiElement.anchorMin * parentSize + uiElement.anchoredPosition;

        // Calculate the new anchored position in the target anchor without modifying the RectTransform.
        Vector2 newPositionInTargetAnchor = currentPositionInParentSpace - targetAnchor * parentSize;
        
        return newPositionInTargetAnchor;
    }
    
    public static float AdjustedViewPortHeight(float originHeight, float itemHeight, float space)
    {
        var linesToShowInViewPort = 0;
        while (true)
        {
            if ((linesToShowInViewPort + 1) * (itemHeight + space) < originHeight)
            {
                linesToShowInViewPort++;
            }
            else
            {
                break;
            }
        }
        var viewPortHeight = linesToShowInViewPort * (itemHeight + space) - space;
        return viewPortHeight;
    }
}
