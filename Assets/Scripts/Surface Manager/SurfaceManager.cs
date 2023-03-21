using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    private static SurfaceManager _instance;
    public static SurfaceManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one SurfaceManager active in the scene! Destroying latest one: " + name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField]
    private List<SurfaceType> Surfaces = new List<SurfaceType>();
    [SerializeField]
    private int DefaultPoolSizes = 10;
    [SerializeField]
    private Surface DefaultSurface;

    //public void HandleImpact(GameObject HitObject, Vector3 HitPoint, Vector3 HitNormal, ImpactType Impact, int TriangleIndex)
    //{
    //    if (HitObject.TryGetComponent<Terrain>(out Terrain terrain))
    //    {
    //        List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, HitPoint);
    //        foreach (TextureAlpha activeTexture in activeTextures)
    //        {
    //            SurfaceType surfaceType = Surfaces.Find(surface => surface.Albedo == activeTexture.Texture);
    //            if (surfaceType != null)
    //            {
    //                foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
    //                {
    //                    if (typeEffect.ImpactType == Impact)
    //                    {
    //                        PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, activeTexture.Alpha);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                foreach (Surface.SurfaceImpactTypeEffect typeEffect in DefaultSurface.ImpactTypeEffects)
    //                {
    //                    if (typeEffect.ImpactType == Impact)
    //                    {
    //                        PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    else if (HitObject.TryGetComponent<Renderer>(out Renderer renderer))
    //    {
    //        Texture activeTexture = GetActiveTextureFromRenderer(renderer, TriangleIndex);

    //        SurfaceType surfaceType = Surfaces.Find(surface => surface.Albedo == activeTexture);
    //        if (surfaceType != null)
    //        {
    //            foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
    //            {
    //                if (typeEffect.ImpactType == Impact)
    //                {
    //                    PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            foreach (Surface.SurfaceImpactTypeEffect typeEffect in DefaultSurface.ImpactTypeEffects)
    //            {
    //                if (typeEffect.ImpactType == Impact)
    //                {
    //                    PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
    //                }
    //            }
    //        }
    //    }
    //}
}
