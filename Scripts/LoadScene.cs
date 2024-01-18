using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] string SceneName;

    public void Load()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
