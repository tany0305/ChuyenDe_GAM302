using UnityEngine;

public class ChatPanelToggle : MonoBehaviour
{
    public GameObject chatPanel;  // Panel chứa UI chat
    public UnityEngine.UI.Button toggleButton; // Nút để bật/tắt giao diện chat

    void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleChatPanel);
        }
        else
        {
            Debug.LogError("Toggle button is not assigned!");
        }
    }

    // Hàm để bật/tắt chat panel
    public void ToggleChatPanel()
    {
        if (chatPanel != null)
        {
            chatPanel.SetActive(!chatPanel.activeSelf);  // Đảo ngược trạng thái hiện tại của panel
        }
        else
        {
            Debug.LogError("Chat panel is not assigned!");
        }
    }
}