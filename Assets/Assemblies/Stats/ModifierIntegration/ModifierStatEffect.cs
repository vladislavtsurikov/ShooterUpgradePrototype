using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [CreateAssetMenu(menuName = "Stats/Effect With Modifier", fileName = "ModifierStatEffect")]
    public class ModifierStatEffect : StatEffect
    {
        [SerializeField]
        private Modifier.Modifier _modifier;

        public Modifier.Modifier Modifier => _modifier;
    }
}
