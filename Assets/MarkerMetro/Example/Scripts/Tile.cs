using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public string name_;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetImage( string image_name, Texture2D texture )
	{
		name_ = image_name;
		Renderer renderer = GetComponent<MeshRenderer>().renderer;
		renderer.material.mainTexture = texture;
		transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
	}

	void OnTouchEnded()
	{
	}

	public void Rotate()
	{
		transform.Rotate( 0, 180, 0 );
	}

	public void OnSwitch()
	{
		Rotate();

		GameObject game_master = GameObject.Find("GameMaster");
        GameMaster script = game_master.GetComponent<GameMaster>();
        script.OnTileSwitch( GetComponent<Tile>() );
	}
}
