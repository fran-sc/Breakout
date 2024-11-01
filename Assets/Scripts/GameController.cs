using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int lives {get; private set;} = 3;
    public static int score {get; private set;} = 0;

    public static List<int> totalBricks {get; private set;} = new List<int> { 0, 32, 32};

    public static void UpdateScore(int points)
    {
        score += points;
    }

    public static void UpdateLives(int numLives)
    {
        lives += numLives;
    }
}
