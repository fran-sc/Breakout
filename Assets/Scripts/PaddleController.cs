using UnityEngine;

/*
 PaddleController
 -----------------
 Responsabilidad:
 - Controla el movimiento horizontal del paddle en respuesta a las teclas izquierda/derecha.
 - Restringe el movimiento dentro de los límites MAX_X y MIN_X para que no salga del área de juego.

 Notas sobre diseño:
 - La velocidad es configurable desde el Inspector mediante el campo 'speed'.
 - Usa transform.Translate con Time.deltaTime para movimiento dependiente del tiempo.
*/
public class PaddleController : MonoBehaviour
{
    /*
     Ajustes serializados
     - speed: velocidad de movimiento horizontal del paddle (unidades por segundo).
    */
    [SerializeField] float speed;

    // Límites horizontales del paddle (no debe salir del área de juego)
    const float MAX_X = 3.1f;
    const float MIN_X = -3.1f;
    
    /*
    Update()
    -----------------
    - Lee la entrada del usuario (teclas izquierda/derecha) y mueve el paddle en X.
    - Restringe el movimiento para que el paddle permanezca dentro de MIN_X..MAX_X.
    - Usa Time.deltaTime para que el movimiento sea independiente del framerate.
    */    
    void Update()
    {
        float x = transform.position.x;

        if (x > MIN_X && Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (x < MAX_X && Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
    }
}
