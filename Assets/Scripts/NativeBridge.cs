using UnityEngine;
using System.Runtime.InteropServices;

public class NativeBridge : Singleton<NativeBridge>
{
    private const string UNITY_OBJECT_NAME = "NativeBridge";

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendRotationToNative(float x, float y, float z);

    [DllImport("__Internal")]
    private static extern void TriggerFireSparkParticle();

    [DllImport("__Internal")]
    private static extern void StopFireSparkParticle();

    [DllImport("__Internal")]
    private static extern void OpenNativePage();
#endif

    private void Awake()
    {
        CleanupDuplicateInstances();
        gameObject.name = UNITY_OBJECT_NAME; 
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (GetSparkParticles() == null)
        {
            Debug.LogError("❌ Fire Sparks Particle System is not assigned!");
        }
        else
        {
            Debug.Log("✅ Fire Sparks Particle System is assigned!");
        }
    }

    /// <summary>
    /// Removes duplicate NativeBridge instances from the scene.
    /// </summary>
    private void CleanupDuplicateInstances()
    {
        var existingBridges = FindObjectsByType<NativeBridge>(FindObjectsSortMode.InstanceID);
        if (existingBridges.Length > 1)
        {
            Debug.LogWarning("⚠️ Duplicate NativeBridge found. Destroying new instance.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sends rotation data to the native iOS side.
    /// </summary>
    public void SendRotationData(Vector3 rotation)
    {
#if UNITY_IOS && !UNITY_EDITOR
        SendRotationToNative(rotation.x, rotation.y, rotation.z);
#else
        Debug.Log($"📡 [NativeBridge] Rotation sent to native (Simulated): {rotation.x}, {rotation.y}, {rotation.z}");
#endif
    }

    /// <summary>
    /// Triggers the fire spark effect in the native iOS side.
    /// </summary>
    public void TriggerFireSparkEmitter()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log("🔥 [NativeBridge] Triggering fire spark particle effect via native.");
        TriggerFireSparkParticle();
#else
        Debug.Log("🔥 [NativeBridge] Simulating fire spark trigger in Unity (Editor mode).");
#endif
    }

    /// <summary>
    /// Stops the fire spark effect in the native iOS side.
    /// </summary>
    public void StopFireSparkEmitter()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log("🛑 [NativeBridge] Stopping fire spark effect via native.");
        StopFireSparkParticle();
#else
        Debug.Log("🛑 [NativeBridge] Simulating stopping fire spark in Unity (Editor mode).");
#endif
    }

    /// <summary>
    /// Handles particle effect color changes from the native side.
    /// </summary>
    public void TriggerParticleEffect(string colorData)
    {
        if (!TryParseColorData(colorData, out Color newColor))
        {
            Debug.LogError($"❌ [NativeBridge] Invalid color data received: {colorData}");
            return;
        }

        ApplyParticleEffect(newColor);
    }

    /// <summary>
    /// Attempts to parse color data from a string.
    /// </summary>
    private bool TryParseColorData(string colorData, out Color color)
    {
        color = Color.white;
        string[] colorValues = colorData.Split(',');

        if (colorValues.Length != 3)
        {
            return false;
        }

        if (float.TryParse(colorValues[0], out float r) &&
            float.TryParse(colorValues[1], out float g) &&
            float.TryParse(colorValues[2], out float b))
        {
            color = new Color(r, g, b);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Applies the parsed color to the particle system.
    /// </summary>
    private void ApplyParticleEffect(Color newColor)
    {
        var sparkParticles = GetSparkParticles();
        if (sparkParticles == null)
        {
            Debug.LogError("❌ [NativeBridge] Fire Sparks Particle System is missing!");
            return;
        }

        Debug.Log($"🔥 [NativeBridge] Spawning fire sparks with color: {newColor}");

        var main = sparkParticles.main;
        main.startColor = newColor;

        sparkParticles.Play();
    }

    /// <summary>
    /// Calls the native function to open the iOS native page.
    /// </summary>
    public void OpenNativePageFromUnity()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log("📲 [NativeBridge] Requesting to open native page.");
        OpenNativePage();
#else
        Debug.Log("📲 [NativeBridge] Simulating opening native page (Editor mode).");
#endif
    }

    /// <summary>
    /// Retrieves the Spark Particle System from the GameManager.
    /// </summary>
    private ParticleSystem GetSparkParticles()
    {
        return GameManager.Instance?.SparkParticles;
    }
}
