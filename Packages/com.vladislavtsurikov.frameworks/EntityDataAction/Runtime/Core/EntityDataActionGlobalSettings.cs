using System;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    [LocationAsset("EntityDataAction", false)]
    public sealed class EntityDataActionGlobalSettings : SerializedScriptableObjectSingleton<EntityDataActionGlobalSettings>
    {
        [OdinSerialize]
        public bool EditModeActive;

        public static bool Active
        {
            get
            {
                if (Application.isPlaying)
                {
                    return true;
                }

                return Instance.EditModeActive;
            }
            set
            {
                if (Application.isPlaying)
                {
                    return;
                }

                if (Active == value)
                {
                    return;
                }

                Instance.EditModeActive = value;
                ActiveChanged?.Invoke();
            }
        }

        public static event Action ActiveChanged;
    }
}
