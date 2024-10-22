using System.Globalization;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] CinemachineCamera playerCamera;
    

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCamera.Priority = 1;
        }
        else
        {
            playerCamera.Priority = 0;
        }
    }

}
