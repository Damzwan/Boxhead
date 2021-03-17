using System;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "ItemObject", menuName = "ItemObject", order = 0)]
    public class ItemObject : ScriptableObject
    {
        public String name;
        public String description;
    }
}