using UnityEngine;

public sealed class DiskRotationDriver : MonoBehaviour
{
    [SerializeField] private DiskController disk;

    private RotationPattern pattern;
    private float t;
    private bool active;

    private void Awake()
    {
        if (disk == null) disk = GetComponent<DiskController>();
    }

    public void SetPattern(RotationPattern p, int stageIndex)
    {
        pattern = p;
        t = 0f;
        active = (pattern != null);
        if (pattern != null) pattern.Begin(stageIndex);
    }

    public void SetActive(bool value) => active = value;


    private void Update()
    {
        if (!active || pattern == null) return;

        t += Time.deltaTime;
        float speed = pattern.Evaluate(t);
        disk.SetSpeed(speed);
    }
}
