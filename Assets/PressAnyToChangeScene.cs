using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PressAnyToChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Fade fade;
    private bool Fadetarted=false;

    public bool active = true;

    private void Start()
    {
        fade.FinishFade += FinishFirstFade;
        fade.FadeIn();
    }

    private void FinishFirstFade()
    {
        fade.FinishFade -= FinishFirstFade;
        fade.FinishFade += ChangeScene;
    }

    public void ButtonInput( /*int code,*/ InputAction.CallbackContext context)
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
                if (!Fadetarted)
                {
                    fade.FadeOut();
                    Fadetarted=true;
                }
                break;
        }
    }

    private void ChangeScene()
    {
        fade.FinishFade -= ChangeScene;
        SceneManager.LoadScene(sceneName);
    }

}