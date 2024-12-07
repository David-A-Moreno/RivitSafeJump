using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildStructure : MonoBehaviour
{
    private bool allThorns = false;
    public bool isActive;

    private Effects effectsScript;

    private LilyPadManager lilyPadManager;

    private ProgressiveBuild progressiveBuild;

    void Awake()
    {
        effectsScript = FindObjectOfType<Effects>();
        lilyPadManager = FindObjectOfType<LilyPadManager>();
        progressiveBuild = FindObjectOfType<ProgressiveBuild>();
        isActive = false;
    }

    public void SetAllThorns(bool allThorns)
    {
        this.allThorns = allThorns;
    }

    public bool GetAllThorns()
    {
        return allThorns;
    }

    public void InstantiateLilypadsBasedOnLevel()
    {
        string[] lilypadPrefabs;
        if (Random.value < 0.2f)
        {
            lilypadPrefabs = new string[] { "Bonus2", "Bonus3" };
        }
        else
        {
            lilypadPrefabs = new string[] { "Bonus1", "Bonus2", "Bonus3" };
        }
        int count = lilyPadManager.GetCurrentLilyPadCount();
        Vector3[] positions = lilyPadManager.GetCurrentLilyPadPositions();
        InstantiateLilypads(lilypadPrefabs, positions, count);
    }

    void InstantiateRandomStructure()
    {
        List<string> availableOptions = new List<string> { "Structure1", "Structure2", "Structure3", "Structure4" };

        int randomIndexOption = Random.Range(0, availableOptions.Count);
        InstantiatePrefab(availableOptions[randomIndexOption], new Vector3(0, 0, 0));
    }


    private void InstantiateRiver()
    {
        InstantiatePrefab("River", new Vector3(4, -0.2f, -0.5f));
    }

    private void InstantiateLilypads(string[] prefabs, Vector3[] positions, int count)
    {
        if (allThorns)
        {
            InstantiateLilypadsWithoutCorrectOptions(positions);
        }
        else
        {
            InstantiateLilypadsWithCorrectOptions(prefabs, positions, count);
        }
    }

    private void InstantiateLilypadsWithCorrectOptions(string[] prefabs, Vector3[] positions, int count)
    {
        // Limitar count a la longitud de positions para evitar accesos fuera de los límites del array
        count = Mathf.Min(count, positions.Length);
        float randomScale = lilyPadManager.GetCurrentLilyPadScale();

        List<int> availablePositions = new List<int>();
        for (int i = 0; i < count; i++)
        {
            availablePositions.Add(i);
        }
        GameObject prefab;
        List<string> availableOptions = prefabs.ToList();

        while (availablePositions.Count != 0)
        {
            int randomIndexOption = Random.Range(0, availableOptions.Count);
            int randomIndex = Random.Range(0, availablePositions.Count);
            int optionPosition = availablePositions[randomIndex];
            prefab = InstantiatePrefab(availableOptions[randomIndexOption], positions[optionPosition]);
            prefab.SetActive(isActive);

            // Aplicar escala aleatoria si es Nivel 3 o superior
            if (progressiveBuild.GetLevel() >= 3)
            {
                //effectsScript.SetTargetScale(randomScale);
            }

            availablePositions.RemoveAt(randomIndex);
        }
    }

    private void InstantiateLilypadsWithoutCorrectOptions(Vector3[] positions)
    {
        bool changeMaterial = Random.value < 0.4f;
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject prefab = InstantiatePrefab("Thorns", positions[i]);
            prefab.SetActive(isActive);

            // Nivel 5: Probabilidad del 50% de cambiar los materiales de las espinas
            if (progressiveBuild.GetLevel() == 5 && changeMaterial)
            {
                if (Random.value < 0.5f)
                {
                    //ChangeThornMaterials(prefab);
                }
            }
        }
    }

    private void ChangeThornMaterials(GameObject thorn)
    {
        // Seleccionar un Bonus aleatorio y aplicar sus dos materiales correspondientes
        int bonusIndex = Random.Range(1, 4); // Bonus1, Bonus2 o Bonus3
        string material1 = $"Bonus{bonusIndex}Material1";
        string material2 = $"Bonus{bonusIndex}Material2";

        // Cargar los materiales desde Resources
        Material mat1 = Resources.Load<Material>("Materials/" + material1);
        Material mat2 = Resources.Load<Material>("Materials/" + material2);

        if (mat1 == null || mat2 == null)
        {
            Debug.LogError("Error: No se pudieron cargar los materiales.");
            return;
        }

        // Aplicar los materiales a los hijos "Thorn" y "Thorn (1)"
        foreach (Transform child in thorn.transform)
        {
            if (child.name == "Thorn" || child.name == "Thorn (1)")
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material[] materials = renderer.materials;
                    materials[0] = mat1;
                    materials[1] = mat2;
                    renderer.materials = materials;
                }
                else
                {
                    Debug.LogWarning("El objeto " + child.name + " no tiene un Renderer.");
                }
            }
        }
    }

    GameObject InstantiatePrefab(string prefabName, Vector3 position)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            instance.transform.parent = this.transform;
            return instance;
        }
        else
        {
            Debug.LogError("Prefab not found: " + prefabName);
            return null;
        }
    }
}
