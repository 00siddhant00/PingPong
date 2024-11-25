using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float paddleSpeed = 10f;
    public float boundaryX = 7.5f;
    private bool isDragging = false;
    private Vector3 touchOffset;
    private int activeTouchId = -1; // Track which touch is moving this paddle

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Handle new touches
        foreach (Touch touch in Input.touches)
        {
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch, touchWorldPos);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    HandleTouchMoved(touch, touchWorldPos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnded(touch);
                    break;
            }
        }
    }

    private void HandleTouchBegan(Touch touch, Vector2 touchWorldPos)
    {
        // Only check for new touches if we're not already being dragged
        if (!isDragging)
        {
            // Cast a ray from touch position
            RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);

            // Check if we hit this specific paddle
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                activeTouchId = touch.fingerId;
                // Calculate and store the offset between touch and paddle position
                touchOffset = transform.position - new Vector3(touchWorldPos.x, transform.position.y, transform.position.z);
            }
        }
    }

    private void HandleTouchMoved(Touch touch, Vector2 touchWorldPos)
    {
        // Only move if this is the touch that started dragging this paddle
        if (isDragging && touch.fingerId == activeTouchId)
        {
            // Calculate new position with offset
            float newX = touchWorldPos.x + touchOffset.x;

            // Clamp paddle movement to stay within boundaries
            float clampedX = Mathf.Clamp(newX, -boundaryX, boundaryX);

            // Update paddle's position smoothly
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    private void HandleTouchEnded(Touch touch)
    {
        // Only reset if this is the touch that was dragging this paddle
        if (isDragging && touch.fingerId == activeTouchId)
        {
            isDragging = false;
            activeTouchId = -1;
        }
    }
}