using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameController game;

    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;

    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;
    [SerializeField] float forceInc;

    Rigidbody2D rb;
    AudioSource sfx;
    int hitCount = 0;   // number of hits on the paddle
    GameObject paddle;
    bool halved = false; // flag to check if paddle size is halved

    Dictionary<string, int> bricks = new Dictionary<string, int> 
    {
        {"brick-r", 25},
        {"brick-a", 20},
        {"brick-g", 15},
        {"brick-y", 10}
    };
    
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        paddle = GameObject.FindGameObjectWithTag("paddle");

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

        if (bricks.ContainsKey(tag))
        {
            // play sound
            sfx.clip = sfxBrick;
            sfx.Play();

            // update score
            game.UpdateScore(bricks[tag]);

            // destroy brick
            Destroy(other.gameObject);
        }
        else if (tag == "paddle")
        {
            // play sound
            sfx.clip = sfxPaddle;
            sfx.Play();

            // update hit count and velocity
            hitCount++;
            if (hitCount % 4 == 0)
            {
                Debug.Log($"Hit count: {hitCount} --> Incrementando velocidad");
                rb.AddForce(rb.linearVelocity.normalized * forceInc, ForceMode2D.Impulse);
            }

            // get paddle position
            Vector3 paddle = other.transform.position;

            // get hit position
            Vector2 contact = other.GetContact(0).point;
            
            // set ball direction
            if ((rb.linearVelocityX < 0 && contact.x > (paddle.x + hitOffset)) || 
                (rb.linearVelocityX > 0 && contact.x < (paddle.x - hitOffset)))
            {
                rb.linearVelocityX *= -1;
            }
        }
        else if (tag == "wall-top" || tag == "wall-lateral")
        {
            // play sound
            sfx.clip = sfxWall;
            sfx.Play();

            // hit on top wall
            if (!halved && tag == "wall-top")
            {
                // reduce paddle size
                HalvePaddle(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        if (tag == "wall-bottom")
        {
            // play sound
            sfx.clip = sfxFail;
            sfx.Play();

            // update lives
            game.UpdateLives(-1);

            // restore paddle size
            if (halved)
            {
                HalvePaddle(false);
            }
            
            // reset ball
            Invoke("LaunchBall", delay);
        }
    }

    private void HalvePaddle(bool halve)
    {
        halved = halve;

        Vector3 scale = paddle.transform.localScale;

        paddle.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z) :
            new Vector3(scale.x * 2.0f, scale.y, scale.z);
    }
}
