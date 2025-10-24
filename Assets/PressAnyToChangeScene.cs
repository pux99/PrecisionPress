using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PressAnyToChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public bool active = true;
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
    public void ButtonInput(/*int code,*/InputAction.CallbackContext context)
    {
        if (active)
            SceneManager.LoadScene(sceneName);
    }
}
