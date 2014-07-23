using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration.Store;

public class GUIStore : MonoBehaviour {
	private int number_iap_packs_ = 0;

	void OnGUI()
	{
 		// Make a background box
		int half_width = Screen.width / 2;
		int half_height = Screen.height / 2;

		int box_width = 200;
		int box_height = (number_iap_packs_ + 1) * 50 + 30;

		int box_x = half_width - box_width / 2;
		int box_y = half_height - box_height / 2;

		int button_offset_y = 50;

        GUI.Box(new Rect( box_x, box_y, box_width, box_height), "IAP Store");
    
 		int current_offset = 30;

 		for ( int i = 0; i < number_iap_packs_; ++ i )
 		{
 			string name = "IAP " + i.ToString();
 			// Second Button Login to FB
	        if(GUI.Button(new Rect( box_x + 10 , box_y + current_offset, box_width - 20, 40), name)) 
	        {

	        }
	        current_offset += button_offset_y;
 		}

        // Second Button Login to FB
        if(GUI.Button(new Rect( box_x + 10 , box_y + current_offset, box_width - 20, 40), "Exit")) 
        {
            GameObject game_master = GameObject.Find("GameMaster");
            GameMaster script = game_master.GetComponent<GameMaster>();
            script.ChangeState( GameMaster.GAME_STATE.GS_PLAYING );
        }


		string to_edit = "Type Here";
		to_edit = GUI.TextField (new Rect (200, 200, 200, 80), to_edit);
	}
}
