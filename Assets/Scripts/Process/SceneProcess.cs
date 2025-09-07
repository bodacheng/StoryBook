using Cysharp.Threading.Tasks;

public abstract class SceneProcess
{
    public virtual async UniTask ProcessEnter()
    {
    }
        
    public virtual async UniTask ProcessEnter<T>(T t)
    {
    }

    public virtual async UniTask ProcessEnd()
    {
    }
    
    public virtual void LocalUpdate()
    {
    }
        
    public virtual void LocalFixedUpdate()
    {
    }
}
