using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectController : MonoBehaviour
{
    [SerializeField, Tooltip("The animator controller used for animation.")]
    private Animator animator;

    private bool _isRotating = false;
    private bool _isAnimating = false;
    private float sensitivity = 10f; // Rotation sensitivity

    public bool IsAnimating => _isAnimating; // Read-only property

    private void Awake()
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator is not assigned in ObjectController.");
        }
    }

    private void OnEnable() 
    {
        StartAnimation();
    }

    void Start()
    {
        EnsureColliderExists();
    }

    void Update()
    {
        HandleTouchInput();
    }

    /// <summary>
    /// Ensures the GameObject has a Collider for interactions.
    /// </summary>
    private void EnsureColliderExists()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    /// <summary>
    /// Handles touch input for rotating the object.
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            float x = touch.deltaPosition.x * Time.deltaTime * sensitivity;
            float y = touch.deltaPosition.y * Time.deltaTime * sensitivity;
            transform.Rotate(x, y, 0);

            if (!_isRotating)
            {
                _isRotating = true;
                StartCoroutine(SendRotationData(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
            }
        }
    }

    /// <summary>
    /// Sends rotation data to the native side and throttles the updates.
    /// </summary>
    private IEnumerator SendRotationData(float x, float y, float z)
    {
        Debug.Log($"🔄 Sending Rotation Data: x={x}, y={y}, z={z}");

        if (NativeBridge.Instance != null)
        {
            Vector3 rotation = new Vector3(x, y, z);
            NativeBridge.Instance.SendRotationData(rotation);
        }
        else
        {
            Debug.LogWarning("⚠️ NativeBridge.Instance is null. Retrying...");
            yield return new WaitForSeconds(0.1f); // Wait before retrying
            if (NativeBridge.Instance != null)
            {
                Vector3 rotation = new Vector3(x, y, z);
                NativeBridge.Instance.SendRotationData(rotation);
            }
        }

        yield return new WaitForSeconds(0.1f); // Ensures throttled updates
        _isRotating = false;
    }

    /// <summary>
    /// Starts the object's animation and disables settings button.
    /// </summary>
    public void StartAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator is missing! Cannot start animation.");
            return;
        }

        animator.enabled = true;
        _isAnimating = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.buttonSetting.interactable = false;
        }
    }

    /// <summary>
    /// Stops the object's animation and enables settings button.
    /// </summary>
    public void StopAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator is missing! Cannot stop animation.");
            return;
        }

        animator.enabled = false;
        _isAnimating = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.buttonSetting.interactable = true;
        }

        Debug.Log("🏁 StopAnimation called - Triggering emitter effect");

        // ✅ Call the native bridge to trigger fire spark particles
        if (NativeBridge.Instance != null)
        {
            NativeBridge.Instance.TriggerFireSparkEmitter();
        }
        else
        {
            Debug.LogError("❌ NativeBridge.Instance is null. Cannot trigger fire spark.");
        }
    }
}
