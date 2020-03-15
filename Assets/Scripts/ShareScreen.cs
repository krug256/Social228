using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShareScreen : MonoBehaviour
{
#if UNITY_ANDROID
    private bool isFocus = false;

    private string shareSubject, shareMessage;
    private bool isProcessing = false;
    private string screenshotName;

    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

    public void OnShareButtonClick()
    {

        screenshotName = "Screen.png";
        shareSubject = " AAAAAAAAAAAA";
        shareMessage = " BBBBBBBBBBBB";

        ShareScreenshot();
    }

    private void ShareScreenshot()
    {
        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshotInAnroid());
        }
    }


    public IEnumerator ShareScreenshotInAnroid()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.25f);

        string screenShotPath = Application.persistentDataPath + "/" + screenshotName;
        ScreenCapture.CaptureScreenshot(screenshotName, 1);
        yield return new WaitForSecondsRealtime(0.25f);

        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }

        yield return new WaitUntil(() => isFocus);
        isProcessing = false;


    }
#endif
}