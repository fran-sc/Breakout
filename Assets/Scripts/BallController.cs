using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 BallController
 -----------------
 Responsabilidad:
 - Controla el comportamiento de la bola en el juego (lanzamiento, colisiones y transición de nivel).
 - Gestiona reproducción de efectos de sonido según el tipo de colisión.
 - Lleva cuenta de impactos contra el paddle y ladrillos destruidos para ajustar velocidad y
     detectar final de nivel.

 Estructuras de datos y estado:
 - Campos serializados para asignar clips de audio y parámetros de física (force, delay, etc.).
 - Diccionario 'bricks' que mapea la etiqueta del ladrillo a los puntos que otorga al destruirse.
 - Variables internas: hitCount (nº impactos al paddle), brickCount (ladrillos destruidos),
     halved (si el paddle fue reducido), sceneId (índice de la escena actual).

 Notas sobre diseño:
 - Usa Rigidbody2D para física y corrutinas/Invoke para temporizaciones.
 - No realiza cambios globales directos aparte de llamar a GameController para actualizar
     puntuación y vidas.
*/
public class BallController : MonoBehaviour
{
    /*
     Referencias serializadas - clips de audio
     - sfxPaddle: sonido al golpear el paddle.
     - sfxBrick: sonido al destruir un ladrillo.
     - sfxWall: sonido al chocar con una pared.
     - sfxFail: sonido al perder una vida.
     - nextLevel: sonido de transición de nivel.
    */
    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;
    [SerializeField] AudioClip nextLevel;

    /*
     Ajustes serializados - parámetros de física y comportamiento
     - force: impulso inicial aplicado a la bola.
     - delay: tiempo de espera antes de lanzar la bola.
     - hitOffset: margen para ajustar inversión de dirección en el paddle.
     - forceInc: incremento de fuerza cada N impactos.
    */
    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;
    [SerializeField] float forceInc;

    // Componentes cacheados
    Rigidbody2D rb;    // cuerpo rígido 2D de la bola
    AudioSource sfx;   // fuente de audio para reproducir efectos

    // Estado de juego local
    int hitCount = 0;   // número de impactos contra el paddle
    int brickCount = 0; // número de ladrillos destruidos en este nivel
    GameObject paddle; // referencia al objeto paddle en escena
    bool halved = false; // indica si el paddle está reducido a la mitad
    int sceneId;        // id de la escena actual

    // Mapa de tipos de ladrillo a los puntos que otorgan al destruirse
    Dictionary<string, int> bricks = new Dictionary<string, int>
    {
        {"brick-r", 25},
        {"brick-a", 20},
        {"brick-g", 15},
        {"brick-y", 10},
        {"brick-pass", 25}
    };
    
    /*
        Start()
        -----------------
        - Inicializa estado local: obtiene el índice de la escena, cachea componentes
            (AudioSource, Rigidbody2D) y referencia al paddle por etiqueta.
        - Programa el lanzamiento de la bola tras 'delay' segundos.
    */    
    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;

        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        paddle = GameObject.FindGameObjectWithTag("paddle");

