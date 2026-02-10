using UnityEngine;

/// <summary>
/// MOBA 风格相机控制器
/// 核心功能：目标跟随、屏幕边缘平移、鼠标右键旋转视角、滚轮调节距离、可切换左右视角
/// </summary>
public class MobaCamera : MonoBehaviour
{
    [Header("核心跟随目标")]
    [Tooltip("相机跟随的目标（如英雄Transform）")]
    public Transform target;

    [Header("基础移动设置")]
    [Tooltip("相机平移速度（屏幕边缘触发时）")]
    public float moveSpeed = 20f;
    [Tooltip("跟随目标的平滑过渡时间（越小越灵敏）")]
    public float smoothTime = 0.15f;
    [Tooltip("鼠标触发边缘平移的像素距离（屏幕边缘向内）")]
    public float edgeSize = 20f;

    [Header("角度控制设置")]
    [Tooltip("相机初始俯仰角度（绕X轴，越大视角越俯视）")]
    public float initialVerticalAngle = 60f;
    [Tooltip("鼠标右键旋转视角的灵敏度")]
    public float horizontalRotateSpeed = 2f;
    [Tooltip("俯仰角度最小值（防止视角过低）")]
    public float minVerticalAngle = 30f;
    [Tooltip("俯仰角度最大值（防止视角过高）")]
    public float maxVerticalAngle = 80f;
    [Tooltip("水平偏移角度：0=默认左侧，180=正右侧，90=后侧，-90=前侧")]
    public float horizontalOffset = 180f;
    [Tooltip("滚轮调节相机距离的速度")]
    public float scrollSpeed = 5f;

    [Header("距离/高度限制")]
    [Tooltip("相机与目标的基础距离")]
    public float baseDistance = 20f;
    [Tooltip("相机最低高度（防止穿地）")]
    public float minHeight = 5f;
    [Tooltip("相机最高高度（防止视角过高）")]
    public float maxHeight = 30f;

    // 平滑移动的速度缓存（SmoothDamp用）
    private Vector3 _currentVelocity = Vector3.zero;
    // 是否自动跟随目标
    private bool _isFollowing = true;
    // 当前俯仰角度（绕X轴）
    private float _currentVerticalAngle;
    // 当前水平旋转角度（绕Y轴）
    private float _currentHorizontalAngle;
    // 当前相机与目标的实际距离
    private float _currentDistance;

    private void Start()
    {
        // 初始化角度和距离（防止空目标报错）
        if (target != null)
        {
            // 初始水平角度 = 目标朝向 + 水平偏移（180度=从左侧切换到右侧）
            _currentHorizontalAngle = target.eulerAngles.y + horizontalOffset;
            _currentVerticalAngle = initialVerticalAngle;
            _currentDistance = baseDistance;

            // 初始化相机位置
            UpdateCameraPosition();
        }
        else
        {
            Debug.LogError("MobaCamera：未设置跟随目标（target），请在Inspector面板赋值！");
        }
    }

    private void LateUpdate()
    {
        // 目标为空时直接返回，避免空引用错误
        if (target == null) return;

        // 1. 空格键重置跟随状态（回到目标右侧视角）
        HandleResetFollow();

        // 2. 处理屏幕边缘平移
        HandleCameraPanning();

        // 3. 处理鼠标右键旋转视角
        //HandleCameraRotation();

        // 4. 处理滚轮调节相机距离/高度
        HandleScrollWheel();

        // 5. 自动跟随目标逻辑
        if (_isFollowing)
        {
            FollowTarget();
        }

        // 6. 强制更新相机位置和朝向（确保角度/距离生效）
        UpdateCameraPosition();
    }

    /// <summary>
    /// 空格键重置为跟随目标状态，还原右侧视角
    /// </summary>
    private void HandleResetFollow()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isFollowing = true;
            // 重置水平角度为「目标朝向 + 右侧偏移」，确保回到右侧视角
            _currentHorizontalAngle = target.eulerAngles.y + horizontalOffset;
        }
    }

    /// <summary>
    /// 平滑跟随目标
    /// </summary>
    private void FollowTarget()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        // 平滑移动相机到目标位置
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }

    /// <summary>
    /// 处理屏幕边缘平移相机
    /// </summary>
    private void HandleCameraPanning()
    {
        Vector3 moveDir = Vector3.zero;

        // 检测鼠标是否在屏幕边缘，计算平移方向
        if (Input.mousePosition.x >= Screen.width - edgeSize)
            moveDir.x += 1;
        if (Input.mousePosition.x <= edgeSize)
            moveDir.x -= 1;
        if (Input.mousePosition.y >= Screen.height - edgeSize)
            moveDir.z += 1;
        if (Input.mousePosition.y <= edgeSize)
            moveDir.z -= 1;

        // 有平移输入时，停止自动跟随，执行平移
        if (moveDir != Vector3.zero)
        {
            _isFollowing = false;
            // 转换为世界空间的平移方向（基于相机朝向，锁定Y轴防止上下移）
            Vector3 move = transform.TransformDirection(moveDir.normalized) * moveSpeed * Time.deltaTime;
            move.y = 0; // 强制锁定Y轴，只在水平面上平移
            transform.Translate(move, Space.World);
        }
    }

    /// <summary>
    /// 处理鼠标右键旋转视角
    /// </summary>
    private void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1)) // 按住鼠标右键
        {
            _isFollowing = false;

            // 水平旋转（绕Y轴）：若旋转方向反了，把“-”改成“+”即可
            _currentHorizontalAngle -= Input.GetAxis("Mouse X") * horizontalRotateSpeed;
            // 垂直旋转（绕X轴）：限制在最小/最大角度之间
            _currentVerticalAngle -= Input.GetAxis("Mouse Y") * horizontalRotateSpeed;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }
    }

    /// <summary>
    /// 处理滚轮调节相机与目标的距离
    /// </summary>
    private void HandleScrollWheel()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            _isFollowing = false;
            // 调整距离并限制在最小/最大高度范围内
            _currentDistance = Mathf.Clamp(_currentDistance - scrollInput * scrollSpeed, minHeight, maxHeight);
        }
    }

    /// <summary>
    /// 计算相机的目标位置（基于角度和距离）
    /// </summary>
    /// <returns>相机应该到达的世界位置</returns>
    private Vector3 CalculateTargetPosition()
    {
        // 基于当前角度创建旋转
        Quaternion rotation = Quaternion.Euler(_currentVerticalAngle, _currentHorizontalAngle, 0);
        // 计算相机相对于目标的偏移方向（Vector3.back = 相机看向目标的方向）
        Vector3 direction = rotation * Vector3.back * _currentDistance;
        // 最终位置 = 目标位置 + 偏移方向
        return target.position + direction;
    }

    /// <summary>
    /// 更新相机位置和朝向（核心方法）
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        // 锁定相机最低高度，防止穿地
        targetPosition.y = Mathf.Max(targetPosition.y, minHeight);
        // 更新相机位置
        transform.position = targetPosition;
        // 相机始终看向目标的略上方（+1.5f避免视角贴地面）
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}