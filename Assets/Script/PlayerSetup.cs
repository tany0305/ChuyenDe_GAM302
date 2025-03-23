using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public void SetupCamera()
    {

        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.AssignCamera(transform);
        }
    }
}
