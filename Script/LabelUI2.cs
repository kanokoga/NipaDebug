using UnityEngine;

namespace NipaDebugs
{
    /// <summary>
    /// 毎フレーム <see cref="LabelManager2"/> から状態を与えられるラベル。
    /// <see cref="LabelUI"/> とは異なり、内部でコールバックによる更新は行わない。
    /// </summary>
    public class LabelUI2 : MonoBehaviour
    {
        public static Camera Camera;

        /// <summary>最後に <see cref="LabelManager2.DrawLabel"/> されたフレーム（未使用スロットは -1）。</summary>
        public int LastDrawFrame { get; set; } = -1;

        [SerializeField] private TextWithBgUI textWithBg;
        [SerializeField] private RectTransform rectTransform;

        private LineUI lineConnection;
        private Vector3 worldPosition;
        private Vector3 offset;
        private bool hasLine;

        public void SetText(string text)
            => this.textWithBg.SetText(text);

        public void SetOffset(Vector3 offset)
            => this.offset = offset;

        public void SetFontSize(int size)
            => this.textWithBg.TextComponent.fontSize = size;

        public void SetTextColor(Color color)
            => this.textWithBg.TextComponent.color = color;

        public void SetWorldPosition(Vector3 position)
            => this.worldPosition = position;

        public void SetBackgroundColor(Color color)
            => this.textWithBg.BackgroundImage.color = color;

        private void Update()
        {
            if(this.LastDrawFrame == Time.frameCount)
            {
                this.RefreshScreenPosition();
            }
        }

        /// <summary>
        /// ワールド座標をスクリーンへ反映し、接続ラインを更新する。
        /// </summary>
        public void RefreshScreenPosition()
        {
            var cam = Camera;
            if(cam == null)
            {
                return;
            }

            var screenPosition = cam.WorldToScreenPoint(this.worldPosition);
            this.rectTransform.position = screenPosition + this.offset;
            if(this.hasLine == true)
            {
                this.lineConnection.startTarget = screenPosition;
                this.lineConnection.endTarget = screenPosition + this.offset;
                this.lineConnection.UpdatePosition();
            }
        }

        public void ApplyLine(float lineWidth, Color lineColor)
        {
            if(lineWidth <= 0f)
            {
                this.DisposeLine();
                return;
            }

            if(this.hasLine == false)
            {
                this.lineConnection = LineManager.Instance.LinePoolFactory.GetObject();
                this.lineConnection.gameObject.SetActive(true);
                this.hasLine = true;
            }

            this.lineConnection.thickness = lineWidth;
            this.lineConnection.color = lineColor;
        }

        public void DisposeLine()
        {
            if(this.hasLine == false)
            {
                return;
            }

            this.lineConnection.gameObject.SetActive(false);
            LineManager.Instance.LinePoolFactory.PoolObject(this.lineConnection);
            this.lineConnection = null;
            this.hasLine = false;
        }
    }
}
