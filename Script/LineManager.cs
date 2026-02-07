using UnityEngine;
using System.Collections.Generic;

namespace NipaDebugs
{
    using NipaFriends;

    public class LineManager : SingletonMonoBehaviour<LineManager>
    {
        public PoolFactory<LineUI> LinePoolFactory => this.linePoolFactory;

        [SerializeField] private Canvas screenOverlayCanvas;
        [SerializeField] private PoolFactory<LineUI> linePoolFactory
            = new PoolFactory<LineUI>();
        private Dictionary<string, LineUI> activeLines =
            new Dictionary<string, LineUI>();


        private void Update()
        {
            this.UpdateLinePositions();
        }

        private void LateUpdate()
        {
            var currentFrame = Time.frameCount;

            foreach(var kvp in this.activeLines)
            {
                var line = kvp.Value;
                if(line.lastUpdateFrame > 0 && line.lastUpdateFrame != currentFrame)
                {
                    // このフレームで更新されていないラインを非アクティブに
                    line.gameObject.SetActive(false);
                }
            }
        }

        public void InitLine(string id,
            Vector3 startWorldPosition,
            Vector3 endWorldPosition,
            Vector2 offset = default,
            bool needUpdateEveryFrame = true)
            => this.InitLine(id,
                startWorldPosition,
                endWorldPosition,
                offset,
                needUpdateEveryFrame,
                5f,
                Color.white);

        /// <summary>
        /// ラインを初期化・作成します
        /// </summary>
        public void InitLine(string id,
            Vector3 startWorldPosition,
            Vector3 endWorldPosition,
            Vector2 offset = default,
            bool needUpdateEveryFrame = true,
            float thickness = 5f,
            Color color = default)
        {
            if(string.IsNullOrEmpty(id))
            {
                return;
            }

            // 既存のラインを更新
            if(this.activeLines.ContainsKey(id))
            {
                var existingLine = this.activeLines[id];
                existingLine.startTarget = this.WorldToScreenPoint(startWorldPosition) + offset;
                existingLine.endTarget = this.WorldToScreenPoint(endWorldPosition) + offset;
                existingLine.thickness = thickness;
                existingLine.color = color == default ? Color.white : color;
                existingLine.SetVerticesDirty();
                return;
            }

            // 新しいラインを作成
            var lineUI = this.linePoolFactory.GetObject();
            lineUI.gameObject.SetActive(true);

            // 設定
            lineUI.startTarget = this.WorldToScreenPoint(startWorldPosition) + offset;
            lineUI.endTarget = this.WorldToScreenPoint(endWorldPosition) + offset;
            lineUI.thickness = thickness;
            lineUI.color = color;
            lineUI.lastUpdateFrame = needUpdateEveryFrame == true
                ? Time.frameCount
                : -1;
            lineUI.SetVerticesDirty();

            this.activeLines[id] = lineUI;
        }

        /// <summary>
        /// ラインを更新します（両端の位置）
        /// </summary>
        public void UpdateLine(string id, Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var line = this.activeLines[id];
            line.startTarget = this.WorldToScreenPoint(startWorldPosition);
            line.endTarget = this.WorldToScreenPoint(endWorldPosition);
            if(line.lastUpdateFrame >= 0)
            {
                line.lastUpdateFrame = Time.frameCount;
            }
            line.SetVerticesDirty();
        }

        /// <summary>
        /// ラインの開始位置を更新します
        /// </summary>
        public void UpdateLine(string id, Vector3 startWorldPosition)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var line = this.activeLines[id];
            line.startTarget = this.WorldToScreenPoint(startWorldPosition);
            if(line.lastUpdateFrame >= 0)
            {
                line.lastUpdateFrame = Time.frameCount;
            }
            line.SetVerticesDirty();
        }

        /// <summary>
        /// ラインの終了位置を更新します
        /// </summary>
        public void UpdateLineEnd(string id, Vector3 endWorldPosition)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var line = this.activeLines[id];
            line.endTarget = this.WorldToScreenPoint(endWorldPosition);
            if(line.lastUpdateFrame >= 0)
            {
                line.lastUpdateFrame = Time.frameCount;
            }
            line.SetVerticesDirty();
        }

        /// <summary>
        /// ラインの色を更新します
        /// </summary>
        public void UpdateLineColor(string id, Color color)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var line = this.activeLines[id];
            line.color = color;
            if(line.lastUpdateFrame >= 0)
            {
                line.lastUpdateFrame = Time.frameCount;
            }
            line.SetVerticesDirty();
        }

        /// <summary>
        /// ラインの太さを更新します
        /// </summary>
        public void UpdateLineThickness(string id, float thickness)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var line = this.activeLines[id];
            line.thickness = thickness;
            if(line.lastUpdateFrame >= 0)
            {
                line.lastUpdateFrame = Time.frameCount;
            }
            line.SetVerticesDirty();
        }

        /// <summary>
        /// ラインを削除します
        /// </summary>
        public void RemoveLine(string id)
        {
            if(!this.activeLines.ContainsKey(id))
            {
                return;
            }

            var lineUI = this.activeLines[id];
            this.linePoolFactory.PoolObject(lineUI);
            this.activeLines.Remove(id);
        }

        /// <summary>
        /// すべてのラインを削除します
        /// </summary>
        public void ClearAllLines()
        {
            foreach(var kvp in this.activeLines)
            {
                this.linePoolFactory.PoolObject(kvp.Value);
            }

            this.activeLines.Clear();
        }

        private void UpdateLinePositions()
        {
            foreach(var kvp in this.activeLines)
            {
                var lineUI = kvp.Value;
                lineUI.UpdatePosition();
            }
        }

        private Vector2 WorldToScreenPoint(Vector3 worldPosition)
        {
            var camera = Camera.main;
            if(camera == null)
            {
                return Vector2.zero;
            }

            return camera.WorldToScreenPoint(worldPosition);
        }
    }
}
