using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform paddle;
    [SerializeField] GameObject ball;
    [SerializeField] Text msg;

    [Header("Settings")]
    [SerializeField] float duration;

    AudioSource sfx;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    void Update()
    {
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
