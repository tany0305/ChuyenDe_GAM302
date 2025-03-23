using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera virtualcamera;
    public void AssignCamera(Transform playertransform)
    {
        virtualcamera.Follow = playertransform;
        virtualcamera.LookAt = playertransform;
    }
}
