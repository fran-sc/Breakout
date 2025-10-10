using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 StartGame
 -----------------
 Responsabilidad:
 - Gestiona el inicio de la partida cuando el jugador pulsa cualquier tecla.
 - Anima el paddle (reducción de escala) y cambia la escena al nivel siguiente.
 - Reproduce el efecto de sonido de inicio.

 Notas sobre diseño:
 - Las referencias a escena (paddle, ball, msg) se asignan desde el Inspector mediante [SerializeField].
 - La duración de la animación es configurable vía el campo 'duration'.
 - Se usa una corrutina para interpolar la escala del paddle antes de cargar la escena.
*/
public class StartGame : MonoBehaviour
{
    /*
     Referencias serializadas
     - paddle: Transform del paddle que se anima al comenzar la partida.
     - ball: GameObject de la bola que se oculta cuando arranca el nivel.
     - msg: Componente Text que muestra el mensaje de 'presiona cualquier tecla'.
    */
    [Header("References")]
    [SerializeField] Transform paddle;
    [SerializeField] GameObject ball;
    [SerializeField] Text msg;

    /*
     Ajustes serializados
     - duration: tiempo en segundos que dura la animación de escala del paddle.
    */
    [Header("Settings")]
    [SerializeField] float duration; // duración de la animación de inicio

    // AudioSource local para reproducir sonido de inicio
    AudioSource sfx;

    void Start()
    {
        /*
         Start()
         -----------------
         Inicialización de la instancia:
         - Cachea componentes locales necesarios (AudioSource).
         - No realiza lógica de juego pesada; deja el inicio real para Update.
        */
        sfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        /*
         Update()
         -----------------
         Observa las entradas del jugador en cada frame.
         - Si se pulsa cualquier tecla, inicia la secuencia de comienzo:
           * Oculta la bola y el mensaje de inicio.
           * Reproduce el sonido de inicio.
           * Lanza la corrutina NextLevel para animar y cambiar de escena.
         - Ejecuta rápidamente; no debe bloquear.
        */
        if (Input.anyKeyDown)       
        {
            ball.SetActive(false);
            msg.enabled = false;
            sfx.Play();

            StartCoroutine(NextLevel());
        }
    }

    IEnumerator NextLevel()
    {
        /*
         NextLevel() - Corrutina
         -----------------
         - Anima la escala del paddle desde su escala actual hasta X=0 durante 'duration'.
         - Usa interpolación lineal (Lerp) basada en tiempo.
         - Efectos secundarios: al terminar carga la escena con buildIndex 1.
         - Contrato: no devuelve valor, se ejecuta de forma asíncrona hasta completion.
        */
        float t = 0.0f;

        Vector3 scaleStart = paddle.localScale;
        Vector3 scaleEnd = new Vector3(0.0f, scaleStart.y, scaleStart.z);

        while (t < duration)
        {
            t += Time.deltaTime;

            // ajustamos el escalado de la barrera
            paddle.localScale = Vector3.Lerp(scaleStart, scaleEnd, t/duration);
            
            yield return null;
        }

        SceneManager.LoadScene(1);
    }    
}
