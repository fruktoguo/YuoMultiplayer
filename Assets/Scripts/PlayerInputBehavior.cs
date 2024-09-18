using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

/// <summary>
/// 优化后的网络化玩家输入行为脚本，基于 NetworkRigidbody 处理玩家的移动、旋转输入以及传送操作，并通过网络同步到其他客户端。
/// 使用 NetworkTransform 组件处理传送的同步。
/// </summary>
[RequireComponent(typeof(NetworkRigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerInputBehavior : NetworkBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 5f; // 玩家移动速度

    [SerializeField] private float rotateSpeed = 720f; // 玩家旋转速度（度/秒）
    [SerializeField] private float teleportCooldown = 5f; // 传送冷却时间（秒）
    [SerializeField] private float inputThreshold = 0.1f; // 输入阈值，减少微小输入
    [SerializeField] private float teleportCheckRadius = 0.5f; // 传送位置周围检查半径
    [SerializeField] private LayerMask teleportObstacleLayers; // 传送障碍物图层
    public Vector3 PlayerCenterOffset;
    private float lastTeleportTime = -Mathf.Infinity; // 上一次传送时间

    private Rigidbody rb;
    private NetworkTransform networkTransform;

    // 输入缓冲区
    private Vector3 inputMoveDirection = Vector3.zero;
    private float inputRotateAmount;

    // 预先分配的碰撞体数组，避免每次调用时分配新的数组
    private Collider[] overlapResults = new Collider[10]; // 根据需求调整大小

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkTransform = GetComponent<NetworkTransform>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            // 可以在这里绑定其他初始化逻辑
        }
    }

    /// <summary>
    /// 每帧调用一次，处理输入。
    /// 使用 FixedUpdate 处理物理输入更为合适。
    /// </summary>
    private void Update()
    {
        if (!IsOwner) return;

        HandleInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // 发送移动和旋转数据的固定间隔
        if (inputMoveDirection.sqrMagnitude > 0 || Mathf.Abs(inputRotateAmount) > 0)
        {
            SubmitMovementAndRotationServerRpc(inputMoveDirection, inputRotateAmount);
            // 重置输入缓冲区
            inputMoveDirection = Vector3.zero;
            inputRotateAmount = 0f;
        }
    }

    /// <summary>
    /// 处理玩家的输入，包括移动、旋转和传送。
    /// 将输入存储在缓冲区中，以便在 FixedUpdate 中统一发送。
    /// </summary>
    private void HandleInput()
    {
        // 获取水平和垂直输入（例如：键盘的A/D和W/S键）
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 创建一个标准化的移动方向向量
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.sqrMagnitude >= inputThreshold * inputThreshold)
        {
            // 不在客户端进行缩放，留给服务器处理
            inputMoveDirection += moveDirection;
        }

        // 处理旋转输入（按下Q键左转，按下E键右转）
        float rotateInput = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotateInput -= 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotateInput += 1f;
        }

        if (Mathf.Abs(rotateInput) >= inputThreshold)
        {
            // 不在客户端进行缩放，留给服务器处理
            inputRotateAmount += rotateInput;
        }

        // 处理传送输入（例如按下T键）
        if (Input.GetKeyDown(KeyCode.T))
        {
            TryTeleport();
        }
    }

    /// <summary>
    /// 尝试进行传送操作，检查冷却时间并发送请求到服务器。
    /// </summary>
    private void TryTeleport()
    {
        if (Time.time < lastTeleportTime + teleportCooldown)
        {
            Debug.Log("传送冷却中，请稍后再试。");
            return;
        }

        Vector3 teleportTarget = GetTeleportTargetPosition();

        if (teleportTarget == transform.position)
        {
            Debug.Log("传送目标位置无效，未进行传送。");
            return;
        }

        RequestTeleportServerRpc(teleportTarget);
        lastTeleportTime = Time.time;
    }

    /// <summary>
    /// 获取传送目标位置的方法。
    /// 这里以鼠标点击位置为例。
    /// </summary>
    /// <returns>传送目标位置</returns>
    private Vector3 GetTeleportTargetPosition()
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point + PlayerCenterOffset;
            }
        }

        return transform.position; // 返回当前玩家位置，表示无效
    }

    /// <summary>
    /// 服务器远程过程调用（ServerRpc），接收客户端的移动和旋转请求，并更新 Rigidbody。
    /// </summary>
    /// <param name="move">移动向量（未缩放）</param>
    /// <param name="rotate">旋转量（未缩放）</param>
    [ServerRpc(RequireOwnership = true)]
    private void SubmitMovementAndRotationServerRpc(Vector3 move, float rotate)
    {
        // 安全性验证：限制移动和旋转的最大值
        float maxMovePerFrame = moveSpeed * Time.fixedDeltaTime * 2; // 允许一定的缓冲
        float maxRotatePerFrame = rotateSpeed * Time.fixedDeltaTime * 2;

        // 计算实际移动量
        Vector3 actualMove = move * moveSpeed * Time.fixedDeltaTime;

        if (actualMove.magnitude > maxMovePerFrame)
        {
            actualMove = actualMove.normalized * maxMovePerFrame;
            Debug.LogWarning($"玩家{OwnerClientId}尝试超出移动限制，已调整移动量。");
        }

        // 计算实际旋转量
        float actualRotate = rotate * rotateSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(actualRotate) > maxRotatePerFrame)
        {
            actualRotate = Mathf.Sign(actualRotate) * maxRotatePerFrame;
            Debug.LogWarning($"玩家{OwnerClientId}尝试超出旋转限制，已调整旋转量。");
        }

        // 使用 Rigidbody 移动和旋转
        rb.MovePosition(rb.position + actualMove);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, actualRotate, 0));
    }

    /// <summary>
    /// 服务器远程过程调用（ServerRpc），处理客户端的传送请求，并更新 Rigidbody 的位置。
    /// </summary>
    /// <param name="teleportTarget">传送目标位置</param>
    [ServerRpc(RequireOwnership = true)]
    private void RequestTeleportServerRpc(Vector3 teleportTarget)
    {
        if (!IsValidTeleportPosition(teleportTarget))
        {
            Debug.LogWarning($"玩家{OwnerClientId}请求了一个非法的传送位置：{teleportTarget}");
            return;
        }

        // 使用 NetworkTransform 的 Teleport 方法实现传送
        networkTransform.Teleport(teleportTarget, rb.rotation, rb.transform.localScale);

        // 重置 Rigidbody 的速度和角速度，防止传送后继续移动
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 使用 ClientRpc 通知所有客户端立即传送到新位置，跳过插值
        TeleportClientRpc(teleportTarget, rb.rotation);
    }

    /// <summary>
    /// 客户端远程过程调用（ClientRpc），在所有客户端上立即设置 Rigidbody 的位置和旋转，跳过插值。
    /// </summary>
    /// <param name="newPosition">新的位置</param>
    /// <param name="newRotation">新的旋转</param>
    [ClientRpc]
    private void TeleportClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        if (networkTransform != null)
        {
            networkTransform.Teleport(newPosition, newRotation, transform.localScale);
        }
        else
        {
            // 备选方案：直接设置 Rigidbody
            rb.position = newPosition;
            rb.rotation = newRotation;
        }
    }

    /// <summary>
    /// 验证传送目标位置是否合法的方法。
    /// 根据具体需求实现，例如检查是否在地图范围内、是否穿过墙体等。
    /// </summary>
    /// <param name="position">传送目标位置</param>
    /// <returns>是否合法</returns>
    private bool IsValidTeleportPosition(Vector3 position)
    {
        // 使用 OverlapSphereNonAlloc 检测目标位置周围是否有障碍物
        int colliderCount = Physics.OverlapSphereNonAlloc(position, teleportCheckRadius, overlapResults,
            teleportObstacleLayers, QueryTriggerInteraction.Ignore);
        if (colliderCount > 0)
        {
            return false;
        }

        // 可选：添加地图边界检查
        // Example:
        /*
        float mapSize = 50f;
        if (position.x < -mapSize || position.x > mapSize || position.z < -mapSize || position.z > mapSize)
        {
            return false;
        }
        */

        return true;
    }
}