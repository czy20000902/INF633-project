using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBrush : TerrainBrush
{
    public float[,] shapePattern; // 2D array defining the custom shape pattern
    public float strength = 1.0f; // Adjust the strength of the effect
    public int radius;            // Define the brush radius

    public override void draw(int x, int z)
    {
        int patternSize = shapePattern.GetLength(0); // Get the size of the shape pattern

        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                // Calculate the index within the shape pattern
                int patternX = (int)Mathf.Lerp(0, patternSize - 1, Mathf.InverseLerp(-radius, radius, xi));
                int patternZ = (int)Mathf.Lerp(0, patternSize - 1, Mathf.InverseLerp(-radius, radius, zi));

                // Use terrain.get to retrieve the current height at the specified coordinates
                float currentHeight = terrain.get(x + xi, z + zi);

                // Get the value from the shape pattern and apply it with the strength
                float shapeValue = shapePattern[patternX, patternZ] * strength;

                // Apply the shape value as a height adjustment
                float newHeight = currentHeight + shapeValue;

                // Use terrain.set to update the height at the specified coordinates
                terrain.set(x + xi, z + zi, newHeight);
            }
        }
    }
}