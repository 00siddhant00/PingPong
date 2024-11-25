using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int playerWonNo;
}

[System.Serializable]
public class Leaderboard
{
    public List<PlayerScore> players = new List<PlayerScore>();
}

public class LeaderboardManager : MonoBehaviour
{
    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI player_1;
    public TextMeshProUGUI player_2;
    public TextMeshProUGUI player_3;

    public TextMeshProUGUI playerScores;
    public TextMeshProUGUI SelfName;
    public TextMeshProUGUI SelfScore;

    private string jsonFilePath;
    private Leaderboard leaderboard;

    void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");

        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            leaderboard = JsonUtility.FromJson<Leaderboard>(json) ?? new Leaderboard();
        }
        else
        {
            leaderboard = new Leaderboard();
        }

        UpdateLeaderboardDisplay();
    }

    public void UpdatePlayerScore(string name)
    {
        PlayerScore player = leaderboard.players.Find(p => p.playerName == name);

        if (player != null)
        {
            player.playerWonNo++;
        }
        else
        {
            leaderboard.players.Add(new PlayerScore { playerName = name, playerWonNo = 1 });
        }

        leaderboard.players.Sort((a, b) => b.playerWonNo.CompareTo(a.playerWonNo));

        SaveLeaderboardToJson();
        UpdateLeaderboardDisplay();
    }

    private void SaveLeaderboardToJson()
    {
        string json = JsonUtility.ToJson(leaderboard, true);
        File.WriteAllText(jsonFilePath, json);
    }

    private void UpdateLeaderboardDisplay()
    {
        if (leaderboard.players.Count > 0)
        {
            player_1.text = string.Join("1. ", leaderboard.players.Count > 0 ? leaderboard.players[0].playerName : "-");
            player_2.text = string.Join("2. ", leaderboard.players.Count > 1 ? leaderboard.players[1].playerName : "-");
            player_3.text = string.Join("3. ", leaderboard.players.Count > 2 ? leaderboard.players[2].playerName : "-");

            string playerName = string.Empty;

            if (leaderboard.players.Count > 3)
                playerName = string.Join("\n", leaderboard.players.ConvertAll(p => p.playerName));
            else
                playerNames.text = "";
            string playerScore = string.Join("\n ", leaderboard.players.ConvertAll(p => p.playerWonNo.ToString()));

            playerScores.text = "\n" + playerScore;
            playerNames.text += "\n\n\n\n" + playerName;
        }
        else
        {
            player_1.text = player_2.text = player_3.text = "-";
            playerNames.text = playerScores.text = "";
        }
    }
}