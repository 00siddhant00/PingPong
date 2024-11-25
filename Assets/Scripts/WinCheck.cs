using UnityEngine;

public class WinCheck : MonoBehaviour
{
    public BallMovement bm;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (gameObject.name)
            {
                case "P1":
                    ScoreManager.Instance.IncreaseScore(1);
                    break;
                case "P2":
                    ScoreManager.Instance.IncreaseScore(2);
                    break;
            }

            bm.StartNewGame();
        }
    }
}
