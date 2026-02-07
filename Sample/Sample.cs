using System;
using NipaUIs;
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
            var config = NLabelInitConfig.GetDefConfig;
            config.id = this.id;
            config.textColor = this.textColor;
            config.backgroundColor = this.backgroundColor;
            NLabel.Instance.InitLabel(config);

            var config2 = NLabelInitConfig.GetDefConfig;
            config2.id = "static_label";
            config2.message = "I am static!";
            config2.worldPosition = new Vector3(0f, 0f, 0f);
            config2.textColor = Color.yellow;
            config2.backgroundColor = Color.blue;
            config2.requireUpdate = false;
            NLabel.Instance.InitLabel(config2);
        }

        private void Update()
        {
            var x = Mathf.Sin(Time.time);
            var z = Mathf.Cos(Time.time);
            this.transform.position = new Vector3(x * 5f, 0f, z * 5f);

            NLabel.Instance.UpdateLabel(this.id,
                this.transform.position.ToString(),
                this.transform.position);
        }
    }
}
