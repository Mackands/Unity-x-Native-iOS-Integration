using TMPro;
using UnityEngine;
using System.Collections;

public class MarqueeText : MonoBehaviour
{
    [SerializeField] private float marqueeSpeed = 100f; // Speed of the marquee text
    private TextMeshProUGUI marqueeText;
    private RectTransform marqueeRectTransform;
    private float textWidth;
    private float screenWidth;
    private WaitForSeconds updateInterval = new WaitForSeconds(1f); // Update date/time every second

    void Start()
    {
        // Get the TextMeshProUGUI component from the child of the Panel
        marqueeText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (marqueeText == null)
        {
            Debug.LogError("❌ No TextMeshProUGUI component found as a child of the Panel.");
            return;
        }

        marqueeRectTransform = marqueeText.GetComponent<RectTransform>();
        textWidth = marqueeText.preferredWidth;
        screenWidth = GetScreenWidth();

        ResetMarqueePosition();

        // Start updating date/time at an interval instead of every frame
        StartCoroutine(UpdateDateTimeRoutine());
    }

    void Update()
    {
        MoveMarqueeText();
    }

    /// <summary>
    /// Moves the marquee text smoothly across the screen.
    /// </summary>
    private void MoveMarqueeText()
    {
        if (marqueeText == null) return;

        // Move text left
        marqueeRectTransform.Translate(Vector3.left * marqueeSpeed * Time.deltaTime);

        // Check if text has moved completely off the left side
        if (marqueeRectTransform.anchoredPosition.x + textWidth < -screenWidth / 2)
        {
            ResetMarqueePosition();
        }
    }

    /// <summary>
    /// Updates the date and time display once per second.
    /// </summary>
    private IEnumerator UpdateDateTimeRoutine()
    {
        while (true)
        {
            if (marqueeText != null)
            {
                marqueeText.text = GetFormattedDateTime();
            }
            yield return updateInterval;
        }
    }

    /// <summary>
    /// Resets the marquee text position to the right edge.
    /// </summary>
    private void ResetMarqueePosition()
    {
        marqueeRectTransform.anchoredPosition = new Vector3(screenWidth / 2 + textWidth / 2, marqueeRectTransform.anchoredPosition.y, 0);
    }

    /// <summary>
    /// Returns the formatted current date and time.
    /// </summary>
    private string GetFormattedDateTime()
    {
        return $"Current Date: {System.DateTime.Now:dddd, d MMMM yyyy}. Current Time: {System.DateTime.Now:HH:mm}";
    }

    /// <summary>
    /// Returns the correct screen width for UI elements.
    /// </summary>
    private float GetScreenWidth()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
        {
            return Screen.width; // Use screen width for world-space UI
        }
        return canvas.GetComponent<RectTransform>().rect.width;
    }
}
