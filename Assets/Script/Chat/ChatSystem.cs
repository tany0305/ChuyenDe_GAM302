using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : NetworkBehaviour
{
    public TextMeshProUGUI textMassage;
    public TMP_InputField inputField;
    public GameObject buttonSend;

    //chạy game sau khi nhân vật đc spawn
    public override void Spawned()
    {
        textMassage = GameObject.Find("Text Message").GetComponent<TextMeshProUGUI>();
        inputField = GameObject.Find("InputField").GetComponentInParent<TMP_InputField>();
        buttonSend = GameObject.Find("Button Send");
        buttonSend.GetComponent<Button>().onClick.AddListener(SendMassageChat);
    }

    public void SendMassageChat()
    {
        var massage = inputField.text;
        if(string.IsNullOrWhiteSpace(massage)) return;
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
        textMassage.text = msg + "\n";
    }    

    
}
