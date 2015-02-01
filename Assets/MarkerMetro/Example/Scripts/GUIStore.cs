using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration.Store;

public class GUIStore : MonoBehaviour {

    const float Offset = 10f;
    const float WindowWidth = 200;
    const float ButtonHeight = 40f;

    GameMaster _gameMasterScript;

    void Start ()
    {
        GameObject gameMasterObject = GameObject.Find("GameMaster");
        _gameMasterScript = gameMasterObject.GetComponent<GameMaster>();
    }

	void OnGUI()
	{
        GUILayout.Window(0, new Rect((Screen.width - WindowWidth) * 0.5f, Offset, 0, 0), (windowID) =>
        {
            int productCount = (_gameMasterScript.StoreProducts == null) ? 0 : _gameMasterScript.StoreProducts.Count;

            for (int i = 0; i < productCount; ++i)
            {
                Product product = _gameMasterScript.StoreProducts[i];
                string name = product.Name;

                if (GUILayout.Button(name, GUILayout.MinHeight(ButtonHeight))) 
                {
                    _gameMasterScript.PurchaseMove(product);
                }
            }

            if (GUILayout.Button("Exit", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_START);
            }

        }, "IAP Store", GUILayout.MinWidth(WindowWidth), GUILayout.MaxWidth(WindowWidth));
    }
}
