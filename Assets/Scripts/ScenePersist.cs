using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    int sceneBuildIndexAtStart;

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        int numScenePersist = FindObjectsOfType<ScenePersist>().Length;
        if (numScenePersist > 1)
        {            
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        sceneBuildIndexAtStart = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        bool amInNewScene = sceneBuildIndexAtStart != SceneManager.GetActiveScene().buildIndex;

        if (amInNewScene)
        {
            Destroy(gameObject);
        }
    }
}
