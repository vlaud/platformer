using System;
using System.Linq;
using UnityEngine;

public static class GetTypeofControlable
{
    public static Type GetType(Transform transform)
    {
        // Get all types derived from RagdollAction
        var types = typeof(Controlable).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Controlable)));

        // Check if transform's type is derived from RagdollAction and return the type if it is
        foreach (var type in types)
        {
            if (transform.GetComponent(type) != null)
            {
                return type;
            }
        }

        // Return null if no derived type is found
        return null;
    }
}
