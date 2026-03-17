using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxSpeed;
    }
    public ParallaxLayer[] layers;
    private Transform cam;
    private Vector3 lastCamPos;
    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }
    void Update()
    {
        Vector3 camDelta = cam.position - lastCamPos;
        foreach(ParallaxLayer layer in layers)
        {
            if(layer.layerTransform != null)
            {
                layer.layerTransform.position += new Vector3(
                    camDelta.x * layer.parallaxSpeed,
                    camDelta.y * layer.parallaxSpeed,
                    0
                );
            }
        }
        lastCamPos = cam.position;
    }
}