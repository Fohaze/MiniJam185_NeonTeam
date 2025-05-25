using UnityEngine;

/// <summary>
/// Rotates the GameObject around its Y axis at a configurable speed.
/// </summary>
public class RotateYAxis : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 45f;

    private void Update()
    {
        // Rotate around local Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
