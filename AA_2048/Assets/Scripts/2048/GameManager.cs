using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard board; //游戏核心逻辑
    public CanvasGroup gameOver; //游戏结束面板
    public TextMeshProUGUI txtScore; //当前游戏得分
    public TextMeshProUGUI txtBestScore; //当前游戏最高得分

    private int score = 0; //游戏分数

    public void NewGame()
    {
        //还原分数
        SetScore(0);

        txtBestScore.text = LoadBestScore().ToString();

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;
        PlayerPrefs.Save();

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        var elapsed = 0f;
        var duration = 0.5f;
        var from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        txtScore.text = score.ToString();

        SaveBestScore();
    }

    private void SaveBestScore()
    {
        var bestScore = LoadBestScore();

        if (score > bestScore) PlayerPrefs.SetInt("bScore", score);
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("bScore", 0);
    }
}