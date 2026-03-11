using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Modifier
{
    [CreateAssetMenu(menuName = "Modifier/Color", fileName = "ColorModifier")]
    public sealed class ColorModifier : Modifier
    {
        [SerializeField]
        private Color _color = Color.white;

        public Color Color => _color;
    }
}
