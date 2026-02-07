using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace NipaUIs
{
    public class NLineTests : MonoBehaviour
    {

        private void Start()
        {
           NLine.Instance.InitLine("test1",
               this.transform.position,
               this.transform.position + new Vector3(10, 10, 0),
               Vector2.zero,
               true);
        }

        private void Update()
        {
            NLine.Instance.UpdateLine("test1",
                this.transform.position,
                this.transform.position + new Vector3(10, 10, 0));
        }

    }
}
