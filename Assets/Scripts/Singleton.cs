using UnityEngine;

/// <summary>
/// Generic Singleton class for MonoBehaviour-based singletons in Unity.
/// Ensures thread safety, prevents duplicate instances, and supports DontDestroyOnLoad.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isQuitting = false; // Prevents access after quitting

    /// <summary>
    /// Global access to the singleton instance.
    /// Automatically creates an instance if none exists.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} requested after application quit. Returning null.");
                return null;
            }

            if (_instance == null)
            {
                T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None); // Run outside of lock to avoid thread issues

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (instances.Length > 0)
                        {
                            _instance = instances[0];

                            if (instances.Length > 1)
                            {
                                Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} found. Using the first one.");
                            }
                        }
                        else
                        {
                            GameObject singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                            _instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Ensures only one instance exists in the scene.
    /// Destroys duplicate instances upon creation.
    /// </summary>
    private void Awake()
    {
        CheckDuplicateInstance();
    }

    /// <summary>
    /// Prevents memory leaks by resetting the singleton when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    /// <summary>
    /// Handles cleanup when the application quits.
    /// </summary>
    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    /// <summary>
    /// Checks for duplicate singleton instances and destroys the extra ones.
    /// </summary>
    private void CheckDuplicateInstance()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} found. Destroying new instance.");
            Destroy(gameObject);
        }
    }
}
