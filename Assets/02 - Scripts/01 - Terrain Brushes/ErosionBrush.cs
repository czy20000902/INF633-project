using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionBrush : TerrainBrush {

    public float erosionRate = 0.1f;  // Adjust this rate as needed

    public int maxRaindrops = 10;  // Maximum number of raindrops in one brush stroke

    public override void draw(int x, int z) {
        for (int raindrop = 0; raindrop < maxRaindrops; raindrop++) {
            int randomX = x + Random.Range(-radius, radius + 1);
            int randomZ = z + Random.Range(-radius, radius + 1);

            float currentHeight = terrain.get(randomX, randomZ);
            float newHeight = currentHeight - erosionRate;

            // Make sure the newHeight doesn't go below a certain threshold (e.g., 0)
            newHeight = Mathf.Max(0, newHeight);

            terrain.set(randomX, randomZ, newHeight);
        }
    }
}
