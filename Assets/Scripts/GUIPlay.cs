using UnityEngine;
using System.Collections;

public class GUIPlay : MonoBehaviour {

	void OnGUI()
	{
		// Make a background box
        GUI.Box(new Rect(10,10,100,160), "Game Playing");
    
        // Make the first button. IAP
		if(GUI.Button(new Rect(20,40,80,50), "Buy Moves")) {
			GameObject game_master = GameObject.Find("GameMaster");
            GameMaster script = game_master.GetComponent<GameMaster>();
            script.ChangeState( GameMaster.GAME_STATE.GS_STORE );
        }

        if(GUI.Button(new Rect(20,100,80,50), "End")) {
            GameObject game_master = GameObject.Find("GameMaster");
            GameMaster script = game_master.GetComponent<GameMaster>();
            script.ChangeState( GameMaster.GAME_STATE.GS_END );
        }
	}
}
