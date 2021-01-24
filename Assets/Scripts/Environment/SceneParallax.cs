using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParallax : MonoBehaviour
{
    public ParallaxNode[] BackgroundLayers;
    private Transform CameraTransform;

    private void Start()
    {
        CameraTransform = BaseGameOverseer.Instance.MainGameCamera.transform;
        for (int i = 0; i < BackgroundLayers.Length; ++i)
        {
            BackgroundLayers[i].SpriteLength = BackgroundLayers[i].SpriteRenderer.bounds.size.x;
            BackgroundLayers[i].StartPosition = BackgroundLayers[i].SpriteRenderer.transform.position.x;
        }
    }


    private void LateUpdate()
    {
        Vector3 CameraPosition = BaseGameOverseer.Instance.MainGameCamera.transform.position;

        foreach (ParallaxNode Layer in BackgroundLayers)
        {
            float temp = (CameraTransform.position.x * (1 - Layer.ParallaxEffectAmount));
            float dist = (CameraTransform.position.x * Layer.ParallaxEffectAmount);

            Layer.SpriteRenderer.transform.position = new Vector3(Layer.StartPosition + dist, Layer.SpriteRenderer.transform.position.y, Layer.SpriteRenderer.transform.position.z);
            if (temp > Layer.StartPosition + Layer.SpriteLength)
            {
                Layer.StartPosition += Layer.SpriteLength;
            }
            else if (temp < Layer.StartPosition - Layer.SpriteLength)
            {
                Layer.StartPosition -= Layer.SpriteLength;
            }
        }
    }

    [System.Serializable]
    public class ParallaxNode
    {
        public SpriteRenderer SpriteRenderer;
        public float ParallaxEffectAmount;
        [System.NonSerialized]
        public float SpriteLength;
        [System.NonSerialized]
        public float StartPosition;
    }
}
