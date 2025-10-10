using UnityEngine;
using UnityEngine.UI;

/*
 GUIController
 -----------------
 Responsabilidad:
 - Actualiza la interfaz de usuario (texto) con la puntuación y las vidas actuales.

 Notas sobre diseño:
 - Las referencias a los componentes Text se asignan desde el Inspector.
 - Usa OnGUI para actualizar el texto en cada frame; en proyectos más grandes se puede
     optimizar actualizando sólo cuando cambie el valor.
*/
public class GUIController : MonoBehaviour
{
    /*
     Referencias serializadas - componentes UI
     - txtScore: componente Text que muestra la puntuación formateada.
     - txtLives: componente Text que muestra el número de vidas restantes.
    */
    [Header("References")]
    [SerializeField] Text txtScore;
    [SerializeField] Text txtLives;

    void OnGUI()
    {
        /*
         OnGUI()
         -----------------
         - Actualiza los componentes Text con la puntuación y las vidas actuales en cada frame.
         - Nota: se podría optimizar actualizando sólo cuando cambien los valores.
        */
        txtScore.text = string.Format("{0,3:D3}", GameController.score);
        txtLives.text = GameController.lives.ToString();
    }
}
