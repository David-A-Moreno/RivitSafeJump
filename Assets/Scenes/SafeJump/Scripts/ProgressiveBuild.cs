using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveBuild : MonoBehaviour
{
    [SerializeField]
    private GameObject[] initialStructures = new GameObject[6];

    [SerializeField]
    private GameObject gameLight;

    [SerializeField]
    private GameObject dome;

    [SerializeField]
    private GameObject cloud;

    [SerializeField]
    private Movement movement; //Movement.cs
    
    [SerializeField]
    private Effects effects; //Effects.cs

    [SerializeField]
    private SafeJumpManager safeJumpManager;
    /*
    [SerializeField]
    private GameOverManager gameOverScript;

    [SerializeField]
    private GameStartUI gameStartUI;

    [SerializeField]
    private AudioFX audioFX;
    */

    [SerializeField]
    private LilyPadManager lilyPadManager;

    [SerializeField]
    private AudioSource music;

    private GameObject lastStructure = null;

    private int stepsProgress = 0;
    private int structuresCreated = 0;
    private int destroyedStructures = 0;
    private int playerLevel = 1;

    public bool gameOver { get; set; } = false;
    Vector3 position = new Vector3(0, 0, 0);

    Vector3 resetPosition = new Vector3(0, 0, 0);

    // Lista para almacenar todas las estructuras
    public List<GameObject> structures = new List<GameObject>();

    private Coroutine currentCoroutine;

    // Constantes
    private const float positionOffsetY = 2.59f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStructures();
        //gameStartUI.showGameStartUI();
    }

    private void InitializeStructures()
    {
        Vector3 targetPosition = new Vector3(0, -1.18f, 0);
        foreach (var structure in initialStructures)
        {
            var buildStructure = structure.GetComponent<BuildStructure>();
            if (playerLevel != GetInitialPlayerLevel(structure))
            {
                playerLevel = GetInitialPlayerLevel(structure);
                lilyPadManager.InitializeLevel(playerLevel);
            }
            bool allThorns = UnityEngine.Random.value < 0.2f;
            if (allThorns && structure != initialStructures[0] && structure != initialStructures[1] && structure != initialStructures[2])
            {
                buildStructure.SetAllThorns(allThorns);
            }
            Console.WriteLine(allThorns);
            buildStructure.InstantiateLilypadsBasedOnLevel();
            /*
            if (!gameStartUI.getFirstGame())
            {
                appearFirstOptions();
            }
            */

            if (System.Array.IndexOf(initialStructures, structure) == 0)
            {
                appearFirstOptions();
            }


            structure.transform.position = targetPosition;
            structures.Add(structure);
            targetPosition.y += positionOffsetY;
        }
        position = targetPosition;
        structuresCreated = 6;
    }

    public void appearFirstOptions()
    {
        effects.AppearOptionEffect(initialStructures[0]);
    }

    private int GetInitialPlayerLevel(GameObject structure)
    {
        int index = System.Array.IndexOf(initialStructures, structure);
        return (index < 7) ? 1 : 2;
    }

    public void OneStep()
    {
        stepsProgress++;
        structuresCreated++;
        //Inicializar nueva estructura
        GameObject randomStructure = InstantiateStructure();
        if (randomStructure == null) return;

        //Agregar nivel al juego
        SetPlayerLevel();

        //Probabilidad del 10% de que la opcion sea NO-GO
        bool allThorns = UnityEngine.Random.value < 0.15f;
        randomStructure.GetComponent<BuildStructure>().SetAllThorns(allThorns);

        //Crear la estructura
        randomStructure.GetComponent<BuildStructure>().InstantiateLilypadsBasedOnLevel();

        //Posicionar la estructura
        randomStructure.transform.position = position;

        //Posicionar el domo
        structures.Add(randomStructure);
        if (stepsProgress > 3)
        {
            RemoveFirstStructure();
        }

        //Guardar la posicion de la siguiente estructura
        position.y += positionOffsetY;

        if (allThorns && playerLevel > 4)
        {
            if (structures.Count > 1) // se asegura de que hay al menos dos estructuras en la lista
            {
                GameObject penultimateStructure = structures[structures.Count - 2];
                BuildStructure penultimateBuild = penultimateStructure.GetComponent<BuildStructure>();
                if (penultimateBuild != null && !penultimateBuild.GetAllThorns())
                {
                    // Ejecutar logica para penultima estructura con "allThorns"
                    lilyPadManager.InitializeLevel(playerLevel);
                }
            }
        }
    }

    private void SetPlayerLevel()
    {
        int newLevel;
        if (structuresCreated < 10)
        {
            newLevel = 1;
        }
        else if (structuresCreated >= 10 && structuresCreated < 20)
        {
            newLevel = 2;
        }
        else if (structuresCreated >= 20 && structuresCreated < 30)
        {
            newLevel = 3;
        }
        else if (structuresCreated >= 40 && structuresCreated < 50)
        {
            newLevel = 4;
        }
        else
        {
            newLevel = 5;
        }
        

        if (playerLevel != newLevel)
        {
            playerLevel = newLevel;
            lilyPadManager.InitializeLevel(playerLevel);
        }
    }


    public void AutomaticMoveTimer()
    {
        // Cancela la corutina actual si est� en ejecuci�n
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        // Inicia la corutina que controla el tiempo de espera seg�n el stepsProgress
        StartCoroutine(CheckMoveStatus());
    }

    GameObject InstantiateStructure()
    {
        List<string> availableOptions = new List<string> { "Structure1", "Structure2", "Structure3", "Structure4" };

        int randomIndexOption = UnityEngine.Random.Range(0, availableOptions.Count);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + availableOptions[randomIndexOption]);
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, resetPosition, Quaternion.identity);
            instance.transform.parent = this.transform;
            return instance;
        }
        else
        {
            Debug.LogError("Prefab not found: " + "Structure");
            return null;
        }
    }

    // M�todo para eliminar la primera estructura de la lista
    public void RemoveFirstStructure()
    {
        if (structures.Count > 0)
        {
            GameObject firstStructure = structures[0];
            structures.RemoveAt(0);
            Destroy(firstStructure);
            destroyedStructures += 1;
        }
        else
        {
            Debug.LogWarning("No structures to remove.");
        }
    }

    public void DissapearOtherOptions(GameObject chosenOption)
    {
        GameObject currentStructure = GetNextStructure();
        GameObject lastStructure = GetLastStructure();
        effects.DisappearOptionEffect(currentStructure, chosenOption);
        effects.DisappearOptionEffect(lastStructure, chosenOption);
    }

    // Corutina para verificar el estado de "move" y esperar el tiempo necesario

    private IEnumerator CheckMoveStatus()
    {
        GameObject nextStructure = GetNextStructure();
        if (!gameOver)
        {
            effects.AppearOptionEffect(nextStructure);
        }
        float waitTime = 2.7f;
        bool allThorns = nextStructure.GetComponent<BuildStructure>().GetAllThorns();
        if (!allThorns)
        {
            if (stepsProgress < 10) 
            {
                movement.SetSpeed(4);
            }
            else if (stepsProgress == 10)
            {
                waitTime = 2f;
                movement.SetSpeed(6);
            }
            else if (stepsProgress == 20)
            {
                waitTime = 1.5f;
                movement.SetSpeed(10);
            }
            else if (stepsProgress == 30)
            {
                waitTime = 1f;
                movement.SetSpeed(16);
            }
            else if (stepsProgress == 40)
            {
                waitTime = 1f;
                movement.SetSpeed(20);
            }
        }
        else
        {
            waitTime = 1.5f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            // Si "move" es true, se cancela la corutina
            if (movement.GetMove())
            {
                yield break; // Termina la corutina si "move" est� en true
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Busca el GameObject con el tag "FreePath" dentro de la estructura en la posici�n stepsProgress+1
        SetAutomaticMovement(nextStructure, "Thorns2", allThorns);
    }

    private GameObject GetNextStructure()
    {
        if (stepsProgress != 1)
        {
            return structures[stepsProgress - destroyedStructures];
        }
        else
        {
            return structures[stepsProgress];
        }
    }

    private GameObject GetLastStructure()
    {
        if (stepsProgress > 1)
        {
            return structures[stepsProgress - destroyedStructures - 1];
        }
        else
        {
            return structures[0];
        }
    }

    public void SetAutomaticMovement(GameObject targetStructure, string targetTag, bool allThorns)
    {
        GameObject target;
        if (allThorns)
        {
            target = FindChildWithTag(targetStructure, "Thorns");
        }
        else
        {
            if (FindChildWithTag(targetStructure, "Bonus1") != null)
            {
                target = FindChildWithTag(targetStructure, "Bonus1");
            }
            else if (FindChildWithTag(targetStructure, "Bonus2") != null)
            {
                target = FindChildWithTag(targetStructure, "Bonus2");
            }
            else
            {
                target = FindChildWithTag(targetStructure, "Bonus3");
            }
            lastStructure = targetStructure;
        }
        Vector3 newPosition = target.transform.position;
        newPosition.x = 0;

        //effects.DestroyOptionEffect(targetStructure);
        StartCoroutine(WaitAndExecute(targetStructure, allThorns, newPosition));
    }

    private IEnumerator WaitAndExecute(GameObject targetStructure, bool allThorns, Vector3 targetPosition)
    {
        // Destruir el efecto
        effects.DestroyOptionEffect(targetStructure);

        //audioFX.PlaySound(5);

        float waitingTime = (allThorns) ? 0.5f : 1f;

        // Esperar 2 segundos
        yield return new WaitForSeconds(waitingTime);

        // AquI continua el codigo que se ejecutar despues de la espera de 2 segundos
        if (stepsProgress > 0)
        {
            if (allThorns)
            {
                movement.MoveToTarget(targetPosition, "");
            }
            else
            {
                if (!gameOver)
                {
                    gameOver = true;
                    safeJumpManager.FinishGame();
                }
                //gameOverScript.GameOver(targetStructure.transform.position, true);
                //audioFX.PlaySound(4);
                //music.mute = true;
            }
        }
    }

    public void ActivateAllChildren(GameObject parent)
    {
        // Iteramos sobre cada transform hijo del objeto padre
        foreach (Transform child in parent.transform)
        {
            // Activamos el GameObject hijo
            child.gameObject.SetActive(true);
        }
    }

    public void DeactivateAllChildren()
    {
        if (lastStructure != null)
        {
            // Iteramos sobre cada transform hijo del objeto padre
            foreach (Transform child in lastStructure.transform)
            {
                GameObject childGameObject = child.gameObject;

                if (childGameObject.CompareTag("Thorns") ||
                    childGameObject.CompareTag("Bonus1") ||
                    childGameObject.CompareTag("Bonus2") ||
                    childGameObject.CompareTag("Bonus3"))
                {
                    // Desactivamos el GameObject hijo
                    childGameObject.SetActive(false);
                }
            }
            lastStructure = null;
        }
    }

    public GameObject[] FindChildrenWithTag(GameObject parent, string tag)
    {
        // Obtenemos todos los componentes Transform dentro del padre (incluyendo los hijos)
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>();

        // Lista para almacenar los GameObjects con el tag deseado
        List<GameObject> childrenWithTag = new List<GameObject>();

        // Recorremos cada transform y verificamos el tag
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag(tag))
            {
                childrenWithTag.Add(child.gameObject);
            }
        }

        // Convertimos la lista a un array y lo retornamos
        return childrenWithTag.ToArray();
    }

    // M�todo auxiliar para buscar un hijo por tag
    private GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public int GetLevel()
    {
        return playerLevel;
    }
}