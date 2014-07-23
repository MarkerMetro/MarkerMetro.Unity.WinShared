using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public GameObject 	gui_start_;
	public GameObject 	gui_play_;
	public GameObject 	gui_end_;
	public GameObject	gui_store_;

	public GUIText 		gui_matches_;
	public GUIText 		gui_remaining_;
	public GUIText 		gui_result_;

	public GAME_STATE state_;

	private List<GameObject> tiles_ = new List<GameObject>();
	private string[] names_ = {"Keith", "Tony", "Greg", "Nigel", "Ivan", "Chad", "Damian", "Brian" };
	private Tile current_switched_1 = null;
	private Tile current_switched_2 = null;
	private float waiting_timer_ = 0.0f;

	private int max_moves_ = 15;
	private int remaining_moves_ = 0;
	private int number_matches_ = 0;

	const int rows_ = 4;
	const int cols_ = 4;
	const float wait_after_switch_ = 0.5f;

	public enum GAME_STATE
	{
		GS_START,
		GS_PLAYING,
		GS_END,
		GS_WAITING,
		GS_STORE
	};

	// Use this for initialization
	void Start () {

		CreateTiles();
		ChangeState( GAME_STATE.GS_START );
	}
	
	// Update is called once per frame
	void Update () {
		if ( state_ == GAME_STATE.GS_WAITING )
		{
			if ( waiting_timer_ > wait_after_switch_ )
			{
				waiting_timer_ = 0.0f;
				ChangeState( GAME_STATE.GS_PLAYING );
			}
			else
			{
				waiting_timer_ += Time.deltaTime;
			}
		}

		if ( state_ == GAME_STATE.GS_PLAYING )
		{
			if ( number_matches_ == tiles_.Count / 2 )
			{
				// Win
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU WIN";
			}
			else if ( remaining_moves_ == 0 )
			{
				// Loss
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU LOSE";
			}
		}
	}

	// Change the games state
	public void ChangeState( GAME_STATE state )
	{
		switch ( state )
		{
			case GAME_STATE.GS_START:
			{
				gui_start_.SetActive( true );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;

				remaining_moves_ = max_moves_;
				number_matches_ = 0;
				gui_result_.text = "YOU LOSE";

				SetupTiles();
			}
			break;
			case GAME_STATE.GS_PLAYING:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( false );
				gui_play_.SetActive( true );
				gui_store_.SetActive( false );
				state_ = state;

				if ( current_switched_1 != null )
				{
					current_switched_1.Rotate();
					current_switched_1 = null;
				}
				if ( current_switched_2 != null )
				{
					current_switched_2.Rotate();
					current_switched_2 = null;
				}

				SetGUIText();
			}
			break;
			case GAME_STATE.GS_END:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( true );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;
			}
			break;
			case GAME_STATE.GS_WAITING:
			{
				waiting_timer_ = 0.0f;
				state_ = state;
			}
			break;
			case GAME_STATE.GS_STORE:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( true );
				state_ = state;
			}
			break;
			default: break;
		}
	}

	void CreateTiles()
	{
		GameObject tile_base = GameObject.Find("GameTile");
		for ( int x = 0; x < rows_; ++x )
		{
			for ( int y = 0; y < cols_; ++y )
			{
				GameObject new_tile = Instantiate( tile_base, new Vector3( x * 1.5f, y * 1.5f, 0.0f ), Quaternion.identity ) as GameObject;
				tiles_.Add( new_tile );
			}
		}
	}

	void SetupTiles()
	{
		int number_tiles = tiles_.Count;
		for ( int i = 0; i < number_tiles; ++i )
		{
			var temp = tiles_[i];
			int random_index = Random.Range( i, tiles_.Count );
			tiles_[i] = tiles_[ random_index ];
			tiles_[ random_index ] = temp;
		}

		for ( int i = 0; i < number_tiles / 2; ++i )
		{
			Tile script_1 = tiles_[i * 2].GetComponent<Tile>();
			script_1.SetImage( names_[i] );
			Tile script_2 = tiles_[i * 2 + 1].GetComponent<Tile>();
			script_2.SetImage( names_[i] );
		}
	}

	public void OnTileSwitch( Tile script )
	{
		if ( current_switched_1 == null )
		{
			current_switched_1 = script;
		}
		else
		{
			--remaining_moves_;
			if ( current_switched_1.name_ == script.name_ )
			{
				// match
				current_switched_1 = null;
				++number_matches_;
			}
			else
			{
				current_switched_2 = script;
				ChangeState( GAME_STATE.GS_WAITING );
			}
		}

		SetGUIText();
	}

	void SetGUIText()
	{
		gui_matches_.text = "Matches: " + number_matches_.ToString();
		gui_remaining_.text = "Moves Remaining: " + remaining_moves_.ToString();
	}

	public void AddMoves()
	{
		remaining_moves_ += 5;
		SetGUIText();
	}
}
