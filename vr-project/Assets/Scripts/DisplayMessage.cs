using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessage : MonoBehaviour
{
    ExtractQRCode extractQRCode;
    GameObject messageObject;
    Text message;

    // Start is called before the first frame update
    void Start()
    {
        messageObject = gameObject.transform.GetChild(0).gameObject;
        message = messageObject.GetComponentInChildren<Text>();
        messageObject.SetActive(false);
        extractQRCode = GameObject.Find("QRCodeExtractionSystem").GetComponent<ExtractQRCode>();
    }

    // Update is called once per frame
    void Update()
    {
        // display error message when extractQRCode script raises an error flag
        if (extractQRCode.IsDisplayErrMsg)
        {
            extractQRCode.IsDisplayErrMsg = false;

            message.text = "QR Code Not Detected!";

            StartCoroutine(Display());
        }

    }

    // show error message for 1.5 seconds
    IEnumerator Display()
    {
        messageObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        messageObject.SetActive(false);
    }

}
