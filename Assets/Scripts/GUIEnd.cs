using UnityEngine;
using System.Collections;

public class GUIEnd : MonoBehaviour {

	void OnGUI()
	{
		// Make a background box
		int half_width = Screen.width / 2;
		int half_height = Screen.height / 2;

		int box_width = 200;
		int box_height = 80;

		int box_x = half_width - box_width / 2;
		int box_y = half_height - box_height / 2;

        GUI.Box(new Rect( box_x, box_y, box_width, box_height), "Main Menu");
    
        // Make the first button. If it is pressed, Start Game
        if(GUI.Button(new Rect( box_x + 10 , box_y + 30, box_width - 20, 40), "Reset Game")) {
            GameObject game_master = GameObject.Find("GameMaster");
            GameMaster script = game_master.GetComponent<GameMaster>();
            script.ChangeState( GameMaster.GAME_STATE.GS_START );
        }
	}
}
