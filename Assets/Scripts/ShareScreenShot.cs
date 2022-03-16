using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShareScreenShot : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    // private ARPlaneManager arPlaneManager;
    private ARPointCloudManager _arPointCloudManager;

    private void Start()
    {
        // arPlaneManager = FindObjectOfType<ARPlaneManager>();
        _arPointCloudManager = FindObjectOfType<ARPointCloudManager>();
    }

    public void TakeScreenShot()
    {
        TurnOnOffARContent();
        StartCoroutine(TakeScreenshotAndShare());
    }

    private void TurnOnOffARContent()
    {
        // var planes = arPlaneManager.trackables;
        // mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        // foreach (var plane in planes)
        // {
        //     plane.gameObject.SetActive(!plane.gameObject.activeSelf);
        // }
        // mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        
        var points = _arPointCloudManager.trackables;
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        foreach (var point in points)
        {
            point.gameObject.SetActive(!point.gameObject.activeSelf);
        }
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        
        
    }
    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
        ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
        ss.Apply();

        string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
        File.WriteAllBytes( filePath, ss.EncodeToPNG() );

        // To avoid memory leaks
        Destroy( ss );

        new NativeShare().AddFile( filePath )
            .SetSubject( "Subject goes here" ).SetText( "Hey! Check out this app!!!" )
            .SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
            .Share();
        TurnOnOffARContent();
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
