using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI P1ScoreTxt;
    public TextMeshProUGUI P2ScoreTxt;

    int P1Score;
    int P2Score;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        P1ScoreTxt.text = P1Score.ToString();
        P2ScoreTxt.text = P2Score.ToString();
    }

    public void IncreaseScore(int playerNo)
    {
        switch (playerNo)
        {
            case 1:
                P1Score++;
                P1ScoreTxt.GetComponent<Animator>().SetTrigger("Pop");
                break;
            case 2:
                P2Score++;
                P2ScoreTxt.GetComponent<Animator>().SetTrigger("Pop");
                break;
        }

        if (P1Score >= 10 || P2Score >= 10)
        {
            GameManager.Instance.ScoreCanvas.SetActive(false);
            GameManager.Instance.GameEndCanvas.SetActive(true);
            GameManager.Instance.StartPorxyPlay();
        }
    }
}
