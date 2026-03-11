using System.Linq;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public abstract class EntityMonoBehaviourAction : EntityAction
    {
        protected EntityMonoBehaviour EntityMonoBehaviour { get; private set; }

        protected sealed override void OnFirstSetupComponent(object[] setupData = null)
        {
            EntityMonoBehaviour = setupData?
                .OfType<EntityMonoBehaviour>()
                .FirstOrDefault();

            OnFirstSetupEntityMonoBehaviourAction(setupData);
        }

        protected virtual void OnFirstSetupEntityMonoBehaviourAction(object[] setupData = null)
        {
        }

        protected TComponent[] GetComponentsInChildren<TComponent>(bool includeInactive)
            where TComponent : UnityEngine.MonoBehaviour
        {
            if (EntityMonoBehaviour == null)
            {
                return System.Array.Empty<TComponent>();
            }

            return EntityMonoBehaviour.GetComponentsInChildren<TComponent>(includeInactive);
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnDrawGizmos()
        {
        }

        internal void InvokeAwake()
        {
            if (Active)
            {
                Awake();
            }
        }

        internal void InvokeStart()
        {
            if (Active)
            {
                Start();
            }
        }

        internal void InvokeUpdate()
        {
            if (Active)
            {
                Update();
            }
        }

        internal void InvokeFixedUpdate()
        {
            if (Active)
            {
                FixedUpdate();
            }
        }

        internal void InvokeLateUpdate()
        {
            if (Active)
            {
                LateUpdate();
            }
        }

        internal void InvokeOnEnable()
        {
            if (Active)
            {
                OnEnable();
            }
        }

        internal void InvokeOnDisable()
        {
            if (Active)
            {
                OnDisable();
            }
        }

        internal void InvokeOnDestroy()
        {
            if (Active)
            {
                OnDestroy();
            }
        }

        internal void InvokeOnDrawGizmos()
        {
            if (Active && ShowGizmo)
            {
                OnDrawGizmos();
            }
        }
    }
}
