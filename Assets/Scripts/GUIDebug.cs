using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration;

public class GUIDebug : MonoBehaviour {

	public GUIText app_version_;
	public GUIText language_;
	public GUIText device_id_;
	public GUIText low_end_;
	public GUIText internet_;

	// Use this for initialization
	void Start () {
#if WINDOWS_PHONE || NETFX_CORE
		app_version_.text = "AppVersion: " + Helper.Instance.GetAppVersion();
		language_.text = "Language: " + Helper.Instance.GetAppLanguage();
		device_id_.text = "Device ID: " + Helper.Instance.GetUserDeviceId();
		low_end_.text = "Is Low End: " + Helper.Instance.IsLowEndDevice();
		internet_.text = "Is Online: " + Helper.Instance.HasInternetConnection;
#endif
	}
	
	// Update is called once per frame
	void Update () {
#if WINDOWS_PHONE || NETFX_CORE	
		language_.text = "Language: " + Helper.Instance.GetAppLanguage();
		internet_.text = "Is Online: " + Helper.Instance.HasInternetConnection;
#endif
	}
}
