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

    /*
    Start()
    -----------------
    - Inicia la corrutina ChangeColor que se encarga de la interpolación del color.
    - No realiza otra lógica; la corrutina controla el ciclo repetitivo.
    */
    void Start()
    {
        // Iniciar la corrutina que anima el color del texto
        StartCoroutine(ChangeColor());
    }

    /*
    ChangeColor() - Corrutina
    -----------------
    - Interpola el color del Text desde negro a blanco en 'duration' segundos.
    - Al finalizar reinicia la corrutina para repetir el efecto de forma indefinida.
    - Contrato: se ejecuta asíncronamente y no devuelve un valor; gestiona el ciclo de color.
    */
    IEnumerator ChangeColor()
    {
        float t = 0.0f;

        // Interpolar color del texto de negro a blanco durante 'duration' segundos
        while (t < duration)
        {
            t += Time.deltaTime;

            // Interpolación lineal entre Color.black (inicio) y Color.white (fin)
            // Crea un efecto de "fade in" del texto
            msg.color = Color.Lerp(Color.black, Color.white, t / duration);

            yield return null;
        }

        // Al completar el ciclo, reiniciar la corrutina para efecto continuo
        StartCoroutine(ChangeColor());
    }
}
