using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoEndSwitcher : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject logoCanvas;
    public void ShowMenu()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(true);
        logoCanvas.SetActive(false);
    }
}
