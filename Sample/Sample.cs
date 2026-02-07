using System;
using NipaDebugs;
using UnityEngine;

namespace NipaUI.Samples
{
    public class Sample : MonoBehaviour
    {
        public Color textColor = Color.white;
        public Color backgroundColor = Color.green;
        private string id = "sample_label";

        private void Start()
        {
            var initconfig = LabelInitConfig.GetDefConfig;
            initconfig.id = this.id;
            initconfig.textColor = this.textColor;
            initconfig.backgroundColor = this.backgroundColor;
            initconfig.fontSize = 20;

            var config = new LabelConfig(initconfig);
            initconfig.configUpdater = () =>
            {
                config.message = Time.time.ToString();
                config.worldPosition = this.transform.position;
                return config;
            };

            LabelManager.Instance.InitLabel(initconfig);
        }

        private void Update()
        {
            var x = Mathf.Sin(Time.time);
            var z = Mathf.Cos(Time.time);
            this.transform.position = new Vector3(x * 5f, 0f, z * 5f);
        }
    }
}
