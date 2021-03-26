using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeControl : MonoBehaviour
{
    public enum BiomeType
    {
        Grassland,
        Forest,
        Field,
        Town,
        Mountain,
        Water
    };

    public BiomeType brushType = BiomeType.Grassland;


    public Material grassMaterial = null;
    public Material fieldMaterial = null;
}
