using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

public class Bilboard : MonoBehaviour
{
    QueryDB queryDB;
    Facility facility;
    MaintenanceRecord[] maintenanceRecords;

    Transform msgMngr;

    GameObject billboardManager;
    GameObject billboardObject;
    GameObject texts;
    GameObject labels;

    GameObject id_obj;
    GameObject name_obj;
    GameObject type_obj;
    GameObject dop_obj;
    GameObject status_obj;
    GameObject maint_obj;
    GameObject livefeed_obj;

    string str_maint;
    string str_live;
    readonly int fontSize = 25;

    bool showVars = false;

    // Start is called before the first frame update
    void Start()
    {
        // reference to QueryDB
        queryDB = GameObject.Find("DBInteractionSystem").GetComponent<QueryDB>();
        // reference to MessageManager
        msgMngr = GameObject.Find("MessageManager").transform;

        // setup BillboardManager
        billboardManager = new GameObject();
        billboardManager.name = "BillboardManager";

        RectTransform rt = billboardManager.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100f, 100f);
        rt.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        rt.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
        rt.localEulerAngles = Vector3.zero;

        Canvas canvas = billboardManager.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.pixelPerfect = false;

        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        // populate billboard if query successful and display billboard
        if (queryDB.IsFacilityInfoReceived && queryDB.IsMaintRecReceived)
        {
            queryDB.IsFacilityInfoReceived = false;
            queryDB.IsMaintRecReceived = false;

            facility = queryDB.GetFacility();
            maintenanceRecords = queryDB.GetMaintenanceRecords();

            showVars = true;

            RectTransform r = billboardManager.GetComponent<RectTransform>();
            r.eulerAngles = msgMngr.eulerAngles;
            r.position = msgMngr.position;

            PopulateStrings();
        }

        //close billboard when user presses back button
        //if (Input.GetKeyDown(KeyCode.Escape))
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Four))
        {
            showVars = false;
        }

        if (showVars)
        {
            
            billboardManager.SetActive(true);
            UpdateStrings();
        }
        else
        {
            queryDB.StopQueryLiveData();
            billboardManager.SetActive(false);

        }
    }

    void InitializeUI()
    {

        billboardObject = new GameObject("Billboard");
        billboardObject.transform.parent = GameObject.Find("BillboardManager").transform;
        billboardObject.transform.localPosition = new Vector3(0f, 100f, 0f);
        billboardObject.transform.localEulerAngles = Vector3.zero;
        billboardObject.transform.localScale = Vector3.one;


        name_obj = VariableObjectManager(name_obj, "Name_obj", "Name:", -202.5f, -110f, 400f, 50f, null, fontSize, TextAnchor.LowerCenter);
        status_obj = VariableObjectManager(status_obj, "Status_obj", "Status:", 202.5f, -110f, 400f, 50f, null, fontSize, TextAnchor.LowerCenter);
        id_obj = VariableObjectManager(id_obj, "Id_obj", "ID:", -202.5f, -165f, 400f, 50f, null, fontSize, TextAnchor.LowerCenter);
        type_obj = VariableObjectManager(type_obj, "Type_obj", "Type:", 202.5f, -165f, 400f, 50f, null, fontSize, TextAnchor.LowerCenter);
        dop_obj = VariableObjectManager(dop_obj, "Dop_obj", "Date of Purchase:", -202.5f, -220f, 400f, 50f, null, fontSize, TextAnchor.LowerCenter);
        maint_obj = VariableObjectManager(maint_obj, "Maint_obj", "Maintenance Records:", 202.5f, -357.5f, 400f, 325f, str_maint, fontSize, TextAnchor.MiddleCenter);
        livefeed_obj = VariableObjectManager(livefeed_obj, "Livefeed_obj", "Live feed:", -202.5f, -385f, 400f, 270f, str_live, fontSize, TextAnchor.MiddleCenter);

    }

    GameObject ComponentComposition(GameObject GO, string label, float sizeX, float sizeY, TextAnchor alignment)
    {
        // initialize gameobject and add canvas
        GO = new GameObject();
        GO.AddComponent<RectTransform>();
        GO.AddComponent<CanvasRenderer>();
        GO.AddComponent<Image>();
        GO.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
        GO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);

        // add text to canvas
        texts = new GameObject();
        texts.AddComponent<RectTransform>();
        texts.AddComponent<CanvasRenderer>();
        texts.AddComponent<Text>();
        texts.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
        texts.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        texts.GetComponent<Text>().alignment = alignment;

        texts.transform.SetParent(GO.transform);
        texts.name = "TextBox";

        // add label to canvas
        labels = new GameObject();
        labels.AddComponent<RectTransform>();
        labels.AddComponent<CanvasRenderer>();
        labels.AddComponent<Text>();
        labels.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
        labels.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        labels.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        labels.GetComponent<Text>().text = label;

        labels.transform.SetParent(GO.transform);
        labels.name = "LabelBox";

        return GO;
    }

    GameObject VariableObjectManager(GameObject gameObject, string name, string label, float posX, float posY, float sizeX, float sizeY, string str, int fontSize, TextAnchor alignment)
    {
        // initialize gameobjct and set it to be a child of billboardObject
        gameObject = ComponentComposition(gameObject, label, sizeX, sizeY, alignment);
        gameObject.name = name;
        gameObject.transform.SetParent(billboardObject.transform);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(posX, posY, 0.0f);

        // assign text to textbox
        Text text = GetChildByName(gameObject, "TextBox").GetComponent<Text>();
        text.text = str;
        text.fontSize = fontSize;
        gameObject.transform.localEulerAngles = Vector3.zero;

        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        return gameObject;
    }

    GameObject GetChildByName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    void PopulateStrings()
    {
        if (!billboardObject)
            return;

        GetChildByName(id_obj, "TextBox").GetComponent<Text>().text = facility._id;
        GetChildByName(name_obj, "TextBox").GetComponent<Text>().text = facility.name;
        GetChildByName(type_obj, "TextBox").GetComponent<Text>().text = facility.type;
        GetChildByName(status_obj, "TextBox").GetComponent<Text>().text = facility.status;
        GetChildByName(dop_obj, "TextBox").GetComponent<Text>().text = facility.dop.Substring(0, 10);
    }

    void UpdateStrings()
    {
        UpdateVars();
        GetChildByName(maint_obj, "TextBox").GetComponent<Text>().text = str_maint;
        GetChildByName(livefeed_obj, "TextBox").GetComponent<Text>().text = str_live;

    }

    void UpdateVars()
    {
        // update mainteance records
        str_maint = null;
        int count = 0;
        if (maintenanceRecords.Length > 0)
        {
            foreach (MaintenanceRecord m in maintenanceRecords) {
                if (count++ == maintenanceRecords.Length)
                    str_maint += string.Format("{0} By {1}: {2}", m.timestamp.Substring(0, 10), m.technician, m.remarks);
                else
                    str_maint += string.Format("{0} By {1}: {2}\r\n", m.timestamp.Substring(0, 10), m.technician, m.remarks);
            }
        }

        // update livefeed
        str_live = null;

        if (facility.status == "Running")
        {
            LiveFeed lf = queryDB.GetLiveFeed();
            str_live = string.Format("{0}\r\nTemperature:\r\n{1} C\r\nPower Consumption:\r\n{2} W", lf.timestamp, lf.temp, lf.wattage);
        }
    }

}
