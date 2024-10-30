using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Text txtScore;
    [SerializeField] Text txtLives;

    [Header("Settings")]
    [SerializeField] int lives;

    int score = 0;
    

    public void UpdateScore(int points)
    {
        score += points;
    }

    public void UpdateLives(int numLives)
    {
        lives += numLives;
    }
    void OnGUI()
    {
        txtScore.text = string.Format("{0,3:D3}", score);
        txtLives.text = lives.ToString();
    }
}
