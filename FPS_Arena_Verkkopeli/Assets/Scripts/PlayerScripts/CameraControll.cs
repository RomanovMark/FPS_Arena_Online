using UnityEngine;
using System.Collections;
using Unity.Netcode;


public class CameraControll : NetworkBehaviour
{

    public Camera cam; 

    void Start()
    {

        if (!IsLocalPlayer)
            cam.enabled = false;

        else
            return;
    
    }

}