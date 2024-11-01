using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Text txtScore;
    [SerializeField] Text txtLives;

    void OnGUI()
    {
        txtScore.text = string.Format("{0,3:D3}", GameController.score);
        txtLives.text = GameController.lives.ToString();
    }
}
