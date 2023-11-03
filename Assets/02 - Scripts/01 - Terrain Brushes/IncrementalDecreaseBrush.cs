using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalDecreaseBrush : TerrainBrush
{
    public float decrement = 1; // Amount to decrease the terrain height by
    public int radius = 20; // Define the brush radius

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                // Use terrain.get to retrieve the current height at the specified coordinates
                float currentHeight = terrain.get(x + xi, z + zi);

                // Decrease the height by the specified decrement but ensure it doesn't go below zero
                float newHeight = Mathf.Max(currentHeight - decrement, 0);

                // Use terrain.set to update the height at the specified coordinates
                terrain.set(x + xi, z + zi, newHeight);
            }
        }
    }
}