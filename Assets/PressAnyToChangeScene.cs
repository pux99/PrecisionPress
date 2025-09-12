using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyToChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void Update()
    {
        KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                SceneManager.LoadScene(sceneName);
                break;
            }
        }
    }
}
