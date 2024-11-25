using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxStartAngle = 45f; // Maximum angle from vertical

    private Vector2 direction;
    private bool isMoving = false;
    private Rigidbody2D rb;

    [Header("Color Settings")]
    [SerializeField] private SpriteRenderer ballSprite;
    [SerializeField] private float colorChangeSpeed = 1f; // Speed of hue shift
    [SerializeField] private ParticleSystem myParticleSystem;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!ballSprite) ballSprite = GetComponent<SpriteRenderer>();
    }

    public void StartNewGame()
    {
        ResetBall();
        LaunchBall();
    }

    private void UpdateParticleMaterialColor(ParticleSystem particleSystem, Color ballColor)
    {
        if (particleSystem != null)
        {
            // Get the Renderer component of the particle system
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

            if (renderer != null && renderer.material != null)
            {
                // Update the material's color to match the ball's color
                renderer.material.color = ballColor;
            }
        }
    }

    private void Update()
    {
        // Continuously change the ball's color by looping through hues
        if (ballSprite != null)
        {
            float hue = Mathf.PingPong(Time.time * colorChangeSpeed, 1f); // Smoothly loops hue
            ballSprite.color = Color.HSVToRGB(hue, 0.7f, 1f); // Saturation fixed at 70%, full brightness
        }

        // Update the particle effect color to match the ball's color
        UpdateParticleMaterialColor(myParticleSystem, ballSprite.color);
    }


    public void LaunchBall()
    {
        if (isMoving) return;

        // Generate random angle between -maxStartAngle and maxStartAngle
        float randomAngle = Random.Range(-maxStartAngle, maxStartAngle);

        // Convert angle to direction vector (starting downward)
        float angleInRadians = ((Random.Range(0, 2) == 1 ? -90f : 90f) + randomAngle) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Apply velocity
        rb.linearVelocity = direction * moveSpeed;
        isMoving = true;
    }

    public void ResetBall()
    {
        // Reset position to center
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("Wall"))
        {
            // Get the collision normal
            Vector2 normal = collision.GetContact(0).normal;

            // Calculate reflection direction
            direction = Vector2.Reflect(direction, normal);

            // Apply new velocity with the same speed
            rb.linearVelocity = direction * moveSpeed;

            // Optional: Add a slight random variation to make gameplay more interesting
            rb.linearVelocity += new Vector2(Random.Range(-0.5f, 0.5f), 0);
        }
    }
}
