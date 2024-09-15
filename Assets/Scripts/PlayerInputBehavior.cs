using Unity.Netcode;
using UnityEngine;

public class PlayerInputBehavior : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private void Update()
    {
        if (IsOwner)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        if (move != Vector3.zero)
        {
            MoveServerRpc(move);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 move)
    {
        transform.position += move;
    }
}
