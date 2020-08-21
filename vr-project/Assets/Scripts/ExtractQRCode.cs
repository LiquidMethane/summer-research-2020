using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZXing;
using System.Drawing;
using ZXing.Multi;
using System.Text;

public class ExtractQRCode : MonoBehaviour
{
    Bitmap bmp;
    BarcodeReader br;
    bool isSnapshotTaken = false;

    public bool IsDisplayErrMsg { get; set; } = false;
    public bool IsQueryReady { get; set; } = false;
    public string Result { get; set; } = null;


    // Start is called before the first frame update
    void Start()
    {
        // initialize variables
        br = new BarcodeReader();
    }

    // Update is called once per frame
    void Update()
    {
        // take screenshot when return is pressed
        // using return for testing purpose, will be re-mapped to oculus controller in the future
        //if (Input.GetKeyDown(KeyCode.Return))
        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Three))
        {
            StartCoroutine(DoScreenshot());
        }

        // analyse snapshot after it is taken
        if (isSnapshotTaken)
        {
            isSnapshotTaken = false;

            //Result r = br.Decode(bmp);

            Result = null;
            StringBuilder test_sb = new StringBuilder();

            // one known issue with this decoder is that if the screenshot has an partial QR code 
            // in it the decoder may not be able to recognize any QR codes in the screenshot,
            // probably because the partial QR code messes up the corner recognition algorithm
            Result[] res_list = br.DecodeMultiple(bmp);

            // For now, the result is set to the last QR code the decoder returns.
            // In the future, there may be a UI that presents all recognized QR codes
            // and lets user choose which one he/she wants to see in detail.
            if (res_list != null && res_list.Length > 0)
            {
                test_sb.AppendLine(string.Format("\r\n{0}", res_list.Length));

                foreach (Result r in res_list)
                {
                    test_sb.AppendLine(r?.ToString());
                    Result = r?.ToString();
                }
            }

            Debug.Log(test_sb.ToString());

            bmp.Dispose();
            IsQueryReady = Result != null;
            IsDisplayErrMsg = !IsQueryReady;

        }
    }

    /// <summary>
    /// the function waits until the end of current frame (when frame is fully rendered), and captures a 
    /// snapshot of the frame into byte array. Byte array is then converted to Bitmap for barcode reader 
    /// to analyze. 
    /// </summary>
    /// <returns></returns>
    IEnumerator DoScreenshot()
    {
        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // convert byte array to a representation of PNG file
        byte[] bytes = texture.EncodeToPNG();

        // leave other processing to next frame
        yield return null;

        bmp = new Bitmap(new MemoryStream(bytes));

        isSnapshotTaken = true;

        // destroy the texture2d object
        Destroy(texture);
    }
}
