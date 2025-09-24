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
        
        // Get Canvas scale factor
        float scaleFactor = _canvas.scaleFactor;

        // Convert safeArea width and height to Canvas coordinates
        float safeAreaWidthInCanvas = safeArea.width / scaleFactor;
        float safeAreaHeightInCanvas = safeArea.height / scaleFactor;
        return (safeAreaWidthInCanvas, safeAreaHeightInCanvas);
    }
    
    /// <summary>
    /// We don't understand why this function gets the correct value in the places it's used. Mainly don't understand what rect.transform.position actually is
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
    
    /// This is always used together with ConvertAnchorPos, rectPos refers to the anchoredPosition of UI elements within Canvas, which is not equivalent to Screen Position.
    /// The maximum value of the former is the length and width of Canvas, the maximum value of the latter is device resolution
    /// </param>
    /// <param name="rectPos"> This value is the UI element coordinate, which should refer to the position where we want to place some effect </param>
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
