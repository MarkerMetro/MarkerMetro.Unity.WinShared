using UnityEngine;
using System.Collections;

namespace MarkerMetro.Unity.WinShared.Example
{
    public class TileTouchReceiver : MonoBehaviour
    {

        void OnTouchEnded()
        {
            Tile script = transform.parent.gameObject.GetComponent<Tile>();
            script.OnSwitch();
        }
    }
}