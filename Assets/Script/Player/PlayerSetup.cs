using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public void SetupCamera()
    {

        CameraFollow cameraFollow = FindFirstObjectByType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.AssignCamera(transform);
        }
    }
}
