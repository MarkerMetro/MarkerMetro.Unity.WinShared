using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {

	public LayerMask touch_input_mask_;

	// Update is called once per frame
	void Update () {

		GameObject game_master = GameObject.Find("GameMaster");
        GameMaster script = game_master.GetComponent<GameMaster>();
		if ( script.State != GameMaster.GAME_STATE.GS_PLAYING )
		{
			return;
		}
		
		Vector3 touch_position = Vector3.zero;
		bool touched = false;
		// Touch
		foreach ( Touch touch in Input.touches ) 
		{
			if ( touch.phase == TouchPhase.Ended )
			{
				touch_position = touch.position;
				touched = true;
			}
		}

		// Mouse
		if ( Input.GetMouseButtonUp(0) )
		{
			touch_position = Input.mousePosition;
			touched = true;
		}

		if ( touched )
		{
			Ray ray = camera.ScreenPointToRay( touch_position );
			RaycastHit hit;

			if ( Physics.Raycast( ray, out hit, touch_input_mask_ ) )
			{
				GameObject recipient = hit.transform.gameObject;
				recipient.SendMessage("OnTouchEnded");
			}
		}
	}
}
