using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HeightBaseBrush : InstanceBrush
{
    public float max_height = 20f;
    public float max_angle = 30f;

    public override void draw(float x, float z)
    {
        // altitude
        float height = terrain.get(x, z);
        // steepness
        if (terrain.getSteepness(x, z) > max_angle)
            return;
        if (height <= max_height / 2)
            terrain.spawnObject(terrain.getInterp3(x, z), 1.0f, 0);
        else if (height <= max_height)
            terrain.spawnObject(terrain.getInterp3(x, z), 1.0f, 1);

    }
}

