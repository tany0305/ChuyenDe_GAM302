using Fusion;
using UnityEngine;
using Invector.vCharacterController;

public class CharacterNetworkHandler : NetworkBehaviour
{
    private vThirdPersonController controller;
    private vThirdPersonInput input;

    private int jumpCount = 0;
    private int maxJumps = 2; // Cho phép nhảy 2 lần

    public AudioSource audioSource;
    public AudioClip footstepClip;
    public AudioClip jumpClip;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f;

    public override void Spawned()
    {
        controller = GetComponent<vThirdPersonController>();
        input = GetComponent<vThirdPersonInput>();

        if (controller == null)
            Debug.LogError("vThirdPersonController is NULL on " + gameObject.name);
        if (input == null)
            Debug.LogError("vThirdPersonInput is NULL on " + gameObject.name);

        if (!Object.HasInputAuthority)
        {
            // Tắt input với player không phải local
            input.enabled = false;
        }
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            // Lấy input từ player local
            var moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var jumpInput = Input.GetButton("Jump");

            // Gửi input lên mạng
            RPC_SetInput(moveInput, jumpInput);

            // Phát âm thanh bước chân nếu đang chạy và chạm đất
            if (controller != null && controller.isGrounded && moveInput.magnitude > 0.1f)
            {
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    PlayFootstepSound();
                    footstepTimer = 0f;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }

        // Kiểm tra nếu đã chạm đất => reset jump count
        if (controller != null && controller.isGrounded && jumpCount > 0)
        {
            jumpCount = 0;
        }

    }

    private void PlayFootstepSound()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpClip != null)
        {
            audioSource.PlayOneShot(jumpClip);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetInput(Vector2 moveInput, bool jumpInput)
    {
        if (controller != null)
        {
            controller.input.x = moveInput.x;
            controller.input.y = moveInput.y;
            if (jumpInput && jumpCount < maxJumps)
            {
                controller.Jump();
                jumpCount++; // Nhảy xong thì tăng đếm
                PlayJumpSound();
            }
        }
    }

    private void CheckComponents()
    {
        controller = GetComponent<vThirdPersonController>();
        input = GetComponent<vThirdPersonInput>();

        if (controller == null || input == null)
            Debug.LogError($"[CharacterNetworkHandler] Missing components on {gameObject.name}");
    }
}
