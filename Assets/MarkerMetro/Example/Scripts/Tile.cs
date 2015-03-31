using UnityEngine;
using System.Collections;

namespace MarkerMetro.Unity.WinShared.Example
{
    public class Tile : MonoBehaviour
    {

        public string name_;

        public void SetImage(string image_name, Texture2D texture)
        {
            name_ = image_name;
            Renderer renderer = GetComponent<MeshRenderer>();
            renderer.material.mainTexture = texture;
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }

        void OnTouchEnded()
        {
        }

        public void Rotate()
        {
            transform.Rotate(0, 180, 0);
        }

        public void OnSwitch()
        {
            Rotate();

            GameObject game_master = GameObject.Find("GameMaster");
            GameMaster script = game_master.GetComponent<GameMaster>();
            script.OnTileSwitch(GetComponent<Tile>());
        }
    }
}