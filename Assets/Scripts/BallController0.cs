using System.Collections.Generic;
using UnityEngine;

public class BallController0 : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;

    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float delay;

    Rigidbody2D rb;
    AudioSource sfx;

    readonly List<string> bricks = new() {"brick-r", "brick-a", "brick-g", "brick-y"};

    void Start()
    {
        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        Invoke("LaunchBall", delay);
    }

    void LaunchBall()
    {
        // reset ball position and velocity
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        // get random direction
        float dirX, dirY = -1.0f;
        dirX = (Random.Range(0, 2) == 0) ? -1.0f : 1.0f;
        Vector2 dir = new Vector2(dirX, dirY).normalized;

        // apply force                
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
