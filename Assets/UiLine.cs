using System;
using UnityEngine;
using UnityEngine.UI;

public class UILine : Graphic
{
    
    public float thickness = 5f;
    public Vector2 startPoint;
    public Vector2 endPoint;

    public UILine line;
    public RectTransform imgA;
    public RectTransform imgB;

    void Update()
    {
        line.startPoint = imgA.anchoredPosition;
        line.endPoint   = imgB.anchoredPosition;
        line.SetVerticesDirty();   // refresh
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        
        vh.Clear();
        Vector2 direction = (endPoint - startPoint).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness / 2f;

        Vector2 v1 = startPoint - normal;
        Vector2 v2 = startPoint + normal;
        Vector2 v3 = endPoint + normal;
        Vector2 v4 = endPoint - normal;

        UIVertex vertex = UIVertex.simpleVert;

        vertex.color = color;

        vertex.position = v1; vh.AddVert(vertex);
        vertex.position = v2; vh.AddVert(vertex);
        vertex.position = v3; vh.AddVert(vertex);
        vertex.position = v4; vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}
