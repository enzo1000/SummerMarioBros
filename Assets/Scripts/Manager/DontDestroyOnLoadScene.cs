using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScene : MonoBehaviour
{

    public GameObject[] objects;
    public static DontDestroyOnLoadScene instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DontDestroyOnLoadScene found!");
            return;
        }
        instance = this;

        foreach (GameObject obj in objects)
        {
            DontDestroyOnLoad(obj);
        }
    }

    public void RemoveFromDDOL()
    {
        foreach (GameObject obj in objects)
        {
            SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
        }
    }
}
