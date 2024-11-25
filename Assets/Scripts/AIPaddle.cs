using UnityEngine;
using System.Collections;

public class AdvancedAIPaddle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ball;

    [Header("Movement Settings")]
    [SerializeField] private float maxPaddleSpeed = 15f;  // Increased base speed
    [SerializeField] private float boundaryX = 1.6f;
    [SerializeField] private float accelerationRate = 2f;  // New: Controls how quickly paddle reaches max speed

    [Header("AI Behavior")]
    [SerializeField] private float reactionDistance = 12f;  // Increased reaction distance
    [SerializeField] private float predictionAccuracy = 0.9f;  // Increased accuracy
    [SerializeField] private float baseErrorMargin = 0.3f;  // Reduced base error
    [SerializeField][Range(0, 1)] private float difficultySetting = 0.7f;

    private float currentSpeed;
    private float targetX;
    private Vector2 previousBallPos;
    private float currentErrorMargin;
    private Rigidbody2D ballRb;
    private State currentState = State.Tracking; // Start in tracking state
    private float currentSpeedMultiplier = 1f;

    private enum State
    {
        Idle,
        Tracking,
        Recovery,
        Hesitating
    }

    private void Start()
    {
        ballRb = ball.GetComponent<Rigidbody2D>();
        currentSpeed = maxPaddleSpeed;
        StartCoroutine(DecisionLoop());
        StartCoroutine(SpeedAdjustmentLoop());
    }

    private void Update()
    {
        // Always update prediction and position
        if (currentState != State.Idle)
        {
            PredictBallPosition();
            UpdatePaddlePosition();
        }
        previousBallPos = ball.position;
    }

    private IEnumerator DecisionLoop()
    {
        while (true)
        {
            MakeStrategicDecision();
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f)); // Faster decision making
        }
    }

    private IEnumerator SpeedAdjustmentLoop()
    {
        while (true)
        {
            // Dynamically adjust speed based on ball distance and velocity
            float distanceToBall = Mathf.Abs(transform.position.x - ball.position.x);
            float urgencyFactor = Mathf.Clamp01(1.0f - (distanceToBall / reactionDistance));

            if (ballRb != null && IsBallApproaching())
            {
                float ballSpeed = ballRb.linearVelocity.magnitude;
                currentSpeedMultiplier = Mathf.Lerp(0.8f, 1.2f, urgencyFactor * ballSpeed / 10f);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsBallApproaching()
    {
        if (ballRb == null) return false;

        bool isAbove = transform.position.y > ball.position.y;
        bool movingDown = ballRb.linearVelocity.y < 0;

        return (isAbove && movingDown) || (!isAbove && !movingDown);
    }

    private void MakeStrategicDecision()
    {
        float distanceToBall = Mathf.Abs(transform.position.y - ball.position.y);

        if (!IsBallApproaching())
        {
            if (currentState != State.Idle)
            {
                currentState = State.Idle;
                ReturnToNeutralPosition();
            }
            return;
        }

        // More aggressive error margin calculation
        float speedFactor = ballRb.linearVelocity.magnitude / 15f;
        currentErrorMargin = baseErrorMargin * (1 + speedFactor) * (1 - difficultySetting);

        // More selective mistake making
        if (ShouldMakeMistake() && distanceToBall > 2f)
        {
            StartCoroutine(DeliberateMistake());
            return;
        }

        // Faster state transitions
        switch (currentState)
        {
            case State.Idle:
                if (distanceToBall < reactionDistance)
                {
                    currentState = Random.value < 0.9f ? State.Tracking : State.Hesitating;
                    if (currentState == State.Hesitating)
                    {
                        StartCoroutine(HesitateBeforeMoving());
                    }
                }
                break;

            case State.Recovery:
                if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
                {
                    currentState = State.Tracking;
                }
                break;
        }
    }

    private void PredictBallPosition()
    {
        if (ballRb == null) return;

        Vector2 ballPos = ball.position;
        Vector2 ballVel = ballRb.linearVelocity;

        float deltaY = transform.position.y - ballPos.y;
        float timeToIntercept = Mathf.Abs(deltaY / (ballVel.y + 0.0001f));

        if (timeToIntercept > 0)
        {
            // More accurate prediction with smaller error window
            float perfectX = ballPos.x + ballVel.x * timeToIntercept;
            float predictionError = (1f - predictionAccuracy) * Random.Range(-0.5f, 0.5f);
            targetX = perfectX + (predictionError * currentErrorMargin);

            // Add slight prediction bias based on ball velocity
            targetX += ballVel.x * 0.1f;

            targetX = Mathf.Clamp(targetX, -boundaryX, boundaryX);
        }
    }

    private void UpdatePaddlePosition()
    {
        if (currentState == State.Idle || currentState == State.Hesitating)
            return;

        float distanceToTarget = Mathf.Abs(transform.position.x - targetX);

        // More aggressive speed scaling
        float speedMultiplier = Mathf.Lerp(0.5f, 1.5f, distanceToTarget);
        float finalSpeed = currentSpeed * speedMultiplier * currentSpeedMultiplier;

        // Smoother movement with acceleration
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            finalSpeed * Time.deltaTime
        );
    }

    private void ReturnToNeutralPosition()
    {
        targetX = 0f;
        currentSpeedMultiplier = 0.5f;
    }

    private bool ShouldMakeMistake()
    {
        if (ballRb == null) return false;

        float ballSpeed = ballRb.linearVelocity.magnitude;
        float mistakeProbability = (1f - difficultySetting) * (ballSpeed / 25f);
        return Random.value < mistakeProbability && Time.time > 2f; // No mistakes in first 2 seconds
    }

    private IEnumerator DeliberateMistake()
    {
        currentState = State.Recovery;
        float originalSpeed = currentSpeed;

        // Shorter mistake duration
        if (Random.value > 0.7f)
        {
            targetX += Random.Range(-0.3f, 0.3f) * (1 - difficultySetting);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
        else
        {
            currentSpeed *= 0.7f;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }

        currentSpeed = originalSpeed;
        currentState = State.Tracking;
    }

    private IEnumerator HesitateBeforeMoving()
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.15f)); // Shorter hesitation
        currentState = State.Tracking;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(targetX, transform.position.y, 0), 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ball.position, 0.2f);
    }
}