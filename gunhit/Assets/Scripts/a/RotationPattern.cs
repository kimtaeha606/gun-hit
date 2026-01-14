using UnityEngine;

public abstract class RotationPattern : ScriptableObject
{
    public virtual void Begin(int stageIndex) { }

    public abstract float Evaluate(float t);

}