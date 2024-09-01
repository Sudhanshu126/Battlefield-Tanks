using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;

    [Header("Values")]
    [SerializeField] private int ownerCameraPriority;
    [SerializeField] private int nonOwnerCameraPriority;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCamera.Priority = ownerCameraPriority;
        }
        else
        {
            playerCamera.Priority = nonOwnerCameraPriority;
        }
    }
}
