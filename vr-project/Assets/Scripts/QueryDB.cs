using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;


/// <summary>
/// helper class for converting Json to object array
/// </summary>
public static class JsonHelper
{
	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper);
	}

	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[Serializable]
	private class Wrapper<T>
	{
		public T[] Items;
	}
}


public class QueryDB : MonoBehaviour
{
	// for testing purpose
	private const string URL = "http://localhost:8080/api/facility/";

	// facility ID
	private string f_id;

	// reference to ExtractQRCode script
	private ExtractQRCode extractQRCode;


	private Facility facility;
	private MaintenanceRecord[] maintRecList;
	private LiveFeed livefeed;

    #region accessors
    public Facility GetFacility()
	{
		return facility;
	}

	public MaintenanceRecord[] GetMaintenanceRecords()
	{
		return maintRecList;
	}

	public LiveFeed GetLiveFeed()
    {
		return livefeed;
    }
    #endregion

    public bool IsMaintRecReceived { get; set; }
	public bool IsFacilityInfoReceived { get; set; }


	// Start is called before the first frame update
	void Start()
	{
		// initialize variables
		f_id = null;
		IsFacilityInfoReceived = false;
		IsMaintRecReceived = false;

		// get script reference
		extractQRCode = GameObject.Find("QRCodeExtractionSystem").GetComponent<ExtractQRCode>();

	}

	// Update is called once per frame
	void Update()
	{
		/**
		 * when qr code is extracted from the frame, the script queries database for device
		 * information and maintenance records.
		 */
		if (extractQRCode.IsQueryReady)
		{
			extractQRCode.IsQueryReady = false;
			f_id = extractQRCode.Result;


			StartCoroutine(QueryMaintRec(f_id));
			StartCoroutine(QueryFacilityInfo(f_id));
		}
		
	}

	void StartQueryLiveData()
	{
		StartCoroutine(QueryLiveData(f_id));
	}

	public void StopQueryLiveData()
	{
		CancelInvoke();
	}

	/// <summary>
	/// coroutine for querying facility information
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	IEnumerator QueryFacilityInfo(string id)
	{
		using (UnityWebRequest wr = UnityWebRequest.Get(URL + id))
		{
			yield return wr.SendWebRequest();

			facility = JsonUtility.FromJson<Facility>(wr.downloadHandler.text);
			IsFacilityInfoReceived = true;

			// if device is running, also start to fetch live data every second.
			if (facility.status == "Running")
            {
				InvokeRepeating("StartQueryLiveData", 0f, 1f);
            }
		}
	}

	/// <summary>
	/// coroutine for querying facility maintenance records
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	IEnumerator QueryMaintRec(string id)
	{
		using (UnityWebRequest wr = UnityWebRequest.Get(URL + "maintenance/" + id))
		{
			yield return wr.SendWebRequest();

			maintRecList = JsonHelper.FromJson<MaintenanceRecord>("{\"Items\":" + wr.downloadHandler.text + "}");
			IsMaintRecReceived = true;
		}
	}

	/// <summary>
	/// coroutine for querying facility live data
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	IEnumerator QueryLiveData(string id)
	{
		using (UnityWebRequest wr = UnityWebRequest.Get(URL + "live_feed/" + id))
		{
			yield return wr.SendWebRequest();

			livefeed = JsonUtility.FromJson<LiveFeed>(wr.downloadHandler.text);

		}
	}
}
