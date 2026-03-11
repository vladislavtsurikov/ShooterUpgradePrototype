using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Modifier
{
    [CreateAssetMenu(menuName = "Modifier/Size", fileName = "SizeModifier")]
    public sealed class SizeModifier : Modifier
    {
        [SerializeField]
        private Vector3 _scale = Vector3.one;

        public Vector3 Scale => _scale;
    }
}
