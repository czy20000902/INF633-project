using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGridBrush : InstanceBrush
{

    public int grid_spacing = 10;
    public override void draw(float x, float z)
    {
        for (int zi = -radius; zi <= radius; zi += 1)
            for (int xi = -radius; xi <= radius; xi += 1)
            {
                int rx = (int)x + xi;
                int rz = (int)z + zi;
                if (rx % grid_spacing == 0 && rz % grid_spacing == 0)
                    spawnObject(rx, rz);
            }
    }
}