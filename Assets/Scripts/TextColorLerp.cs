using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 TextColorLerp
 -----------------
 Responsabilidad:
 - Cambia el color de un componente Text interpolando entre dos colores (negro y blanco)
     durante un intervalo de tiempo y repite el ciclo.

 Notas sobre diseño:
 - El componente Text y la duración de la interpolación se asignan desde el Inspector.
 - Usa una corrutina para realizar una interpolación progresiva y reinicia la corrutina
     al completar el ciclo para repetir indefinidamente.
*/
public class TextColorLerp : MonoBehaviour
{
    /*
     Referencias serializadas
     - msg: componente Text cuyo color se interpolará.
     - duration: duración en segundos de la interpolación de color.
    */
    [SerializeField] Text msg;
    [SerializeField] float duration;

    void Start()
    {
        /*
         Start()
         -----------------
         - Inicia la corrutina ChangeColor que se encarga de la interpolación del color.
         - No realiza otra lógica; la corrutina controla el ciclo repetitivo.
        */
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        /*
         ChangeColor() - Corrutina
         -----------------
         - Interpola el color del Text desde negro a blanco en 'duration' segundos.
         - Al finalizar reinicia la corrutina para repetir el efecto de forma indefinida.
         - Contrato: se ejecuta asíncronamente y no devuelve un valor; gestiona el ciclo de color.
        */
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            // interpolamos el color del texto entre el negro y el blanco
            msg.color = Color.Lerp(Color.black, Color.white, t / duration);

            yield return null;
        }

        StartCoroutine(ChangeColor());
    }
}
