using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration.Store;

public class GUIStore : MonoBehaviour {

    GameMaster gameMaster;

    void Start ()
    {
        GameObject gameMasterObject = GameObject.Find("GameMaster");
        gameMaster = gameMasterObject.GetComponent<GameMaster>();

        gameMaster.RetrieveProducts();
    }

	void OnGUI()
	{
        int productCount = (gameMaster.StoreProducts == null) ? 0 : gameMaster.StoreProducts.Count;

 		// Make a background box
		int half_width = Screen.width / 2;
		int half_height = Screen.height / 2;

		int box_width = 200;
		int box_height = (productCount + 1) * 50 + 30;

		int box_x = half_width - box_width / 2;
		int box_y = half_height - box_height / 2;

		int button_offset_y = 50;

        GUI.Box(new Rect( box_x, box_y, box_width, box_height), "IAP Store");
    
 		int current_offset = 30;

 		for ( int i = 0; i < productCount; ++ i )
 		{
            Product product = gameMaster.StoreProducts[i];
 			string name = product.Name;

            if(GUI.Button(new Rect( box_x + 10 , box_y + current_offset, box_width - 20, 40), name)) 
	        {
                gameMaster.PurchaseMove(product);
	        }
	        current_offset += button_offset_y;
 		}

        // Exit back to game.
        if(GUI.Button(new Rect( box_x + 10 , box_y + current_offset, box_width - 20, 40), "Exit")) 
        {
            gameMaster.ChangeState(GameMaster.GAME_STATE.GS_PLAYING);
        }
    }
}
