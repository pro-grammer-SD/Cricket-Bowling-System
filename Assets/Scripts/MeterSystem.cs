using UnityEngine;
using UnityEngine.UI;

public class MeterSystem : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float speed = 2.5f;

    private bool isRunning = true;
    private float currentValue;

    // Constants
    private const float CENTER_POINT = 0.5f;
    private const float BLUE_ZONE = 0.05f;
    private const float GREEN_ZONE = 0.15f;
    private const float YELLOW_ZONE = 0.30f;

    void Update()
    {
        if (isRunning)
        {
            // PingPong returns 0 to 1 based on length
            currentValue = Mathf.PingPong(Time.time * speed, 1f);
            if (slider) slider.value = currentValue;
        }
    }

    public void SetActive(bool state) => isRunning = state;

    public float GetAccuracy()
    {
        isRunning = false;

        float dist = Mathf.Abs(CENTER_POINT - currentValue);

        // Optimized logical check
        if (dist < BLUE_ZONE) return 1.0f; // Blue
        if (dist < GREEN_ZONE) return 0.7f; // Green
        if (dist < YELLOW_ZONE) return 0.4f; // Yellow

        return 0.0f; // Red
    }
}