        Invoke("LaunchBall", delay);
    }

    /*
        LaunchBall()
        -----------------
        - Posiciona la bola en el origen y resetea su velocidad.
        - Elige una dirección X aleatoria (izquierda/derecha) y Y fija hacia arriba.
        - Aplica una fuerza inicial (impulso) para iniciar el movimiento.
        - Contrato: no toma parámetros, usa los campos serializados 'force' y 'delay'.
    */
    void LaunchBall()
    {

        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        float dirX, dirY = -1.0f;
        dirX = (Random.Range(0, 2) == 0) ? -1.0f : 1.0f;
        Vector2 dir = new Vector2(dirX, dirY).normalized;

        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    /*
        OnCollisionEnter2D(Collision2D)
        -----------------
        - Maneja colisiones físicas con diferentes tipos de objetos según su etiqueta.
        - Casos:
        * Ladrillos (según 'bricks'): destruye el ladrillo y actualiza puntuación.
        * Paddle: reproduce SFX, cuenta impactos, aumenta la fuerza cada N impactos
            y ajusta la dirección horizontal si el contacto lo requiere.
        * Paredes/rock: reproduce SFX y, si es la pared superior, reduce el paddle
            (solo una vez, controlado por 'halved').
        - Efectos secundarios: reproducción de sonidos, cambios en velocidad y tamaño del paddle.
    */
    void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;

        if (bricks.ContainsKey(tag))
        {
            DestroyBrick(other.gameObject);
        }
        else if (tag == "paddle")
        {
            sfx.clip = sfxPaddle;
            sfx.Play();

            hitCount++;
            if (hitCount % 4 == 0)
            {
                Debug.Log($"Hit count: {hitCount} --> Incrementando velocidad");
                rb.AddForce(rb.linearVelocity.normalized * forceInc, ForceMode2D.Impulse);
            }

            Vector3 paddle = other.transform.position;
            Vector2 contact = other.GetContact(0).point;

            if ((rb.linearVelocityX < 0 && contact.x > (paddle.x + hitOffset)) ||
                (rb.linearVelocityX > 0 && contact.x < (paddle.x - hitOffset)))
            {
                rb.linearVelocityX *= -1;
            }
        }
        else if (tag == "wall-top" || tag == "wall-lateral" || tag == "brick-rock")
        {
            sfx.clip = sfxWall;
            sfx.Play();

            if (!halved && tag == "wall-top")
            {
                HalvePaddle(true);
            }
        }
    }

    /*
        OnTriggerEnter2D(Collider2D)
        -----------------
        - Detecta triggers (por ejemplo, salida por debajo del área de juego).
        - Casos:
        * wall-bottom: pérdida de vida, reproducción de sonido, restauración del paddle
            si estaba reducido y relanzamiento de la bola tras 'delay'.
        * brick-pass: algunos ladrillos usan trigger para ser destruidos al contacto.
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "wall-bottom")
        {
            sfx.clip = sfxFail;
            sfx.Play();

            GameController.UpdateLives(-1);

            if (halved)
            {
                HalvePaddle(false);
            }

            Invoke("LaunchBall", delay);
        }
        else if (tag == "brick-pass")
        {
            DestroyBrick(other.gameObject);
        }
    }

    /*
        DestroyBrick(GameObject)
        -----------------
        - Ejecutado cuando la bola impacta un ladrillo que debe destruirse.
        - Efectos secundarios:
        * Reproduce el sonido de destrucción.
        * Actualiza la puntuación usando el diccionario 'bricks'.
        * Destruye el GameObject del ladrillo.
        * Incrementa 'brickCount' y, si se han destruido todos los ladrillos del nivel,
            inicia la secuencia de final de nivel (sonido, ocultar bola, cargar siguiente escena).
    */
    private void DestroyBrick(GameObject obj)
    {
        sfx.clip = sfxBrick;
        sfx.Play();

        GameController.UpdateScore(bricks[obj.tag]);

        Destroy(obj);

        brickCount++;
        if (brickCount == GameController.totalBricks[sceneId])
        {
            sfx.clip = nextLevel;
            sfx.Play();

            rb.linearVelocity = Vector2.zero;
            GetComponent<SpriteRenderer>().enabled = false;

            Invoke("NextScene", 3);
        }
    }

    /*
        HalvePaddle(bool)
        -----------------
        - Ajusta la escala del paddle para reducirla a la mitad o restaurarla.
        - Parámetro 'halve': true = reducir, false = restaurar al tamaño original duplicándolo.
        - Efecto secundario: actualiza la bandera 'halved'.
    */
    void HalvePaddle(bool halve)
    {
        halved = halve;

        Vector3 scale = paddle.transform.localScale;

        paddle.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z) :
            new Vector3(scale.x * 2.0f, scale.y, scale.z);
    }

    /*
        NextScene()
        -----------------
        - Calcula el siguiente índice de escena y lo carga.
        - Si la escena siguiente excede el conteo de escenas en build settings,
        vuelve a la escena 0 (loop de niveles).
    */
    void NextScene()
    {
        int nextId = sceneId + 1;

        if (nextId == SceneManager.sceneCountInBuildSettings)
        {
            nextId = 0;
        }

        SceneManager.LoadScene(nextId);
    }
}
