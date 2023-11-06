using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InstanceBrushShape
{
    square,
    circle,
}

public class ShapeInstanceBrush : InstanceBrush
{
    public InstanceBrushShape instanceBrushShape = InstanceBrushShape.square;

    public override void draw(float x, float z)
    {
        float radiusSquare = radius * radius;
        float xi = Random.Range(-radius, radius);
        float zi = Random.Range(-radius, radius);
        if (instanceBrushShape == InstanceBrushShape.square)
            spawnObject(x + xi, z + zi);
        else
            if ((xi * xi + zi * zi) > radiusSquare)
                spawnObject(x, z);
            else
                spawnObject(x + xi, z + zi);
    }
}


