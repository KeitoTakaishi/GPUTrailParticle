using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    int camID = 0;
    [SerializeField] GameObject[] cam;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            camID = (camID + 1) % 2;
        }
        if(camID == 0)
        {
            cam[0].active = true;
            cam[1].active = false;

        } else
        {
            cam[1].active = true;
            cam[0].active = false;
        }
    }
}
