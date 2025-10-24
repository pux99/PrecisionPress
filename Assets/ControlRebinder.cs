using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class ControlRebinder : MonoBehaviour
{
    [Header("Assign your InputActions for forms 1 through 6")]
    public InputActionReference[] formActions;

    [Header("Optional UI")]
    public TMP_Text rebindingStatus;
    public string originalText;
    public PressAnyToChangeScene pressAnyToChangeScene;

    [ContextMenu("start rebinding")]
    public void StartFormRebinding()
    {
        rebindingStatus.text = originalText;
        pressAnyToChangeScene.active = false;
        pressAnyToChangeScene.enabled = false;
        if (formActions == null || formActions.Length < 6)
        {
            Debug.LogError("Assign 6 InputActionReferences for the forms.");
            return;
        }
        StartCoroutine(RebindFormsOneByOne());
    }
    
    private IEnumerator RebindFormsOneByOne()
    {
        for (int i = 0; i < formActions.Length; i++)
        {
            if (rebindingStatus != null)
                rebindingStatus.text = $"Press a key for Form {i + 1}";

            bool rebindComplete = false;
            var action = formActions[i].action;
            action.Disable();

            var rebindOp = action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(op =>
                {
                    action.Enable();
                    op.Dispose();
                    rebindComplete = true;
                })
                .Start();

            // Wait until the user presses a key and rebinding completes before proceeding
            yield return new WaitUntil(() => rebindComplete);
            yield return new WaitForSeconds(1);
        }

        if (rebindingStatus != null)
            rebindingStatus.text = $"All forms rebound!";
        yield return new WaitForSeconds(2);
        rebindingStatus.text = originalText;
        pressAnyToChangeScene.enabled = true;
        pressAnyToChangeScene.active = true;
    }
}