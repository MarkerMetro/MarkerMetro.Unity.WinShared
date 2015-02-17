using UnityEngine;
using System.Collections;

namespace MarkerMetro.Unity.WinShared.Example
{
    public class Rotate : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, 50.0f * Time.deltaTime);
        }
    }
}