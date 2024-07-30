using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    public Transform target; // The target position
    [SerializeField] private float launchAngle; // Launch angle in degrees
    [SerializeField] private float launchSpeed; // Speed of the projectile
    private Rigidbody2D rb;
    private Vector2 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        LaunchProjectile();
    }

    public void LaunchProjectile()
    {
        Vector2 targetPosition = target.position;
        Vector2 launchVelocity = CalculateLaunchVelocity(startPosition, targetPosition, launchAngle);
        rb.velocity = launchVelocity * launchSpeed;
    }

    Vector2 CalculateLaunchVelocity(Vector2 start, Vector2 end, float angle)
    {
        float gravity = Physics2D.gravity.y * rb.gravityScale;
        float radianAngle = angle * Mathf.Deg2Rad;

        float distance = Vector2.Distance(start, end);
        float x = distance * Mathf.Cos(radianAngle);
        float y = distance * Mathf.Sin(radianAngle);
        float velocitySquared = (x * x * gravity) / (2 * (x * Mathf.Tan(radianAngle) - y));
        float velocity = Mathf.Sqrt(Mathf.Abs(velocitySquared));

        Vector2 launchVelocity = new Vector2(
            velocity * Mathf.Cos(radianAngle),
            velocity * Mathf.Sin(radianAngle)
        );

        return launchVelocity;
    }

    public void InitializeProjectile(Transform target, Transform holdSpot) {
        this.target = target;
        startPosition = holdSpot.position;
    }
}
