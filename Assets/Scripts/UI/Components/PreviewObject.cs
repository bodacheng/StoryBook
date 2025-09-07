using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Scrollのレイアウトプレビュー用オブジェクト
/// </summary>
public class PreviewObject : MonoBehaviour
{
    public RectTransform RectTransform => (RectTransform)transform;

    static PreviewObject()
    {
        PrefabStage.prefabStageClosing -= DestroyAllPreviewObjects;
        PrefabStage.prefabStageClosing += DestroyAllPreviewObjects;
    }

    private static void DestroyAllPreviewObjects(PrefabStage prefabStage)
    {
        if (Application.isPlaying) return;

        using var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabStage.assetPath);
        var prefabRoot = editingScope.prefabContentsRoot;
        var previewObjects = prefabRoot.GetComponentsInChildren<PreviewObject>(includeInactive: true);
        foreach (var item in previewObjects)
        {
            DestroyImmediate(item.gameObject);
        }
    }
}