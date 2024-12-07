using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    private bool inGame = true;

    public void GameOver()
    {
        inGame = false;
    }

    public void DestroyOptionEffect(GameObject parent)
    {
        // Iterar sobre todos los hijos del GameObject
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag("Thorns") ||
                child.CompareTag("Bonus1") ||
                child.CompareTag("Bonus2") ||
                child.CompareTag("Bonus3"))
            {
                // Obtener el nombre del hijo para construir la ruta del prefab
                string childTag = child.tag;  // Usar el tag del hijo, asegúrate de asignar el tag adecuado
                string prefabPath = $"Prefabs/{childTag}DestroyEffect";

                // Cargar el prefab desde la ruta especificada
                GameObject destroyEffectPrefab = Resources.Load<GameObject>(prefabPath);

                if (destroyEffectPrefab != null)
                {
                    // Instanciar el prefab en la nueva posición
                    GameObject destroyEffectInstance = Instantiate(destroyEffectPrefab, child.position, Quaternion.identity);

                    // Destruir el prefab después de un tiempo (si es necesario), por ejemplo, después de 5 segundos
                    Destroy(destroyEffectInstance, 5f);
                }
                else
                {
                    Debug.LogWarning($"Prefab not found at path: {prefabPath}");
                }

                // Destruir el hijo original
                Destroy(child.gameObject);
            }
        }
    }

    public void AppearOptionEffect(GameObject parent)
    {
        if (inGame)
        {
            // Iterar sobre todos los hijos del GameObject
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag("Thorns") ||
                    child.CompareTag("Bonus1") ||
                    child.CompareTag("Bonus2") ||
                    child.CompareTag("Bonus3"))
                {
                    // Activar el objeto hijo
                    child.gameObject.SetActive(true);

                    // Iniciar la animación de aparición
                    StartCoroutine(ScaleUp(child));
                }
            }
        }
    }

    public void DisappearOptionEffect(GameObject parent, GameObject chosenOption)
    {
        if (inGame)
        {
            // Iterar sobre todos los hijos del GameObject
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag("Thorns") ||
                    child.CompareTag("Bonus1") ||
                    child.CompareTag("Bonus2") ||
                    child.CompareTag("Bonus3"))
                {
                    if (child != chosenOption.transform)
                    {
                        StartCoroutine(ScaleDown(child));
                    }
                    // Iniciar la animación de aparición
                }
            }
        }
    }

    // Coroutine para escalar el objeto progresivamente
    private IEnumerator ScaleUp(Transform child)
    {
        Vector3 initialScale = new Vector3(0.03f, 0.03f, 0.03f); // Escala inicial
        Vector3 targetScale = new Vector3(0.13f, 0.13f, 0.13f);
        float duration = 0.08f; // Duración de la animación en segundos
        float elapsed = 0.0f;

        // Escalar progresivamente desde initialScale hasta targetScale en el tiempo 'duration'
        while (elapsed < duration)
        {
            child.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que la escala final sea exactamente la objetivo
        child.localScale = targetScale;
    }

    private IEnumerator ScaleDown(Transform child)
    {
        Vector3 initialScale = child.localScale; // Escala inicial (la escala actual del objeto)
        Vector3 targetScale = new Vector3(0, 0, 0); // Escala pequeña (desaparecer)
        float duration = 0.08f; // Duración de la animación en segundos
        float elapsed = 0.0f;

        // Escalar progresivamente desde initialScale hasta targetScale en el tiempo 'duration'
        while (elapsed < duration)
        {
            child.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que la escala final sea exactamente la objetivo
        child.localScale = targetScale;
    }


}
