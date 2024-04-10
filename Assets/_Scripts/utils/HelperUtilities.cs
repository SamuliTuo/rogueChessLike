using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public static class HelperUtilities
{
    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public static Tuple<CompassDir, Vector3> DetermineCompassDir(Vector3 dir)
    {
        var dirNormalized = dir.normalized;
        float xDot = Vector3.Dot(Vector3.right, dirNormalized);
        float yDot = Vector3.Dot(Vector3.forward, dirNormalized);

        // Get the 3 directions of the quadrant the input is in
        Dictionary<CompassDir, Vector3> dirs = new Dictionary<CompassDir, Vector3>();
        if (xDot > 0)
        {
            if (yDot > 0)
            {
                dirs.Add(CompassDir.NORTH, Vector3.forward);
                dirs.Add(CompassDir.NORTH_EAST, new Vector3(0.7171068f, 0, 0.7171068f));
                dirs.Add(CompassDir.EAST, Vector3.right);
            }
            else
            {
                dirs.Add(CompassDir.EAST, Vector3.right);
                dirs.Add(CompassDir.SOUTH_EAST, new Vector3(0.7171068f, 0, -0.7171068f));
                dirs.Add(CompassDir.SOUTH, Vector3.back);
            }
        }
        else
        {
            if (yDot <= 0)
            {
                dirs.Add(CompassDir.SOUTH, Vector3.back);
                dirs.Add(CompassDir.SOUTH_WEST, new Vector3(-0.7171068f, 0, -0.7171068f));
                dirs.Add(CompassDir.WEST, Vector3.left);
            }
            else
            {
                dirs.Add(CompassDir.WEST, Vector3.left);
                dirs.Add(CompassDir.NORTH_WEST, new Vector3(-0.7171068f, 0, 0.7171068f));
                dirs.Add(CompassDir.NORTH, Vector3.forward);
            }
        }

        // Check which direction in the quadrant is closest
        KeyValuePair<CompassDir, Vector3> finalDir = new KeyValuePair<CompassDir, Vector3>();
        float distance = 1000000;
        foreach (KeyValuePair<CompassDir, Vector3> compassDirection in dirs)
        {
            float dist2 = (dirNormalized - compassDirection.Value).magnitude;
            if (dist2 < distance)
            {
                distance = dist2;
                finalDir = compassDirection;
            }
        }
        return new Tuple<CompassDir, Vector3>(finalDir.Key, finalDir.Value);
    }


    public static T Clone<T>(this T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (UnityEngine.Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
