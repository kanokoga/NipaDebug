using System.Collections.Generic;
using UnityEngine;

namespace NipaDebugs
{
    using NipaFriends;

    /// <summary>
    /// 1 フレーム分のラベル描画スタイル（<see cref="LabelManager2.DrawLabel"/> 用）。
    /// </summary>
    public struct LabelDraw2Style
    {
        public Vector3 offset;
        public int fontSize;
        public Color textColor;
        public Color backgroundColor;
        public float lineWidth;

        public static LabelDraw2Style Default => new LabelDraw2Style
        {
            offset = new Vector3(20f, 20f, 0f),
            fontSize = 12,
            textColor = Color.white,
            backgroundColor = Color.black,
            lineWidth = 2f,
        };
    }

    /// <summary>
    /// Unity の <c>Debug</c> に近いラベル表示。利用側は毎フレーム <see cref="DrawLabel"/> を呼ぶ必要がある。
    /// 呼ばれなかったスロットの <see cref="LabelUI2"/> はオブジェクトプールへ戻る。
    /// </summary>
    public class LabelManager2 : SingletonMonoBehaviour<LabelManager2>
    {
        public bool IsActive { get; private set; } = true;

        [SerializeField] private Canvas screenOverlayCanvas;

        [SerializeField] private PoolFactory<LabelUI2> labelPoolFactory
            = new PoolFactory<LabelUI2>();

        [SerializeField] private GameObject pool;

        private readonly List<LabelUI2> rentedLabels = new List<LabelUI2>();

        private void Awake()
        {
            LabelUI2.Camera = Camera.main;
        }

        public void SetActive(bool active)
        {
            this.IsActive = active;
            if(this.pool != null)
            {
                this.pool.SetActive(active);
            }

            if(this.screenOverlayCanvas != null)
            {
                this.screenOverlayCanvas.enabled = active;
            }
        }

        /// <summary>
        /// 既定スタイルでラベルを 1 件描画登録する（このフレーム内の呼び出し順でスロットが決まる）。
        /// </summary>
        public void DrawLabel(Vector3 worldPosition, string message)
            => this.DrawLabel(worldPosition, message, LabelDraw2Style.Default);

        public void DrawLabel(Vector3 worldPosition, string message, Vector2 offset)
        {
            var style = LabelDraw2Style.Default;
            style.offset = offset;
            this.DrawLabel(worldPosition, message, style);
        }

        /// <summary>
        /// スタイルを指定してラベルを 1 件描画登録する。
        /// </summary>
        public void DrawLabel(Vector3 worldPosition, string message, in LabelDraw2Style style)
        {
            if(this.IsActive == false)
            {
                return;
            }

            var frame = Time.frameCount;
            LabelUI2 label = null;
            for(var i = 0; i < this.rentedLabels.Count; i++)
            {
                if(this.rentedLabels[i].LastDrawFrame != frame)
                {
                    label = this.rentedLabels[i];
                    break;
                }
            }

            if(label == null)
            {
                label = this.labelPoolFactory.GetObject();
                this.rentedLabels.Add(label);
            }

            label.LastDrawFrame = frame;
            label.gameObject.SetActive(true);
            label.SetText(message);
            label.SetWorldPosition(worldPosition);
            label.SetOffset(style.offset);
            label.SetFontSize(style.fontSize);
            label.SetTextColor(style.textColor);
            label.SetBackgroundColor(style.backgroundColor);
            label.ApplyLine(style.lineWidth, style.backgroundColor);
            label.RefreshScreenPosition();
        }

        private void LateUpdate()
        {
            var frame = Time.frameCount;
            for(var i = this.rentedLabels.Count - 1; i >= 0; i--)
            {
                if(this.rentedLabels[i].LastDrawFrame != frame)
                {
                    var stale = this.rentedLabels[i];
                    stale.DisposeLine();
                    stale.gameObject.SetActive(false);
                    this.labelPoolFactory.PoolObject(stale);
                    this.rentedLabels.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 保持中のラベルをすべてプールへ戻す。
        /// </summary>
        public void ClearAllLabels()
        {
            for(var i = 0; i < this.rentedLabels.Count; i++)
            {
                var label = this.rentedLabels[i];
                label.DisposeLine();
                label.gameObject.SetActive(false);
                this.labelPoolFactory.PoolObject(label);
            }

            this.rentedLabels.Clear();
        }
    }
}
