using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PressAnyToChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public bool active = true;

    public void ButtonInput(/*int code,*/InputAction.CallbackContext context)
    {
        if (!active) return;
        if (!context.performed) return;

        switch (context.action.name)
        {
            case "Form1":
            case "Form2":
            case "Form3":
            case "Form4":
            case "Form5":
            case "Form6":
                SceneManager.LoadScene(sceneName);
                break;
        }
    }
}
