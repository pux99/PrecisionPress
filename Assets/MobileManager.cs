using System.Runtime.InteropServices;
using UnityEngine;

public class MobileManager : MonoBehaviour
{
    [SerializeField] private GameObject helper;
    [SerializeField] private GameObject mobileBottons;
    void Start()
    {
        if (IsMobile())
        {
            helper.SetActive(false);
            mobileBottons.SetActive(true);
        }
        else
        {
            mobileBottons.SetActive(false);
        }
    }
    [DllImport("__Internal")]
        private static extern int IsMobileBrowser();
    
        public bool IsMobile()
        {
    #if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobileBrowser() == 1;
    #else
            return false;
    #endif
        }
}
