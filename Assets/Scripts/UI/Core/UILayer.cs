using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UILayer : MonoBehaviour
{
    public string Index { get; set; }
    
    /// <summary>Pre-open page processing</summary>
    public virtual async UniTask OnPreOpen()
    {
    }
    
    /// <summary>Pre-close page processing</summary>
    public virtual async UniTask OnPreClose()
    {
    }
    
    public void Pop()
    {
        ProcessesRunner.Main.Pop().Forget();
    }
    
    public void Move(MainSceneStep step)
    {
        ProcessesRunner.Main.TrySwitchToStep(step).Forget();
    }

    public virtual void OnDestroy()
    {
        
    }
    
    
    protected void SetGridGroupSize(GridLayoutGroup grid, float paddingLeftRight)
    {
        // Get parent object width
        RectTransform parentRect = grid.transform.parent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        // Calculate each grid size based on parent width, left/right padding and grid spacing
        int cellsPerRow = grid.constraintCount; // Number of cells per row
        float cellWidth =  (parentWidth - paddingLeftRight * 2 - grid.spacing.x * (cellsPerRow - 1)) / cellsPerRow;

        // Ensure grid cells are square
        grid.cellSize = new Vector2(cellWidth, cellWidth);
    }
    
    protected void ToTop()
    {
        gameObject.transform.SetAsLastSibling();
    }
}
