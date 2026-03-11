using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public class Entity
    {
        [OdinSerialize]
        private EntityActionCollection _actions;

        [OdinSerialize]
        private EntityDataCollection _data;

        [OdinSerialize]
        private bool _localActive = true;

        internal Func<Type[]> ActionTypesProvider;
        internal Action<Entity> AfterCreateDataAndActionsCallback;

        internal Func<Type[]> ComponentDataTypesProvider;

        internal DirtyActionRunner DirtyRunner;
        internal Action<Entity> BeforeOnSetupEntity;

        public EntityDataCollection Data => _data;
        public EntityActionCollection Actions => _actions;
        public object[] SetupData { get; private set; }

        public bool IsSetup { get; private set; }

        public bool LocalActive
        {
            get => _localActive;
            set
            {
                if (_localActive == value)
                {
                    return;
                }

                _localActive = value;
                HandleActiveChanged();
            }
        }

        public bool Active => _localActive && EntityDataActionGlobalSettings.Active;

        public void SetSetupData(object[] setupData) => SetupData = setupData;

        public void Setup(bool force = false)
        {
            if (!force)
            {
                if (IsSetup)
                {
                    return;
                }
            }

            EntityDataActionGlobalSettings.ActiveChanged -= HandleActiveChanged;
            EntityDataActionGlobalSettings.ActiveChanged += HandleActiveChanged;

            _data ??= new EntityDataCollection();
            _data.Entity = this;
            _actions ??= new EntityActionCollection();
            _actions.Entity = this;

            DirtyRunner ??= new DirtyActionRunner(this, _data, _actions);
            DirtyRunner.Setup();

            if (BeforeOnSetupEntity != null)
            {
                BeforeOnSetupEntity(this);
            }

            if (Active)
            {
                _data.Setup(true, SetupData);
                _actions.Setup(true, SetupData);
            }

            CreateDefaultData();
            CreateDefaultActions();

            AfterCreateDataAndActionsCallback?.Invoke(this);

            _data.ElementAdded += HandleDataChanged;
            _data.ElementRemoved += HandleDataChanged;

            if (Active)
            {
                _actions.Run().Forget();
            }

            IsSetup = true;
        }

        protected virtual Type[] ComponentDataTypesToCreate() => null;

        protected virtual Type[] ActionTypesToCreate() => null;

        public T GetData<T>() where T : ComponentData => Data.GetElement<T>();

        public T GetAction<T>() where T : EntityAction => Actions.GetElement<T>();

        internal void HandleDataChanged(int index) => DirtyRunner?.TriggerAll();

        private void CreateDefaultData()
        {
            Type[] types = ComponentDataTypesProvider != null
                ? ComponentDataTypesProvider()
                : ComponentDataTypesToCreate();

            if (types == null)
            {
                return;
            }

            _data.SyncToTypesIfNeeded(types);
        }

        private void CreateDefaultActions()
        {
            if (_actions == null)
            {
                return;
            }

            Type[] types = ActionTypesProvider != null
                ? ActionTypesProvider()
                : ActionTypesToCreate();

            if (types == null)
            {
                return;
            }

            _actions.SyncToTypesIfNeeded(types);
        }

        public void Disable()
        {
            EntityDataActionGlobalSettings.ActiveChanged -= HandleActiveChanged;
            _data.ElementAdded -= HandleDataChanged;
            _data.ElementRemoved -= HandleDataChanged;

            _data.OnDisable();
            _actions.OnDisable();

            DirtyRunner?.OnDisable();

            IsSetup = false;
        }

        private void HandleActiveChanged()
        {
            if (_localActive && EntityDataActionGlobalSettings.Active)
            {
                Disable();
                Setup();
            }
            else
            {
                Disable();
            }
        }
    }
}
