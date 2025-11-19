using UnityEngine;

public class SImpleFormAnimate : MonoBehaviour
{
    private RectTransform rect;

    [Header("Scale Animation Settings")]
    public Vector3 minScale = Vector3.one;
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);
    public float speed = 1f;

    [Header("Rotation Animation Settings")]
    public float minZAngle = 0f;
    public float maxZAngle = 45f;

    [Header("Movement Settings")]
    public Vector3[] points;
    public float moveSpeed = 1f;

    private int currentPoint = 0;
    private int direction = 1; // 1: forward, -1: backward

    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (points != null && points.Length > 0)
            rect.anchoredPosition3D = points[0];
    }

    void Update()
    {
        // Scale Animation
        float t = Mathf.PingPong(Time.time * speed, 1f);
        rect.localScale = Vector3.Lerp(minScale, maxScale, t);

        // Rotation Animation
        float zAngle = Mathf.Lerp(minZAngle, maxZAngle, t);
        rect.localRotation = Quaternion.Euler(0f, 0f, zAngle);

        // Movement Animation between points
        if (points != null && points.Length > 1)
        {
            Vector3 target = points[currentPoint];
            rect.anchoredPosition3D = Vector3.MoveTowards(
                rect.anchoredPosition3D, target, moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(rect.anchoredPosition3D, target) < 0.01f)
            {
                // At target, get next point in ping-pong manner
                if (currentPoint == points.Length - 1)
                    direction = -1;
                else if (currentPoint == 0)
                    direction = 1;

                currentPoint += direction;
            }
        }
    }
}


