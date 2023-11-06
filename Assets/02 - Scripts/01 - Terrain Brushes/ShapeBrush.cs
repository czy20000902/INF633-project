using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrushShape
{
    square,
    circle,
}
public class ShapeBrush : TerrainBrush 
{
    public float incrementalHeight = 5;
    public BrushShape shapeType = BrushShape.square;
    public override void draw(int x, int z) 
    {
        float radiusSquare = radius * radius;

        for (int zi = -radius; zi <= radius; zi++) 
            for (int xi = -radius; xi <= radius; xi++) 
            {
                if(shapeType == BrushShape.circle)
                    if ((xi * xi + zi * zi) > radiusSquare)
                        continue; // not inside the circle
                terrain.set(x + xi, z + zi, incrementalHeight);
            }
    }
}