using UnityEngine;

public class StayInCamera: MonoBehaviour
{
    public Camera cam;
    public float fixedDistance = 10f; // Distance from the camera
    public Vector2 screenOffset = new Vector2(0.5f, -0.5f); // Optional adjustment
    public Vector3 fixedScale = new Vector3(0.01f, 0.01f, 0.01f); // Prevent oversized bar

    public float smoothSpeed = 10f; // Higher = faster

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 targetViewportPos = new Vector3(0f, 1f, fixedDistance);
        Vector3 targetWorldPos = cam.ViewportToWorldPoint(targetViewportPos);
        targetWorldPos += (Vector3)screenOffset;

        // Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, targetWorldPos, Time.deltaTime * smoothSpeed);

        // Optional: Look at camera
        

        transform.localScale = fixedScale;
    }
}