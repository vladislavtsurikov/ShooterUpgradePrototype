using System;
using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.StateMachine.Runtime.Data;

namespace VladislavTsurikov.StateMachine.Runtime.Definitions
{
    [Serializable]
    public class State : Node
    {
        [OdinSerialize]
        private string _stateId;
        [OdinSerialize]
        private int _priority;

        [NonSerialized]
        private StateMachineData _owner;

        [OdinSerialize, HideInInspector]
        private ReactiveProperty<bool> _isEligibleForTransition = new ReactiveProperty<bool>();

        [NonSerialized]
        private CompositeDisposable _subscriptions = new CompositeDisposable();

        public EntityMonoBehaviour EntityMonoBehaviour { get; private set; }

        public string StateId
        {
            get => _stateId;
            set
            {
                if (_stateId == value)
                {
                    return;
                }

                _stateId = value;
            }
        }

        public int Priority
        {
            get => _priority;
            set
            {
                if (_priority == value)
                {
                    return;
                }

                _priority = value;
            }
        }

        public ReactiveProperty<bool> IsEligibleForTransition
        {
            get
            {
                _isEligibleForTransition ??= new ReactiveProperty<bool>();
                return _isEligibleForTransition;
            }
            set => _isEligibleForTransition = value;
        }

        protected override void SetupComponent(object[] setupData = null)
        {
            _subscriptions = new CompositeDisposable();

            EntityMonoBehaviour = (EntityMonoBehaviour)setupData[0];
            _owner = (StateMachineData)setupData[1];

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    bool eligible = Conditional();
                    if (IsEligibleForTransition.Value != eligible)
                    {
                        IsEligibleForTransition.Value = eligible;
                        _owner?.SetStateEligible(this, eligible);
                    }
                })
                .AddTo(_subscriptions);

            SetupComponentState(setupData);
        }

        protected virtual void SetupComponentState(object[] setupData = null)
        {

        }

        protected override void OnDisableElement()
        {
            _subscriptions?.Clear();
        }

        protected T GetData<T>() where T : ComponentData
        {
            return EntityMonoBehaviour?.GetData<T>();
        }

        protected T GetAction<T>() where T : EntityAction
        {
            return EntityMonoBehaviour?.GetAction<T>();
        }

        public virtual bool CanEnter(EntityMonoBehaviour entity, State fromState) => true;
        public virtual bool CanExit(EntityMonoBehaviour entity, State toState) => true;
        public virtual void Enter(EntityMonoBehaviour entity) { }
        public virtual void Exit(EntityMonoBehaviour entity) { }
        public virtual void Update(EntityMonoBehaviour entity, float deltaTime) { }
        protected virtual bool Conditional() => false;
    }
}
