using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon Transform Span")]
    public class UIPolygonTransformSpan : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public bool fill = true;
        public List<Transform> transforms = new List<Transform>();
        public float thickness = 5;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (transforms.Count > 2)
            {
                int vertices = transforms.Count + 1;
                List<Vector2> points = new List<Vector2>();

                for (int i = 0; i < transforms.Count; i++)
                {
                    points.Add(new Vector2((transforms[i].position.x - transform.position.x)/transform.lossyScale.x, (transforms[i].position.y - transform.position.y) / transform.lossyScale.y));
                }
                points.Add(new Vector2((transforms[0].position.x - transform.position.x) / transform.lossyScale.x, (transforms[0].position.y - transform.position.y) / transform.lossyScale.y));
                points.Add(new Vector2((transforms[1].position.x - transform.position.x) / transform.lossyScale.x, (transforms[1].position.y - transform.position.y) / transform.lossyScale.y));


                Vector2 uv0 = new Vector2(0, 1);
                Vector2 uv1 = new Vector2(1, 1);
                Vector2 uv2 = new Vector2(1, 0);
                Vector2 uv3 = new Vector2(0, 0);
                for (int i = 0; i < vertices; i++)
                {
                    vh.AddUIVertexQuad(SetVbo(new[] { points[i], Vector2.zero, Vector2.zero, points[i+1] }, new[] { uv0, uv1, uv2, uv3 }));
                }
            }
        }
    }
}
