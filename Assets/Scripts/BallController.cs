using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Text txtGameOver;

    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;
    [SerializeField] AudioClip nextLevel;

    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;
    [SerializeField] float forceInc;
    [SerializeField] float durationGameOver;

    Rigidbody2D rb;
    AudioSource sfx;
    int hitCount = 0;   // number of hits on the paddle
    int brickCount = 0; // number of bricks destroyed
    GameObject paddle;
    bool halved = false; // flag to check if paddle size is halved
    int sceneId;

    Dictionary<string, int> bricks = new Dictionary<string, int> 
    {
        {"brick-r", 25},
        {"brick-a", 20},
        {"brick-g", 15},
        {"brick-y", 10},
        {"brick-pass", 25}
    };
    
    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;

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
            DestroyBrick(other.gameObject);
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
        else if (tag == "wall-top" || tag == "wall-lateral" || tag == "brick-rock")
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
            GameController.UpdateLives(-1);

            // restore paddle size
            if (halved)
            {
                HalvePaddle(false);
            }
            
            if (GameController.lives == 0)
            {
                StartCoroutine(GameOver());
            }
            else
            {
                // reset ball position and velocity
                Invoke("LaunchBall", delay);
            }
        }
        else if (tag == "brick-pass")
        {
            DestroyBrick(other.gameObject);
        }
    }

    private void DestroyBrick(GameObject obj)
    {
        // play sound
        sfx.clip = sfxBrick;
        sfx.Play();

        // update score
        GameController.UpdateScore(bricks[obj.tag]);

        // destroy brick
        Destroy(obj);

        // update destroyed brick count
        brickCount++;
        if (brickCount == GameController.totalBricks[sceneId])
        {
            // play level transition sound
            sfx.clip = nextLevel;
            sfx.Play();

            // reset ball velocity and hide it
            rb.linearVelocity = Vector2.zero;
            GetComponent<SpriteRenderer>().enabled = false;

            // load next level
            Invoke("NextScene", 3);
        }
    }

    void HalvePaddle(bool halve)
    {
        halved = halve;

        Vector3 scale = paddle.transform.localScale;

        paddle.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z) :
            new Vector3(scale.x * 2.0f, scale.y, scale.z);
    }

    void NextScene()
    {   
        int nextId = sceneId + 1;

        if (nextId == SceneManager.sceneCountInBuildSettings)
        {
            nextId = 0;
        }

        SceneManager.LoadScene(nextId);
    }

    IEnumerator GameOver()
    {
        // show game over message
        txtGameOver.gameObject.SetActive(true);

        // fade in game over message
        float t = 0.0f;
        while (t < durationGameOver)
        {
            t += Time.deltaTime;

            txtGameOver.color = Color.Lerp(Color.black, Color.white, t / durationGameOver);

            yield return null;
        }

        // load initial scene
        SceneManager.LoadScene(0);
    }
}
