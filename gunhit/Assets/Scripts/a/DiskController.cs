using Unity.VisualScripting;
using UnityEngine;

public sealed class DiskController : MonoBehaviour
{
    [SerializeField] private Transform diskTransform;

    private float speed;
    private bool active;

    private void Awake()
    {
        if (diskTransform == null) diskTransform = transform;
    }

    public void Apply(float rotateSpeed)
    {
        speed = rotateSpeed;
    }

    public void SetActive(bool value) //활성화할건지
    {
        active = value;
    }

    private void Update()
    {
        if (!active) return;
        diskTransform.Rotate(0f,0f,speed * Time.deltaTime);
    }
}
