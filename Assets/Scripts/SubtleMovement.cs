using UnityEngine;

public class SubtleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveAmount = 5f; 
    [SerializeField] private float moveSpeed = 1f; 
    private Vector3 initialPosition;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 5f; 
    [SerializeField] private float rotationSpeed = 1f; 
    private float initialRotationZ; 

    private void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotationZ = transform.localRotation.eulerAngles.z;
    }

    private void Update()
    {
        float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        float offsetY = Mathf.Cos(Time.time * moveSpeed * 0.8f) * moveAmount; 

        transform.localPosition = initialPosition + new Vector3(offsetX, offsetY, 0f);

        float offsetRotationZ = Mathf.Sin(Time.time * rotationSpeed * 1.2f) * rotationAmount; 

        transform.localRotation = Quaternion.Euler(0, 0, initialRotationZ + offsetRotationZ);
    }
}