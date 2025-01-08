using System.Collections.Generic;
using UnityEngine;

public class LilyPadManager : MonoBehaviour
{
    private Effects effectsScript;

    private Dictionary<int, List<Vector3[]>> lilyPadPositions = new Dictionary<int, List<Vector3[]>>()
    {
        // Opciones de lilyPads con 4 posiciones
        [4] = new List<Vector3[]>
        {
            new Vector3[] { new Vector3(-1.95f, 0, 0), new Vector3(-0.65f, 0, 0), new Vector3(0.68f, 0, 0), new Vector3(1.99f, 0, 0) },
            new Vector3[] { new Vector3(-1.95f, 0.5f, 0), new Vector3(-0.65f, -0.24f, 0), new Vector3(0.68f, 0.14f, 0), new Vector3(1.99f, 0.41f, 0) },
            new Vector3[] { new Vector3(-1.95f, 0.5f, 0), new Vector3(-0.65f, 0.5f, 0), new Vector3(0.68f, -0.1f, 0), new Vector3(1.99f, -0.1f, 0) },
            new Vector3[] { new Vector3(-1.95f, 0.5f, 0), new Vector3(-0.65f, -0.1f, 0), new Vector3(0.68f, -0.1f, 0), new Vector3(1.99f, 0.5f, 0) },
            new Vector3[] { new Vector3(-1.87f, -0.6f, 0), new Vector3(-0.86f, 0.54f, 0), new Vector3(0.8f, -0.6f, 0), new Vector3(1.99f, 0.5f, 0) },
            new Vector3[] { new Vector3(-1.95f, -0.58f, 0), new Vector3(-0.65f, -0.23f, 0), new Vector3(0.68f, 0.1f, 0), new Vector3(1.99f, 0.37f, 0) }
        },
        // Opciones de lilyPads con 3 posiciones
        [3] = new List<Vector3[]>
        {
            new Vector3[] { new Vector3(-1.6f, 0, 0), new Vector3(0, 0, 0), new Vector3(1.6f, 0, 0) },
            new Vector3[] { new Vector3(-1.85f, -0.1f, 0), new Vector3(0, 0.5f, 0), new Vector3(1.85f, -0.1f, 0) },
            new Vector3[] { new Vector3(-1.85f, 0.5f, 0), new Vector3(0, 0, 0), new Vector3(1.85f, -0.5f, 0) },
            new Vector3[] { new Vector3(-1.35f, 0.5f, 0), new Vector3(0, -0.5f, 0), new Vector3(1.35f, 0.5f, 0) },
            new Vector3[] { new Vector3(-1.35f, 0.4f, -1.86f), new Vector3(0, 0.4f, 0), new Vector3(1.35f, 0.4f, 0) }
        },
        // Opciones de lilyPads con 2 posiciones
        [2] = new List<Vector3[]>
        {
            new Vector3[] { new Vector3(-1.2f, 0, 0), new Vector3(1.2f, 0, 0) },
            new Vector3[] { new Vector3(-0.7f, -0.5f, 0), new Vector3(0.7f, 0, 0) },
            new Vector3[] { new Vector3(-1.8f, 0.5f, 0), new Vector3(1.8f, -0.5f, 0) },
            new Vector3[] { new Vector3(0.176f, 0, 0), new Vector3(1.52f, 0,0) }
        },
        [1] = new List<Vector3[]>
        {
            new Vector3[] { new Vector3(0, 0, 0) }
        }
    };

    private Vector3[] currentPositions;
    private int currentCount;
    private float currentScale;

    void Awake()
    {
        // Para niveles 1 a 3, establecer número y posición predeterminada, sin variación de escala
        currentCount = 3;
        currentPositions = new Vector3[] { new Vector3(-1.6f, 0, 0), new Vector3(0, 0, 0), new Vector3(1.6f, 0, 0) };
        currentScale = 1.0f; // Escala predeterminada
        effectsScript = FindObjectOfType<Effects>();
    }


    public void InitializeLevel(int level)
    {
        Debug.Log("nivel: " + level);
        if (level == 1)
        {
            // Para niveles 1 a 3, establecer número y posición predeterminada, sin variación de escala
            currentCount = 3;
            currentPositions = lilyPadPositions[currentCount][0];
            currentScale = 0.13f; // Escala predeterminada
        }
        else if (level == 2)
        {
            currentCount = 3;
            RandomPosition(currentCount);
        }
        else if (level == 3)
        {
            currentCount = 3;
            RandomPosition(currentCount);
            RandomScale();
        }
        else
        {
            RandomCount();
            RandomPosition(currentCount);
            RandomScale();
        }
    }

    public void RandomScale()
    {
        currentScale = Random.Range(0.09f, 0.145f); // Ejemplo de escala variable para niveles altos
        effectsScript.targetScaleUp = Vector3.one * currentScale;
    }

    public void RandomCount()
    {
        currentCount = Random.Range(1, 5);
    }

    public void RandomPosition(int count)
    {
        int optionIndex = Random.Range(0, lilyPadPositions[currentCount].Count);
        currentPositions = lilyPadPositions[currentCount][optionIndex];
    }

    public int GetCurrentLilyPadCount()
    {
        return currentCount;
    }

    public Vector3[] GetCurrentLilyPadPositions()
    {
        return currentPositions;
    }

    public float GetCurrentLilyPadScale()
    {
        return currentScale;
    }

    public void ResetManager()
    {
        currentCount = 3;
        currentPositions = lilyPadPositions[currentCount][0];
        currentScale = 1.0f;
    }
}
