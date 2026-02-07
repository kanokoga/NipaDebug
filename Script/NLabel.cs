using UnityEngine;
using System.Collections.Generic;

namespace NipaUIs
{
    using NipaFriends;

    public struct NLabelInitConfig
    {
        public string id;
        public string message;
        public Vector3 worldPosition;
        public Vector3 offset;
        public bool requireUpdate;
        public int fontSize;
        public Color textColor;
        public Color backgroundColor;
        public float lineWidth;

        public static NLabelInitConfig GetDefConfig = new NLabelInitConfig
        {
            id = "",
            message = "",
            worldPosition = Vector3.zero,
            offset = new Vector3(20f, 20f, 0f),
            requireUpdate = true,
            fontSize = 12,
            textColor = Color.white,
            backgroundColor = Color.black,
            lineWidth = 2f
        };
    }

    public class NLabel : SingletonMonoBehaviour<NLabel>
    {
        [SerializeField] private Canvas screenOverlayCanvas;

        [SerializeField] private PoolFactory<LabelUI> labelPoolFactory
            = new PoolFactory<LabelUI>();

        private Dictionary<string, LabelUI> activeLabels =
            new Dictionary<string, LabelUI>();


        private void Awake()
        {
            LabelUI.Camera = Camera.main;
        }

        private void Update()
        {
            this.UpdateLabelPositions();
        }

        private void LateUpdate()
        {
            var currentFrame = Time.frameCount;

            foreach(var kvp in this.activeLabels)
            {
                var label = kvp.Value;
                if(label.lastUpdateFrame > 0 && label.lastUpdateFrame != currentFrame)
                {
                    // このフレームで更新されていないラベルを非アクティブに
                    label.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// ラベルを初期化・作成します
        /// </summary>
        public void InitLabel(NLabelInitConfig config)
        {
            var id = config.id;
            var message = config.message;
            var worldPosition = config.worldPosition;
            var offset = config.offset;
            var needUpdateEveryFrame = config.requireUpdate;
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
                this.UpdateLabel(id, message, worldPosition);
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
            labelUI.lastUpdateFrame = needUpdateEveryFrame == true
                ? Time.frameCount
                : -1;
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

            label.SetWorldPosition(worldPosition);
            if(label.lastUpdateFrame >= 0)
            {
                label.lastUpdateFrame = Time.frameCount;
            }
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
            if(label.lastUpdateFrame >= 0)
            {
                label.lastUpdateFrame = Time.frameCount;
            }
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
            if(label.lastUpdateFrame >= 0)
            {
                label.lastUpdateFrame = Time.frameCount;
            }
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
            if(label.lastUpdateFrame >= 0)
            {
                label.lastUpdateFrame = Time.frameCount;
            }
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

        private void UpdateLabelPositions()
        {
            foreach(var kvp in this.activeLabels)
            {
                var labelUI = kvp.Value;
                labelUI.UpdatePosition();
            }
        }
    }
}
