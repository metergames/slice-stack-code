using UnityEngine;

public static class VibrationManager
{
    private static AndroidJavaObject _vibrator;
    private static int _sdkInt;

    static VibrationManager()
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                _sdkInt = version.GetStatic<int>("SDK_INT");
            }

            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
        }
    }

    /// <summary>
    /// Vibrate with specified duration (ms) and amplitude (1–255). On Android API<26 amplitude is ignored.
    /// </summary>
    public static void Vibrate(long duration = 500, int amplitude = 255)
    {
        if (Application.platform == RuntimePlatform.Android && _vibrator != null)
        {
            try
            {
                if (_sdkInt >= 26)
                {
                    using (var vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                    {
                        var effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                            "createOneShot", duration, Mathf.Clamp(amplitude, 1, 255)
                        );
                        _vibrator.Call("vibrate", effect);
                    }
                }
                else
                {
                    _vibrator.Call("vibrate", duration);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Vibration failed: " + e.Message);
                Handheld.Vibrate();
            }
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    /// <summary>
    /// Cancel any ongoing vibration.
    /// </summary>
    public static void Cancel()
    {
        if (Application.platform == RuntimePlatform.Android && _vibrator != null)
        {
            _vibrator.Call("cancel");
        }
    }
}
