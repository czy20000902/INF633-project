using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianBrush : TerrainBrush
{
    public float strength = 10; // Adjust the strength of the effect
    public int radius = 10; // Define the brush radius

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                float distanceSqr = xi * xi + zi * zi;
                float effect = Mathf.Exp(-distanceSqr / (2 * strength * strength));

                // Use terrain.get to retrieve the current height at the specified coordinates
                float currentHeight = terrain.get(x + xi, z + zi);

                // Calculate the new height using the Gaussian distribution and adjust strength
                float newHeight = currentHeight + effect;

                // Use terrain.set to update the height at the specified coordinates
                terrain.set(x + xi, z + zi, newHeight);
            }
        }
    }
}