using UnityEngine;

[CreateAssetMenu(menuName = "GunHit/StageDatabase")]
public sealed class StageDatabase : ScriptableObject
{
    public StageConfig[] stages;

    public StageConfig Get(int stageIndex)
    {
        if (stages == null || stages.Length == 0)
            return StageConfig.Default();

        int idx = Mathf.Clamp(stageIndex, 0, stages.Length - 1);
        return stages[idx];
    }

    [System.Serializable]
    public struct StageConfig
    {
        public RotationPattern rotationPattern;
        public float rotateSpeed;
        public int fruitCount;
        public int ammo;

        public static StageConfig Default()
        {
            return new StageConfig { rotateSpeed = 60f, fruitCount = 3, ammo = 5 };
        }
    }
}