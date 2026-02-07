using UnityEngine;
using UnityEngine.UI;

namespace NipaUIs
{
    public class LabelUI : MonoBehaviour
    {
        public static Camera Camera;

        public int lastUpdateFrame = -1;

        [SerializeField] private TextWithBgUI textWithBg;
        [SerializeField] private RectTransform rectTransform;
        private LineUI lineConnection;
        private Vector3 worldPosition;
        private Vector3 offset;
        private bool hasLine = false;

        public void SetText(string text)
        {
            this.textWithBg.SetText(text);
        }

        public void SetOffset(Vector3 offset)
            => this.offset = offset;

        public void SetFontSize(int size)
        {
            this.textWithBg.TextComponent.fontSize = size;
        }

        public void SetTextColor(Color color)
        {
            this.textWithBg.TextComponent.color = color;
        }

        public void SetWorldPosition(Vector3 position)
            => this.worldPosition = position;

        public void SetBackgroundColor(Color color)
        {
            this.textWithBg.BackgroundImage.color = color;
        }

        public void InitLine(float lineWidth, Color lineColor)
        {
            this.hasLine = lineWidth > 0;
            if(this.hasLine == false)
            {
                return;
            }

            this.lineConnection = NLine.Instance.LinePoolFactory.GetObject();
            this.lineConnection.gameObject.SetActive(true);
            this.lineConnection.thickness = lineWidth;
            this.lineConnection.color = lineColor;
        }

        public void UpdatePosition()
        {
            var screenPosition = Camera.WorldToScreenPoint(this.worldPosition);
            this.rectTransform.position = screenPosition + this.offset;
            if(this.hasLine == true)
            {
                this.lineConnection.startTarget = screenPosition;
                this.lineConnection.endTarget = screenPosition + this.offset;
                this.lineConnection.UpdatePosition();
            }
        }

        private void OnDisable()
        {
            if(this.hasLine == true)
            {
                this.lineConnection.gameObject.SetActive(false);
                NLine.Instance.LinePoolFactory.PoolObject(this.lineConnection);
                this.lineConnection = null;
            }
        }
    }
}
