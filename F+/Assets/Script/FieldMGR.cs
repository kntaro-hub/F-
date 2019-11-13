using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FieldMGR : MonoBehaviour
{
    public static Vector3 GridToWorld(Point grid)
    {
        Vector3 world = new Vector3(
            (float)(grid.x * FieldInformation.GridSize),
            0.0f,
            (float)(grid.y * FieldInformation.GridSize));

        return world;
    }

    public static Point WorldToGrid(Vector3 world)
    {
        Point grid = new Point(
            (int)(world.x / FieldInformation.GridSize),
            (int)(world.z / FieldInformation.GridSize));

        return grid;
    }
}
