using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UILayer : MonoBehaviour
{
    public string Index { get; set; }
    
    /// <summary>ページを開く前の処理</summary>
    public virtual async UniTask OnPreOpen()
    {
    }
    
    /// <summary>ページを閉じる前の処理</summary>
    public virtual async UniTask OnPreClose()
    {
    }
    
    public void Pop()
    {
        ProcessesRunner.Main.Pop().Forget();
    }

    public virtual void OnDestroy()
    {
        
    }
    
    
    protected void SetGridGroupSize(GridLayoutGroup grid, float paddingLeftRight)
    {
        // 获取父对象的宽度
        RectTransform parentRect = grid.transform.parent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        // 根据父对象的宽度，左右padding和格子间距来计算每个格子的大小
        int cellsPerRow = grid.constraintCount; // 每行的格子数量
        float cellWidth =  (parentWidth - paddingLeftRight * 2 - grid.spacing.x * (cellsPerRow - 1)) / cellsPerRow;

        // 确保格子是正方形
        grid.cellSize = new Vector2(cellWidth, cellWidth);
    }
    
    protected void ToTop()
    {
        gameObject.transform.SetAsLastSibling();
    }
}
