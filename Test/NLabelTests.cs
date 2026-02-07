using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace NipaUIs
{
    public class NLabelTests : MonoBehaviour
    {
        public bool updateNeeded = true;

        private void Start()
        {
            var initConfig = NLabelInitConfig.GetDefConfig;
            initConfig.id = "l1";
            initConfig.message = "Test Label 1";
            initConfig.worldPosition = this.transform.position;
            initConfig.offset = new Vector3(50, 50, 0);
            initConfig.requireUpdate = updateNeeded;

            NLabel.Instance.InitLabel(initConfig);
        }

        private void Update()
        {
            NLabel.Instance.UpdateLabel("l1", this.transform.position);
        }
    }
}
