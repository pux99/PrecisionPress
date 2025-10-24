using System.Collections.Generic;
using UnityEngine;

public class ButtonsTest : MonoBehaviour
{
    [System.Serializable]
    public class ButtonObjectPair
    {
        public KeyCode button;
        public GameObject targetObject;
    }

    [SerializeField] private List<ButtonObjectPair> buttonObjectPairs = new List<ButtonObjectPair>();

    private void Update()
    {
        foreach (var pair in buttonObjectPairs)
        {
            if (pair.targetObject == null) continue;

            if (Input.GetKeyDown(pair.button))
            {
                pair.targetObject.SetActive(true);
            }
            else if (Input.GetKeyUp(pair.button))
            {
                pair.targetObject.SetActive(false);
            }
        }
    }
}
