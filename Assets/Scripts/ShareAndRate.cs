using UnityEngine;
using System.Collections;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;


public class ShareAndRate : MonoBehaviour
{
    [SerializeField] TMP_InputField textMesh;

    string subject = "Hi";
    string body = " ";

#if UNITY_IPHONE
 
 [DllImport("__Internal")]
 private static extern void sampleMethod (string iosPath, string message);
 
 [DllImport("__Internal")]
 private static extern void sampleTextMethod (string message);
 
#endif

    public void OnAndroidTextSharingClick()
    {
        body = textMesh.text;
        StartCoroutine(ShareAndroidText());

    }
    IEnumerator ShareAndroidText()
    {
        yield return new WaitForEndOfFrame();
        // выполнить строки ниже, если они запущены на устройстве Android
#if UNITY_ANDROID

        //Ссылка на класс AndroidJavaClass для Intent
        //Intent это абстрактное описание выполняемой операции.
        //Его можно использовать для запуска Activity и т.д
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");

        //Ссылка на класс AndroidJavaObject для intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        //вызов метода setAction созданного объекта Intent  call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        //Установите тип общего доступа, который происходит set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        //добавьте данные, которые будут переданы другому виду деятельности, т. е./add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);

        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Text Sharing ");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);

        //получить текущую активность/ get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //начните действие, отправив данные о намерениях/start the activity by sending the intent data
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);

#endif
    }


    public void OniOSTextSharingClick()
    {

#if UNITY_IPHONE || UNITY_IPAD
  string shareMessage = "Wow I Just Share Text ";
  sampleTextMethod (shareMessage);
  
#endif
    }

    public void RateUs()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=YOUR_ID");
#elif UNITY_IPHONE
  Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
    }

}
