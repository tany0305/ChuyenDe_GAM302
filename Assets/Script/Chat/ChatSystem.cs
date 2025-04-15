using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : NetworkBehaviour
{
    public TextMeshProUGUI textMassage;
    public TMP_InputField inputField;
    public GameObject buttonSend;
    public GameObject chatPanel;  // Thêm tham chiếu tới panel chat

    //chạy game sau khi nhân vật đc spawn
    public override void Spawned()
    {

        textMassage = GameObject.Find("Text Message").GetComponent<TextMeshProUGUI>();
        inputField = GameObject.Find("InputField").GetComponentInParent<TMP_InputField>();
        buttonSend = GameObject.Find("Button Send");
        chatPanel = GameObject.Find("Panel chat");  // Lấy reference tới Chat Panel

        /*// Ẩn chat panel khi game bắt đầu
        if (chatPanel != null)
        {
            chatPanel.SetActive(false);  // Ẩn panel chat ngay khi game bắt đầu
        }
        else
        {
            Debug.LogError("Chat panel is not assigned!");
        }*/

        buttonSend.GetComponent<Button>().onClick.AddListener(SendMassageChat);

    }

    public void SendMassageChat()
    {
        var massage = inputField.text;
        if (string.IsNullOrWhiteSpace(massage)) return;
        var id = Runner.LocalPlayer.PlayerId;
        var text = $"Player {id}: {massage}";
        RpcChat(text);
        inputField.text = "";
    }

    //Sources: gửi từ đầu
    //Targets: đối tượng nhận
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string msg)
    {
        textMassage.text += msg + "\n";
    }
}