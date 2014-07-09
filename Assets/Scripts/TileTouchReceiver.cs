using UnityEngine;
using System.Collections;

public class TileTouchReceiver : MonoBehaviour {
	
	void OnTouchEnded()
	{
		Tile script = transform.parent.gameObject.GetComponent<Tile>();
		script.OnSwitch();
	}
}
