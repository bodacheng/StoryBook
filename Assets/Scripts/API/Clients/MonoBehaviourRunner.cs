using UnityEngine;

/// <summary>
/// 用于在非MonoBehaviour类中执行协程的单例
/// </summary>
public class MonoBehaviourRunner : MonoBehaviour
{
    private static MonoBehaviourRunner _instance;
    
    public static MonoBehaviourRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("MonoBehaviourRunner");
                _instance = go.AddComponent<MonoBehaviourRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
