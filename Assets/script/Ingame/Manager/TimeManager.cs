using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static bool isPaused = false;

    public static void PauseTime()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public static void ResumeTime()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public static bool IsPaused()
    {
        return isPaused;
    }
}