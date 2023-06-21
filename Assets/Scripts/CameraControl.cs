using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class CameraControl : MonoBehaviour
{
    private bool winShooted;
    
    public CinemachineVirtualCamera topDownCam;
    public CinemachineVirtualCamera winCam;

    public void WinShoot(Transform winnerStacker)
    {
        if (!winShooted)
        {
            winCam.LookAt = winnerStacker;

            winCam.gameObject.SetActive(true);
            topDownCam.gameObject.SetActive(false);

            winShooted = true;
        }
        
    }


}
