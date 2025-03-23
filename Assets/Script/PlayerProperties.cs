using UnityEngine;
using Fusion;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float health { get; set; }
    public float maxHealth { get; set; }

    public TextMeshProUGUI healthText;

    public void OnHealthChanged()
    {
        healthText.text = $"{health}/{maxHealth}";
    }

}   
