using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float rotationDamping = 2.0f;  // smoother rotation
    public float positionDamping = 2.0f;  // smoother position movement
    public float heightOffset = 2.0f;     // adjust camera height if needed
    public float distance = 5.0f;         // distance behind target

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, heightOffset, -distance);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.transform.position + target.transform.TransformDirection(offset);

        // Smoothly move camera to position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionDamping);

        // Smoothly rotate camera to face target
        Quaternion desiredRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamping);
    }
}
