using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public TMP_Text scoreValue;
    public TMP_Text highscoreValue;
    public int score;
    public Slider hungrySlider;
    public float maxFixedDeltaTime;
    public float minFixedDeltaTime;

    private void OnEnable()
    {
        Snake.OnSnakeGrows += IncreaseScore;
        Snake.OnRestartGame += CheckAndSaveHighscore;
        Snake.OnRestartGame += ResetScore;
    }

    private void Start()
    {
        score = 0;
        maxFixedDeltaTime = Snake.maxFixedDeltaTime;
        minFixedDeltaTime = Snake.minFixedDeltaTime;
        try
        {
            LoadHighscore();
        }
        catch (System.Exception)
        {
            PlayerDataSerializer.CreateFile("Highscore.xml");
        }
        finally
        {
            LoadHighscore();
        }
    }

    private void FixedUpdate()
    {
        hungrySlider.value = 100 - (Time.fixedDeltaTime - minFixedDeltaTime) * 100 / (maxFixedDeltaTime - minFixedDeltaTime);
    }

    public void IncreaseScore()
    {
        score++;
        scoreValue.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        scoreValue.text = score.ToString();
    }

    public void CheckAndSaveHighscore()
    {
        if (NewScoreIsHighest())
        {
            SaveHighscore(score);
            LoadHighscore();
        }
    }

    public void ResetHighscore()
    {
        SaveHighscore(0);
        LoadHighscore();
    }

    public void SaveHighscore(int currentScore)
    {
        PlayerDataSerializer.SerializePlayerInfo(new HighscoreInfo(currentScore), "Highscore.xml");
    }

    public void LoadHighscore()
    {
        highscoreValue.text = PlayerDataSerializer.DeserializePlayerInfo("Highscore.xml").ToString();
    }

    public bool NewScoreIsHighest()
    {
        int highscore;
        int.TryParse(highscoreValue.text, out highscore);
        return score > highscore;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        Snake.OnSnakeGrows -= IncreaseScore;
        Snake.OnRestartGame -= CheckAndSaveHighscore;
        Snake.OnRestartGame -= ResetScore;
    }
}
