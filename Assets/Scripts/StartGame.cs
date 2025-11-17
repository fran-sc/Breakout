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

    /*
    Start()
    -----------------
    Inicialización de la instancia:
    - Cachea componentes locales necesarios (AudioSource).
    - No realiza lógica de juego pesada; deja el inicio real para Update.
    */
    void Start()
    {
        // Cachear el componente AudioSource para reproducir efectos de sonido
        sfx = GetComponent<AudioSource>();
    }

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
    void Update()
    {
        // Detectar si se ha presionado cualquier tecla para iniciar el juego
        if (Input.anyKeyDown)       
        {
            // Ocultar la bola de la pantalla de inicio
            ball.SetActive(false);
            // Ocultar el mensaje de "presiona cualquier tecla"
            msg.enabled = false;
            // Reproducir efecto de sonido de inicio
            sfx.Play();

            // Iniciar la corrutina de transición al siguiente nivel
            StartCoroutine(NextLevel());
        }
    }

    /*
    NextLevel() - Corrutina
    -----------------
    - Anima la escala del paddle desde su escala actual hasta X=0 durante 'duration'.
    - Usa interpolación lineal (Lerp) basada en tiempo.
    - Efectos secundarios: al terminar carga la escena con buildIndex 1.
    - Contrato: no devuelve valor, se ejecuta de forma asíncrona hasta completion.
    */
    IEnumerator NextLevel()
    {
        float t = 0.0f;

        // Guardar escala inicial del paddle
        Vector3 scaleStart = paddle.localScale;
        // Definir escala final: ancho = 0, manteniendo alto y profundidad
        Vector3 scaleEnd = new Vector3(0.0f, scaleStart.y, scaleStart.z);

        // Bucle de animación durante 'duration' segundos
        while (t < duration)
        {
            t += Time.deltaTime;

            // Interpolar linealmente la escala del paddle desde inicio hasta fin
            // Esto crea un efecto visual de "cierre" del paddle
            paddle.localScale = Vector3.Lerp(scaleStart, scaleEnd, t/duration);
            
            yield return null;
        }

        // Al finalizar la animación, cargar la escena del primer nivel (buildIndex 1)
        SceneManager.LoadScene(1);
    }    
}
