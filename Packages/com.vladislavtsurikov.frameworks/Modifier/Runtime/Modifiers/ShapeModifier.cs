using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Modifier
{
    [CreateAssetMenu(menuName = "Modifier/Shape", fileName = "ShapeModifier")]
    public sealed class ShapeModifier : Modifier
    {
        [SerializeField]
        private GameObject _prefab;

        public GameObject Prefab => _prefab;
    }
}
