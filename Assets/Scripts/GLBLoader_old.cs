using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoAR
{
    public class GLBLoader_old : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("This can be a local or external resource uri.")]
        private string uri = string.Empty;

        private async void Start()
        {
            Response response = new Response();

            try
            {
                response = await Rest.GetAsync(uri, readResponseData: true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            if (!response.Successful)
            {
                Debug.LogError($"Failed to get glb model from {uri}");
                return;
            }

            var gltfObject = GltfUtility.GetGltfObjectFromGlb(response.ResponseData);

            try
            {
                await gltfObject.ConstructAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return;
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}