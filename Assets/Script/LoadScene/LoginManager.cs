using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Text errorMessageText;

    public Button buttonMale;
    public Button buttonFemale;


    private void Start()
    {
        buttonMale.onClick.AddListener(() => OnButtonClick("Male"));
        buttonFemale.onClick.AddListener(() => OnButtonClick("Female"));
    }

    void OnButtonClick(string playerClass)
    {
        ////đọc thông tin ngừơi chơi va bắt lỗi
        //var playerName = nameInputField.text;

        // Đọc thông tin người chơi
        var playerName = nameInputField.text.Trim(); // Loại bỏ khoảng trắng đầu và cuối

        // Kiểm tra nếu tên trống
        if (string.IsNullOrEmpty(playerName))
        {
            // Hiển thị thông báo lỗi (giả sử có một Text UI để báo lỗi)
            errorMessageText.text = "Vui lòng nhập tên nhân vật!";
            return;
        }

        //lưu thông tin người chơi
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("PlayerClass", playerClass);
        SceneManager.LoadScene("Main");
    }    
}
