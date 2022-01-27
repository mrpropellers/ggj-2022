using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.GlobalConsts
{
    public static class Tags
    {
        public const string Ingredient = "Ingredient";
        public static HashSet<string> Items = new HashSet<string>()
        {
            Ingredient
        };
    }

    public static class SortingLayers
    {
        // TODO: Add "Decoration" layer between normal and foreground
        public static int Foreground = SortingLayer.NameToID("Foreground");
    }
}
