using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextColorLerp : MonoBehaviour
{
    [SerializeField] Text msg;
    [SerializeField] float duration;

    void Start()
    {
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            // interpolamos el color del texto entre el negro y el blanco
            msg.color = Color.Lerp(Color.black, Color.white, t / duration);

            yield return null;
        }

        // relanzamos la corrutina de cambio de color del texto
        StartCoroutine(ChangeColor());
    }
}
