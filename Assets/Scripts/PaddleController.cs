using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] float speed;

    const float MAX_X = 3.1f;
    const float MIN_X = -3.1f;
    
    void Update()
    {
        float x = transform.position.x;

        if (x > MIN_X && Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (x < MAX_X && Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
    }
}
