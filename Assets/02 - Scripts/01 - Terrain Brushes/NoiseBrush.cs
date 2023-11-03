using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseBrush : TerrainBrush
{
    public float noiseScale = 0.1f; // Adjust the noise scale
    public float strength = 1.0f;   // Adjust the strength of the noise effect
    public int radius = 20;         // Define the brush radius

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                // Use terrain.get to retrieve the current height at the specified coordinates
                float currentHeight = terrain.get(x + xi, z + zi);

                // Calculate Perlin Noise at this position and scale it
                float perlinValue = Mathf.PerlinNoise((x + xi) * noiseScale, (z + zi) * noiseScale);

                // Apply the Perlin Noise as a height adjustment
                float newHeight = currentHeight + perlinValue * strength;

                // Use terrain.set to update the height at the specified coordinates
                terrain.set(x + xi, z + zi, newHeight);
            }
        }
    }
}