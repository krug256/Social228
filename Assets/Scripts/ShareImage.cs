using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShareImage : MonoBehaviour
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

        screenshotName = "Share";
        shareSubject = " ";
        shareMessage = " ";

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
        Texture2D screenTexture = new Texture2D(512, 512, TextureFormat.RGB24, true); 
        screenTexture.Apply();
        byte[] dataToSave = Resources.Load<TextAsset>(screenshotName).bytes;
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        Debug.Log(destination);
        File.WriteAllBytes(destination, dataToSave);

        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);

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