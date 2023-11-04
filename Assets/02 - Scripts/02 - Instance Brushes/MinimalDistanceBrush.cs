using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimalDistanceBrush : InstanceBrush
{
    public float mindistance = 10f;


    public bool checkMinDistance(float x,float z)
    {
        float distance = float.MaxValue;
        var n = terrain.getObjectCount();
        for (int i = 0; i < n; i++)
        {
            Vector3 objLoc = terrain.getObjectLoc(i);
            // distance to nearest instances
            distance = Mathf.Min(distance, Mathf.Sqrt((objLoc.x - x) * (objLoc.x - x) + (objLoc.z - z) * (objLoc.z - z)));
        }

       

        if (distance < mindistance)
        {
            print(distance+ "you can't place the object");
            return false;
        }
        else {
            print(distance+"OK");
            return true; }
    }
    public override void draw(float x, float z)
    {

        if (checkMinDistance(x, z)) spawnObject(x, z);
        
    }
}

