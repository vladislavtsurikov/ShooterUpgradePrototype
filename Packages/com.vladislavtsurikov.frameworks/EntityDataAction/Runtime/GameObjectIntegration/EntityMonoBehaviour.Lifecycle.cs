using System;
using UnityEngine;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public partial class EntityMonoBehaviour
    {
        private void Awake()
        {
            SetupEntityBindings();
            Entity.SetSetupData(new object[] { this });
            Entity.Setup();
            ForEachLifecycleAction(action => action.InvokeAwake());
        }

        private void Start()
        {
            ForEachLifecycleAction(action => action.InvokeStart());
        }

        protected void OnEnable()
        {
            if (!Application.isPlaying)
            {
                SetupEntityBindings();
                Entity.SetSetupData(new object[] { this });
                Entity.Setup(true);
            }

            ForEachLifecycleAction(action => action.InvokeOnEnable());
        }

        protected void OnDisable()
        {
            Entity.Disable();
        }

        protected void OnDestroy() => ForEachLifecycleAction(action => action.InvokeOnDestroy());

        protected void Update() => ForEachLifecycleAction(action => action.InvokeUpdate());

        protected void FixedUpdate() => ForEachLifecycleAction(action => action.InvokeFixedUpdate());

        protected void LateUpdate() => ForEachLifecycleAction(action => action.InvokeLateUpdate());

        protected void OnDrawGizmos() => ForEachLifecycleAction(action => action.InvokeOnDrawGizmos());

        private void ForEachLifecycleAction(Action<EntityMonoBehaviourAction> handler)
        {
            if (!Active || !Entity.IsSetup)
            {
                return;
            }

            EntityActionCollection actions = Entity.Actions;

            foreach (Action action in actions.ElementList)
            {
                if (action is EntityMonoBehaviourAction entityLifecycleAction)
                {
                    handler(entityLifecycleAction);
                }
            }
        }
    }
}
