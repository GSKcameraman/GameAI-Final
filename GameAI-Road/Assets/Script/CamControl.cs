using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;

    public void SwitchCam()
    {
        if (cam1.activeSelf)
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
        }
        else
        {
            cam2.SetActive(false);
            cam1.SetActive(true);
        }
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
