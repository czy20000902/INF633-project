using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalIncreaseBrush : TerrainBrush
{
    public float increment = 1; // Amount to increase the terrain height by
    public int radius = 20; // Define the brush radius

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                // Use terrain.set to increment the height at the specified coordinates
                float currentHeight = terrain.get(x + xi, z + zi);
                terrain.set(x + xi, z + zi, currentHeight + increment);
            }
        }
    }
}