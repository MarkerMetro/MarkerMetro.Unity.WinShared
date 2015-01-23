using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration;

public class GUIDebug : MonoBehaviour
{

    [SerializeField]
    private GUIText _appVersion;
    [SerializeField]
    private GUIText _language;
    [SerializeField]
    private GUIText _deviceID;
    [SerializeField]
    private GUIText _lowEnd;
    [SerializeField]
    private GUIText _internet;
    [SerializeField]
    private GUIText _meteredConnection;

    void Start()
    {
#if !UNITY_EDITOR
        _appVersion.text = "AppVersion: " + Helper.Instance.GetAppVersion();
        _language.text = "Language: " + Helper.Instance.GetAppLanguage();
        _deviceID.text = "Device ID: " + Helper.Instance.GetUserDeviceId();
        _lowEnd.text = "Is Low End: " + Helper.Instance.IsLowEndDevice();
        _internet.text = "Is Online: " + Helper.Instance.HasInternetConnection;
        _meteredConnection.text = "Is metered connection: " + Helper.Instance.IsMeteredConnection;
#endif
    }

    void Update()
    {
#if !UNITY_EDITOR
        _language.text = "Language: " + Helper.Instance.GetAppLanguage();
        _internet.text = "Is Online: " + Helper.Instance.HasInternetConnection;
        _meteredConnection.text = "Is metered connection: " + Helper.Instance.IsMeteredConnection;
#endif
    }
}
