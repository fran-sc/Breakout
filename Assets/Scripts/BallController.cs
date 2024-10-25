using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;
    [SerializeField] GameController game;

    Rigidbody2D rb;
    Dictionary<string, int> bricks = new Dictionary<string, int> 
    {
        {"brick-r", 25},
        {"brick-a", 20},
        {"brick-g", 15},
        {"brick-y", 10}
    };
    
    void Start()
    {
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

        if (bricks.ContainsKey(tag))
        {
            // update score
            game.UpdateScore(bricks[tag]);

            // destroy brick
            Destroy(other.gameObject);
        }

        if (tag == "paddle")
        {
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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        if (tag == "wall-bottom")
        {
            // update lives
            game.UpdateLives(-1);
            
            // reset ball
            Invoke("LaunchBall", delay);
        }
    }
}
