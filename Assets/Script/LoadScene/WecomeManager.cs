using UnityEngine;
using UnityEngine.SceneManagement;

public class WecomeManager : MonoBehaviour
{
    private void Start()
    {
        // Kiểm tra nếu đang ở scene "Welcome" thì mới chạy Invoke
        if (SceneManager.GetActiveScene().name == "Wecome")
        {
            Invoke(nameof(LoadScene), 2f);
        }
    }

    void LoadScene()
    {
        // Chỉ load scene nếu đang ở "Welcome"
        if (SceneManager.GetActiveScene().name == "Wecome")
        {
            SceneManager.LoadScene("Login");
        }
    }
}
