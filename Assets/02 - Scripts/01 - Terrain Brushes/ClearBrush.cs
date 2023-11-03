using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBrush : TerrainBrush
{
    public float clearHeight = 0; // Set the desired height to clear to
    public int radius = 20; // Define the brush radius

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                // Use terrain.set to clear the height at the specified coordinates
                terrain.set(x + xi, z + zi, clearHeight);
            }
        }
    }
}