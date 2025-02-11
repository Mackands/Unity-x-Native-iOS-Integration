using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages game state, UI transitions, and native bridge interactions.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField, Tooltip("The 3D Object Panel component reference.")]
    private GameObject object3D;

    [Tooltip("The particle system used for spark effects.")]
    public ParticleSystem SparkParticles;

    [SerializeField, Tooltip("The Home Panel component.")]
    private GameObject homePanel;

    [SerializeField, Tooltip("The Setting Panel component.")]
    private GameObject settingPanel;

    [SerializeField, Tooltip("The Title Panel component.")]
    private TextMeshProUGUI titleText;

    [Tooltip("The setting button interactable used to block input when animation is in progress.")]
    public Button buttonSetting;

    private void Awake()
    {
        CleanupDuplicateInstances();
    }

    private void Start()
    {
        if (SparkParticles != null)
        {
            SparkParticles.Stop();
        }
        else
        {
            Debug.LogWarning("⚠️ SparkParticles is not assigned in the GameManager!");
        }
    }

    /// <summary>
    /// Opens the Settings panel and stops spark effects.
    /// </summary>
    public void OpenSetting()
    {
        SetTitleText("Setting");
        TogglePanels(false);

        if (SparkParticles != null)
        {
            SparkParticles.Stop();
        }

        Debug.Log("🔴 Stopping fire spark effect...");
        if (NativeBridge.Instance != null)
        {
            NativeBridge.Instance.StopFireSparkEmitter();
        }
        else
        {
            Debug.LogWarning("⚠️ NativeBridge.Instance is null! Cannot stop fire spark.");
        }
    }

    /// <summary>
    /// Opens the native page via the Unity-iOS bridge.
    /// </summary>
    public void OpenNativePage()
    {
        if (NativeBridge.Instance != null)
        {
            Debug.Log("📲 Requesting to open the native page...");
            NativeBridge.Instance.OpenNativePageFromUnity();
        }
        else
        {
            Debug.LogError("❌ NativeBridge.Instance is null! Cannot open the native page.");
        }
    }

    /// <summary>
    /// Returns to the Home panel and re-enables the 3D object.
    /// </summary>
    public void BackHome()
    {
        SetTitleText("Home");
        buttonSetting.interactable = false;
        TogglePanels(true);
    }

    /// <summary>
    /// Updates the title text on the UI.
    /// </summary>
    private void SetTitleText(string text)
    {
        if (titleText != null)
        {
            titleText.text = text;
        }
        else
        {
            Debug.LogWarning("⚠️ titleText is not assigned in the GameManager!");
        }
    }

    /// <summary>
    /// Toggles between the Home and Setting panels.
    /// </summary>
    private void TogglePanels(bool showHome)
    {
        if (homePanel != null) homePanel.SetActive(showHome);
        if (settingPanel != null) settingPanel.SetActive(!showHome);
        if (object3D != null) object3D.SetActive(showHome);
    }

    /// <summary>
    /// Ensures only one instance of GameManager exists in the scene.
    /// </summary>
    private void CleanupDuplicateInstances()
    {
        var existingManagers = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID);
        if (existingManagers.Length > 1)
        {
            Debug.LogWarning("⚠️ Duplicate GameManager found. Destroying new instance.");
            Destroy(gameObject);
        }
    }
}
