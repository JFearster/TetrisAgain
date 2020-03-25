using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // UI references
    [SerializeField] private Text linesClearedText;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text highScoreText;

    // score, lines, level, and speed.
    private int score = 0;
    private int lines = 0;
    private int currentLines = 0;
    private int level = 0;
    public float GameSpeed { get; private set; } = 1;

    /// <summary>
    /// Adding points to the total points scored
    /// </summary>
    /// <param name="pointsScored">The number of points scored.</param>
    public void ScorePoints(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: score += 100 * level;
                break;
            case 2: score += 300 * level;
                break;
            case 3: score += 500 * level;
                break;
            case 4: score += 800 * level;
                break;
            default:
                break;
        }

        IncreaseLevel(linesCleared);
    }

    /// <summary>
    /// Increase game level when the number of points scored exceeds a certain threshold.
    /// </summary>
    private void IncreaseLevel(int linesCleared)
    {
        lines += linesCleared;
        currentLines += linesCleared;
        if (currentLines >= 10)
        {
            currentLines -= 10;
            level++;
            GameSpeed = 1 - (level * 0.05f);
            if (GameSpeed < 0)
            {
                GameSpeed = 0;
            }
        }
    }

    /// <summary>
    /// Update score on the UI
    /// </summary>
    private void UpdateScore()
    {

    }

    /// <summary>
    /// Update lines cleared on the UI
    /// </summary>
    private void UpdateLines()
    {

    }
}
