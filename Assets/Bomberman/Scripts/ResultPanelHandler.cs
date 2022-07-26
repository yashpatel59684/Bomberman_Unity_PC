using TMPro;
using UnityEngine;

public sealed class ResultPanelHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameStatusText, totalGameText, totalWinText;
    ResultPanelHandler() { }
    void OnEnable()
    {
        gameStatusText.text = GameManager.Instance.IsGameWin ? "You Win!" : "You Lose!";
        totalGameText.text = $"Total Played Game:- {GameManager.Instance.TotalPlayedGames}";
        totalWinText.text = $"Total Win Game:- {GameManager.Instance.TotalWinGames}";
    }
    public void RestartGame()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
}
