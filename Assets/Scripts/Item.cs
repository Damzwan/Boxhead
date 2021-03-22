using System;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item", order = 0)]
    public class Item : ScriptableObject
    {
        public String title;
        public String description;
    }
}