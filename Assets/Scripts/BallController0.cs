using System.Collections.Generic;
using UnityEngine;

/*
 BallController0
 -----------------
 Responsabilidad:
 - Versión simplificada de BallController que maneja lanzamiento y reproducción de
     efectos de sonido al colisionar con distintos objetos.

 Notas sobre diseño:
 - Mantiene una lista inmutable de etiquetas de ladrillos para identificar colisiones.
 - Se centra en la reproducción de sonidos; no realiza conteo de puntos ni transiciones.
*/
public class BallController0 : MonoBehaviour
{
    /*
     Referencias serializadas - clips de audio
     - sfxPaddle: sonido al rebotar en el paddle.
     - sfxBrick: sonido al chocar con un ladrillo.
     - sfxWall: sonido al chocar con pared lateral.
    */
    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;

    /*
     Ajustes serializados
     - force: fuerza de lanzamiento de la bola.
     - delay: retraso antes de lanzar la bola al iniciar.
    */
    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float delay;

    // Componentes cacheados
    Rigidbody2D rb; // Rigidbody2D de la bola
    AudioSource sfx; // AudioSource para reproducir efectos

    // Lista de etiquetas que identifican ladrillos (sin valores asociados)
    readonly List<string> bricks = new() {"brick-r", "brick-a", "brick-g", "brick-y"};

    /*
        Start()
        -----------------
        - Cachea componentes locales (AudioSource, Rigidbody2D) y programa el lanzamiento
        de la bola tras 'delay' segundos.
    */
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        Invoke("LaunchBall", delay);
    }

    /*
        LaunchBall()
        -----------------
        - Reinicia posición y velocidad de la bola, elige una dirección aleatoria
        y aplica un impulso inicial usando el parámetro 'force'.
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

    void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;

        if (bricks.Contains(tag))
        {
            sfx.clip = sfxBrick;
        }
        else if (tag == "paddle")
        {
            sfx.clip = sfxPaddle;
        }
        else if (tag == "wall-lateral")
        {
            sfx.clip = sfxWall;
        }
        sfx.Play();
    }
}
