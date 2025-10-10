# Documentación de métodos clave

Este documento explica con más detalle métodos específicos clave del proyecto Breakout.

Se incluyen contratos, efectos secundarios y fragmentos de código extraídos del código actual.

## 1) Lanzamiento de la pelota — `LaunchBall()` (en `BallController`)

Responsabilidad:

- Posicionar la bola en el centro del campo y aplicarle un impulso inicial para iniciar el movimiento.

Contrato / comportamiento:

- No recibe parámetros.

- Usa los campos serializados `force` y `delay` para determinar la fuerza aplicada y los tiempos de lanzamiento.

- Efectos secundarios: resetea posición/velocidad de la bola y aplica una fuerza (impulso) al `Rigidbody2D`.

Código (extracto):

```csharp
void LaunchBall()
{
    /*
     LaunchBall()
     -----------------
     - Posiciona la bola en el origen y resetea su velocidad.
     - Elige una dirección X aleatoria (izquierda/derecha) y Y fija hacia arriba.
     - Aplica una fuerza inicial (impulso) para iniciar el movimiento.
     - Contrato: no toma parámetros, usa los campos serializados 'force' y 'delay'.
    */
    transform.position = Vector3.zero;
    rb.linearVelocity = Vector2.zero;

    float dirX, dirY = -1.0f;
    dirX = (Random.Range(0, 2) == 0) ? -1.0f : 1.0f;
    Vector2 dir = new Vector2(dirX, dirY).normalized;

    rb.AddForce(dir * force, ForceMode2D.Impulse);
}
```

Notas y recomendaciones:

- El método elige `dirY = -1.0f` en el código actual; si quieres que la bola vaya "hacia arriba" en pantalla, verificar la orientación de la escena (en algunas configuraciones Y negativa apunta hacia abajo).

- Para variar la dificultad, ajustar `force` y `delay` desde el Inspector.


## 2) Colisiones con ladrillos y final de nivel — `DestroyBrick(GameObject)` y `NextScene()` (en `BallController`)

Responsabilidad:

- `DestroyBrick`: gestionar la destrucción de un ladrillo tras el impacto de la bola; actualizar la puntuación y comprobar si se ha completado el nivel.

- `NextScene`: calcular y cargar la escena siguiente cuando el nivel termina.

Contrato / comportamiento:

- `DestroyBrick` recibe el `GameObject` del ladrillo como parámetro.

- Actualiza la puntuación usando el diccionario `bricks` (mapa etiqueta -> puntos).

- Destruye el GameObject del ladrillo y aumenta `brickCount`.

- Si `brickCount` alcanza `GameController.totalBricks[sceneId]`, reproduce `nextLevel` y llama a `NextScene` con un retraso.

Código (extracto):

```csharp
private void DestroyBrick(GameObject obj)
{
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

void NextScene()
{
    /*
     NextScene()
     -----------------
     - Calcula el siguiente índice de escena y lo carga.
     - Si la escena siguiente excede el conteo de escenas en build settings,
       vuelve a la escena 0 (loop de niveles).
    */
    int nextId = sceneId + 1;

    if (nextId == SceneManager.sceneCountInBuildSettings)
    {
        nextId = 0;
    }

    SceneManager.LoadScene(nextId);
}
```

Notas y recomendaciones:

- `GameController.totalBricks` se usa para detectar el final del nivel; asegúrate de que los conteos por escena estén actualizados.

- El `Invoke("NextScene", 3)` deja 3 segundos para reproducir efectos/animaciones; puedes reducir o parametrizar este tiempo.

- Considera usar un sistema de eventos para desacoplar la lógica (por ejemplo, emitir `OnLevelComplete` en lugar de invocar `NextScene` directamente).


## 3) Interpolación de color repetitiva — `ChangeColor()` (en `TextColorLerp`)

Responsabilidad:

- Interpolar periódicamente el color de un `Text` entre negro y blanco durante `duration` segundos y repetir indefinidamente.

Contrato / comportamiento:

- Es una corrutina que no recibe parámetros; usa `msg` y `duration` serializados.

- Efectos secundarios: modifica la propiedad `msg.color` en cada frame mientras la corrutina está activa.

Código (extracto):

```csharp
IEnumerator ChangeColor()
{
    /*
     ChangeColor() - Corrutina
     -----------------
     - Interpola el color del Text desde negro a blanco en 'duration' segundos.
     - Al finalizar reinicia la corrutina para repetir el efecto de forma indefinida.
     - Contrato: se ejecuta asíncronamente y no devuelve un valor; gestiona el ciclo de color.
    */
    float t = 0.0f;

    while (t < duration)
    {
        t += Time.deltaTime;

        // interpolamos el color del texto entre el negro y el blanco
        msg.color = Color.Lerp(Color.black, Color.white, t / duration);

        yield return null;
    }

    StartCoroutine(ChangeColor());
}
```

Notas y recomendaciones:

-- En su forma actual, la corrutina se relanza recursivamente al terminar, lo que está bien pero puede ser más explícito utilizar un bucle externo `while(true)` para repetir indefinidamente.

- Para suavizar el efecto hacia atrás (blanco -> negro), se puede alternar la interpolación o usar `Mathf.PingPong` con un solo `Lerp`.

### Posibles mejoras comunes

- Parametrizar los delays y duraciones para sintonizar la jugabilidad desde el Inspector.

- Usar eventos C# para desacoplar la lógica (por ejemplo, publicación de `OnBrickDestroyed` o `OnLevelComplete`).

- Añadir tests unitarios para la lógica no gráfica (por ejemplo, funciones que calculen puntuaciones o estados de nivel).
