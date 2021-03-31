using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeControl : MonoBehaviour
{
    //public enum BiomeType
    //{
    //    Blank,
    //    Grassland,
    //    Forest,
    //    Field,
    //    Town,
    //    Mountain,
    //};

    public RegionBiome.BiomeType brushType = RegionBiome.BiomeType.Grassland;


    public Material grassMaterial = null;
    public Material fieldMaterial = null;
}
