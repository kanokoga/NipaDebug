using System;
using UnityEngine;
using System.Collections.Generic;

namespace NipaDebugs
{
    using NipaFriends;

    public struct LabelInitConfig
    {
        public string id;
        public string message;
        public Vector3 worldPosition;
        public Vector3 offset;
        public int fontSize;
        public Color textColor;
        public Color backgroundColor;
        public float lineWidth;
        public Func<LabelConfig> configUpdater;

        public static LabelInitConfig GetDefConfig = new LabelInitConfig
        {
            id = "",
            message = "",
            worldPosition = Vector3.zero,
            offset = new Vector3(20f, 20f, 0f),
            fontSize = 12,
            textColor = Color.white,
            backgroundColor = Color.black,
            lineWidth = 2f,
            configUpdater = null
        };
    }

    public struct LabelConfig
    {
        public LabelConfig(LabelInitConfig initConfig)
        {
            this.message = initConfig.message;
            this.worldPosition = initConfig.worldPosition;
            this.offset = initConfig.offset;
            this.textColor = initConfig.textColor;
            this.backgroundColor = initConfig.backgroundColor;
        }

        public string message;
        public Vector3 worldPosition;
        public Vector3 offset;
        public Color textColor;
        public Color backgroundColor;
    }

    public class LabelManager : SingletonMonoBehaviour<LabelManager>
    {
        public bool IsActive { get; private set; } = true;

        [SerializeField] private Canvas screenOverlayCanvas;

        [SerializeField] private PoolFactory<LabelUI> labelPoolFactory
            = new PoolFactory<LabelUI>();

        private Dictionary<string, LabelUI> activeLabels =
            new Dictionary<string, LabelUI>();

        [SerializeField] private GameObject pool = null;


        private void Awake()
        {
            LabelUI.Camera = Camera.main;
        }

        public void SetActive(bool active)
        {
            this.IsActive = active;
            this.pool.SetActive(active);
            this.screenOverlayCanvas.enabled = active;
        }

        /// <summary>
        /// ラベルを初期化・作成します
        /// </summary>
        public void InitLabel(LabelInitConfig config)
        {
            var id = config.id;
            var message = config.message;
            var worldPosition = config.worldPosition;
            var offset = config.offset;
            var fontSize = config.fontSize;
            var textColor = config.textColor;
            var backgroundColor = config.backgroundColor;
            var lineWidth = config.lineWidth;

            if(string.IsNullOrEmpty(id))
            {
                return;
            }

            if(this.activeLabels.ContainsKey(id))
            {
                this.RemoveLabel(id);
                return;
            }

            // 新しいラベルを作成
            var labelUI = this.labelPoolFactory.GetObject();
            labelUI.gameObject.SetActive(true);

            // 設定
            labelUI.SetText(message);
            labelUI.SetWorldPosition(worldPosition);
            labelUI.SetOffset(offset);
            labelUI.SetFontSize(fontSize);
            labelUI.SetTextColor(textColor);
            labelUI.SetBackgroundColor(backgroundColor);
            labelUI.SetLabelConfigCallback(config.configUpdater);
            labelUI.InitLine(lineWidth, backgroundColor);

            this.activeLabels[id] = labelUI;
        }


        /// <summary>
        /// ラベルを更新します（メッセージと位置）
        /// </summary>
        public void UpdateLabel(string id, string message, Vector3 worldPosition)
        {
            if(!this.activeLabels.ContainsKey(id))
            {
                return;
            }

            var label = this.activeLabels[id];
            label.SetText(message);
        }

        /// <summary>
        /// ラベルの位置を更新します
        /// </summary>
        public void UpdateLabel(string id, Vector3 worldPosition)
        {
            if(!this.activeLabels.ContainsKey(id))
            {
                return;
            }

            var label = this.activeLabels[id];
            label.SetWorldPosition(worldPosition);
        }

        /// <summary>
        /// ラベルのメッセージを更新します
        /// </summary>
        public void UpdateLabel(string id, string message)
        {
            if(!this.activeLabels.ContainsKey(id))
            {
                return;
            }

            var label = this.activeLabels[id];
            label.SetText(message);
        }

        /// <summary>
        /// ラベルのオフセットを更新します
        /// </summary>
        public void UpdateLabelOffset(string id, Vector3 offset)
        {
            if(!this.activeLabels.ContainsKey(id))
            {
                return;
            }

            var label = this.activeLabels[id];
            label.SetOffset(offset);
        }

        /// <summary>
        /// ラベルを削除します
        /// </summary>
        public void RemoveLabel(string id)
        {
            if(!this.activeLabels.ContainsKey(id))
            {
                return;
            }

            var labelUI = this.activeLabels[id];
            labelUI.Dispose();
            this.labelPoolFactory.PoolObject(labelUI);
            this.activeLabels.Remove(id);
        }

        /// <summary>
        /// すべてのラベルを削除します
        /// </summary>
        public void ClearAllLabels()
        {
            foreach(var kvp in this.activeLabels)
            {
                this.labelPoolFactory.PoolObject(kvp.Value);
            }

            this.activeLabels.Clear();
        }
    }
}
