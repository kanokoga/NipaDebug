using UnityEngine;
using UnityEngine.UI;

namespace NipaUIs
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class LineUI : MaskableGraphic
    {
        public int lastUpdateFrame = -1;
        public Vector2 startTarget;
        public Vector2 endTarget;
        public float thickness = 5f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // ターゲット間の相対座標を取得
            var pos1 = this.startTarget;
            var pos2 = this.endTarget;

            // 距離が非常に小さい場合は描画しない
            var distance = Vector2.Distance(pos1, pos2);
            if(distance < 0.1f)
            {
                return;
            }

            // 線の方向と垂直なベクトルを計算（厚みの計算用）
            var dir = (pos2 - pos1).normalized;
            var perp = new Vector2(-dir.y, dir.x) * (this.thickness * 0.5f);

            // 四角形の4頂点を生成
            var vertex = UIVertex.simpleVert;
            vertex.color = this.color;

            vertex.position = pos1 - perp;
            vh.AddVert(vertex);

            vertex.position = pos1 + perp;
            vh.AddVert(vertex);

            vertex.position = pos2 + perp;
            vh.AddVert(vertex);

            vertex.position = pos2 - perp;
            vh.AddVert(vertex);

            // 三角形2つで四角形を描画
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        /// <summary>
        /// ラインの位置を更新します
        /// </summary>
        public void UpdatePosition()
        {
            // 必要に応じて再描画
            this.SetVerticesDirty();
        }
    }
}
