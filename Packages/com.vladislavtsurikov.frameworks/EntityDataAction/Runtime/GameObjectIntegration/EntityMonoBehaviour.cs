using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    [ExecuteInEditMode]
    public partial class EntityMonoBehaviour : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Entity _entity;

        public Entity Entity
        {
            get
            {
                _entity ??= new Entity();

                return _entity;
            }
        }

        public EntityDataCollection Data => Entity.Data;
        public EntityActionCollection Actions => Entity.Actions;

        public bool IsSetup => Entity.IsSetup;
        public DirtyActionRunner DirtyRunner => Entity.DirtyRunner;

        public bool LocalActive
        {
            get => Entity.LocalActive;
            set => Entity.LocalActive = value;
        }

        public bool IsEntityActive => Active;

        internal bool Active
        {
            get
            {
#if UNITY_EDITOR
                if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                {
                    return false;
                }
#endif

                return Entity.Active;
            }
        }

        protected virtual void BeforeOnSetupEntity()
        {
        }

        protected virtual Type[] ComponentDataTypesToCreate() => null;

        protected virtual Type[] ActionTypesToCreate() => null;

        protected virtual void OnAfterCreateDataAndActions()
        {
        }

        public T GetData<T>() where T : ComponentData => Entity.GetData<T>();

        public T GetAction<T>() where T : EntityAction => Entity.GetAction<T>();

        public void Setup() => Entity.Setup();

        private void SetupEntityBindings()
        {
            Entity.ComponentDataTypesProvider = ComponentDataTypesToCreate;
            Entity.ActionTypesProvider = ActionTypesToCreate;
            Entity.AfterCreateDataAndActionsCallback = _ => OnAfterCreateDataAndActions();
            Entity.BeforeOnSetupEntity = _ => BeforeOnSetupEntity();
        }
    }
}
