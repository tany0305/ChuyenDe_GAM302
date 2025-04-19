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

    [Networked, OnChangedRender(nameof(OnAnimationChanged))]
    public bool Slash { get; set; } = false;

    Animator anim;
    AudioSource audioSource;
    
    public GameObject Weapon;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public AudioClip slashClip;
    private bool isSlashing = false;
    //public AudioClip moveClip;

    private void OnAnimationChanged()
    {
        anim.SetTrigger("Slash");
        // Phát âm thanh nếu không đang trong hoạt ảnh Slash
        if (!isSlashing && audioSource != null && slashClip != null)
        {
            audioSource.PlayOneShot(slashClip);
            isSlashing = true;
        }
    }
    private IEnumerator DelayUIUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        OnInfoChanged();
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
        audioSource = GetComponent<AudioSource>();
        if (Object.HasStateAuthority)
        {
            // Gán lại để trigger OnChangedRender
            PlayerName = PlayerName;
            Health = Health;
            MaxHealth = MaxHealth;
        }
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

                
                // Gọi hàm phát âm thanh chém
                PlaySound(slashClip);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }

    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcTakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log("Player received damage. Health: " + Health);

        if (Health <= 0)
        {
            Debug.Log("Player is dead. Stopping the game...");

            // Dừng toàn bộ thời gian game
            Time.timeScale = 0f;
        }
        // Cập nhật lại hiển thị máu
        healthText.text = $"Health: {Health} / {MaxHealth}";
    }

}

