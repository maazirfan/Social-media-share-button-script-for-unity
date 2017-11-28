using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ShareScreenScript : MonoBehaviour {

    public static ShareScreenScript instance;

    //for android
    private bool isProcessing = false;
    public string message;


    void Awake()
    {
        MakeInstance();
    }

    //method whihc make this object instance
    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //function called from a button
    public void ButtonShare()
    {
        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshot());
        }
    }
    public IEnumerator ShareScreenshot()
    {
        isProcessing = true;
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
        // create the texture
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        // put buffer into texture
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        // apply
        screenTexture.Apply();
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
        byte[] dataToSave = screenTexture.EncodeToPNG();
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, dataToSave);
        if (!Application.isEditor)
        {
            // block to open the file and share it ------------START
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "" + message);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");

            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
        }
        isProcessing = false;

    }// for android



    //    //for iOS
    //    public static event Action ScreenshotFinishedSaving;
    //    public static event Action ImageFinishedSaving;

    //    public static string savedImagePath = string.Empty;

    //#if UNITY_IPHONE

    //	[DllImport("__Internal")]
    //	private static extern bool saveToGallery (string path);

    //#endif

    //    public static IEnumerator Save(string fileName, string albumName = "MyScreenshots", bool callback = false)
    //    {
    //#if UNITY_IPHONE

    //		bool photoSaved = false;

    //		string date = System.DateTime.Now.ToString ("dd-MM-yy");

    //		ScreenshotHandler.ScreenShotNumber++;

    //		string screenshotFilename = fileName + "_" + ScreenshotHandler.ScreenShotNumber + "_" + date + ".png";

    //		Debug.Log ("Save screenshot " + screenshotFilename);


    //		if (Application.platform == RuntimePlatform.IPhonePlayer) {
    //			Debug.Log ("iOS platform detected");

    //			string iosPath = Application.persistentDataPath + "/" + fileName;
    //			savedImagePath = iosPath;
    //			Application.CaptureScreenshot (screenshotFilename);

    //			while (!photoSaved) {
    //				photoSaved = saveToGallery (iosPath);

    //				yield return new WaitForSeconds (.5f);
    //			}				

    //			iPhone.SetNoBackupFlag (iosPath);

    //		} else {

    //			Application.CaptureScreenshot (screenshotFilename);

    //		}


    //#endif
    //        yield return 0;
    //        if (callback)
    //            ScreenshotFinishedSaving();
    //    }


    //    public static IEnumerator SaveExisting(string filePath, bool callback = false)
    //    {
    //        yield return 0;

    //        bool photoSaved = false;

    //        Debug.Log("Save existing file to gallery " + filePath);

    //#if UNITY_IPHONE

    //		if (Application.platform == RuntimePlatform.IPhonePlayer) {
    //			Debug.Log ("iOS platform detected");

    //			while (!photoSaved) {
    //				photoSaved = saveToGallery (filePath);

    //				yield return new WaitForSeconds (.5f);
    //			}

    //			iPhone.SetNoBackupFlag (filePath);
    //		}

    //#endif

    //        if (callback)
    //            ImageFinishedSaving();
    //    }


    //    public static int ScreenShotNumber
    //    {
    //        set { PlayerPrefs.SetInt("screenShotNumber", value); }

    //        get { return PlayerPrefs.GetInt("screenShotNumber"); }
    //    }

    //    //for iOS
}
