using System.Collections.Generic;
using UnityEngine;

/*
 GameController
 -----------------
 Responsabilidad:
 - Mantiene el estado global del juego: vidas, puntuación y conteo total de ladrillos por nivel.
 - Proporciona métodos estáticos para actualizar puntuación y vidas desde otros componentes.

 Estructuras de datos:
 - 'lives' y 'score' como enteros estáticos de acceso público (get) y modificables sólo
     mediante métodos estáticos para centralizar cambios.
 - 'totalBricks' es una lista con el número total de ladrillos por buildIndex de escena.

 Notas sobre diseño:
 - Al ser estático, el estado puede ser accedido desde cualquier script sin necesidad de
     referencias de instancia. Esto facilita updates desde colisiones y controladores de nivel.
*/
public class GameController : MonoBehaviour
{
    // Estado global del juego (accesible estáticamente)
    public static int lives {get; private set;} = 3; // vidas restantes
    public static int score {get; private set;} = 0; // puntuación acumulada

    // Cantidad total de ladrillos por escena (index = buildIndex)
    public static List<int> totalBricks {get; private set;} = new List<int> { 0, 32, 32};

    /*
     UpdateScore(int)
     -----------------
     - Incrementa la puntuación global en la cantidad indicada.
     - Parámetro: points (positivo para sumar puntos).
     - Efecto secundario: modifica el estado estático 'score'.
    */
    public static void UpdateScore(int points)
    {
        score += points;
    }

    /*
     UpdateLives(int)
     -----------------
     - Modifica el contador de vidas por el valor indicado (puede ser negativo).
     - Parámetro: numLives (por ejemplo, -1 para restar una vida).
     - Efecto secundario: modifica el estado estático 'lives'.
    */
    public static void UpdateLives(int numLives)
    {
        lives += numLives;
    }
}
