using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform Player;
    public int activePlayerCount;
    public BallMovement bm;
    public TMP_InputField PlayerName;
    public GameObject ScoreCanvas;
    public GameObject GameEndCanvas;
    public GameObject LeaderboardCanvas;
    public LeaderboardManager leaderboardManager;

    public GameObject P1;
    public GameObject P2;
    public GameObject AIP1;
    public GameObject AIP2;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckStated()
    {
        foreach (Transform child in Player) // Iterate over all children of the Player parent
        {
            if (child.gameObject.activeSelf) // Check if the child GameObject is active
            {
                activePlayerCount++;
            }
        }

        // Start the game if there are exactly 2 active child objects
        if (activePlayerCount >= 2)
        {
            ScoreCanvas.SetActive(true);
            bm.StartNewGame();
        }
    }

    public void ShowLeaderboard()
    {
        leaderboardManager.UpdatePlayerScore(PlayerName.text);
        GameEndCanvas.SetActive(false);
        LeaderboardCanvas.SetActive(true);
    }

    public void StartPorxyPlay()
    {
        P1.SetActive(false);
        P2.SetActive(false);
        AIP1.SetActive(true);
        AIP2.SetActive(true);
    }
}
