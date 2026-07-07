using UnityEngine;

public abstract class MazeGenerationStage : MonoBehaviour
{
    public abstract void Execute(MazeContext ctx);

    public virtual void Clear() { }
}
