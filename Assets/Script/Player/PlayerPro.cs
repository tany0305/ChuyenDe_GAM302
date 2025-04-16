using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Unity.Cinemachine;

public class PlayerPro : NetworkBehaviour
{
    public CinemachineCamera FollowCamera;

    [Networked, OnChangedRender(nameof(OnInfoChanged))]
    public string PlayerName {  get; set; }
    [Networked, OnChangedRender(nameof(OnInfoChanged))]
    public int Health { get; set; } = 100;

    [Networked, OnChangedRender(nameof(OnInfoChanged))]
    public int MaxHealth { get; set; } = 100;

    Animator anim;
    [Networked, OnChangedRender(nameof(OnAnimationChanged))]
    public bool Slash { get; set; } = false;

    public GameObject Weapon;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;

    private void OnAnimationChanged()
    {
        anim.SetTrigger("Slash");
    }

    private void OnInfoChanged()
    {
        nameText.text = PlayerName;
        healthText.text = $"Health: {Health} / {MaxHealth}";
    }
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        FollowCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    public override void FixedUpdateNetwork()
    {
        base.Spawned();
        if (Object.HasInputAuthority && FollowCamera != null)
        {
            FollowCamera.Follow = transform;
            FollowCamera.LookAt = transform;
        }

        if (Object.HasInputAuthority)
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
            nameText.text = PlayerName;
        }

        if (HasInputAuthority)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Slash = !Slash;
                Weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcTakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log("Player received damage. Health: " + Health);

        if (Health <= 0)
        {
        }
        // Cập nhật lại hiển thị máu
        healthText.text = $"Health: {Health} / {MaxHealth}";
    }
}

