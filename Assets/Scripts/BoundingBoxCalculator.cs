using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoAR
{
    public static class BoundingBoxCalculator
    {
        public static Bounds CalculateBoundingBox(GameObject obj)
        {
            // Get all the mesh renderers in the object and its children
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            // If there are no mesh renderers, return an empty bounds
            if (renderers.Length == 0)
            {
                return new Bounds();
            }

            // Initialize the bounds to the first renderer's bounds
            Bounds bounds = renderers[0].bounds;

            // Expand the bounds to include all the other renderers' bounds
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // Return the final bounds
            return bounds;
        }
    }
}
