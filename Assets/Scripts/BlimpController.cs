using UnityEngine;

public class BlimpController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 2.0f;
    public float areaPadding = 2.0f; // Padding from edges
    public float minChangeDirectionTime = 2f;
    public float maxChangeDirectionTime = 5f;

    [Header("Height Settings")]
    public float fixedHeight = 10.0f;

    private Vector3 targetPosition;
    private float areaMinX, areaMaxX, areaMinZ, areaMaxZ;
    private Camera cam;
    private float nextDirectionChangeTime;

    void Start()
    {
        cam = Camera.main;
        CalculateBounds();
        PickNewTarget();
    }

    void Update()
    {
        // Keep blimp at fixed height
        Vector3 currentPosition = transform.position;
        currentPosition.y = fixedHeight;
        transform.position = currentPosition;

        // Move smoothly towards target
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0; // Only move in XZ plane
        if (direction.magnitude > 0.1f)
        {
            // Smooth turning
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Smooth movement
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            PickNewTarget();
        }

        // Change direction after random time
        if (Time.time > nextDirectionChangeTime)
        {
            PickNewTarget();
        }
    }

    void CalculateBounds()
    {
        // Calculate world bounds based on camera view
        // For orthographic camera
        if (cam.orthographic)
        {
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * cam.aspect;

            areaMinX = cam.transform.position.x - horzExtent + areaPadding;
            areaMaxX = cam.transform.position.x + horzExtent - areaPadding;
            areaMinZ = cam.transform.position.z - vertExtent + areaPadding;
            areaMaxZ = cam.transform.position.z + vertExtent - areaPadding;
        }
        else
        {
            // Perspective cameras: define your own area or use a fixed boundary
            areaMinX = -20f + areaPadding;
            areaMaxX = 20f - areaPadding;
            areaMinZ = -20f + areaPadding;
            areaMaxZ = 20f - areaPadding;
        }
    }

    void PickNewTarget()
    {
        float x = Random.Range(areaMinX, areaMaxX);
        float z = Random.Range(areaMinZ, areaMaxZ);
        targetPosition = new Vector3(x, fixedHeight, z);

        nextDirectionChangeTime = Time.time + Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
    }

    // Optional: Draw bounds and target in Scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3((areaMinX + areaMaxX) / 2, fixedHeight, (areaMinZ + areaMaxZ) / 2),
                            new Vector3(areaMaxX - areaMinX, 0.1f, areaMaxZ - areaMinZ));
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.5f);
    }
}