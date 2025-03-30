using UnityEngine;
using Fusion;
public class PlayerMoments : NetworkBehaviour
{
   public CharacterController Controller;
   public float Speed;

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasStateAuthority) return;
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var move = transform.right * x + transform.forward * y;
        Controller.Move(move * Speed * Time.fixedDeltaTime );
    }
}
