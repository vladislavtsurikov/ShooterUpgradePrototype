using System;
using System.Collections.Generic;
using OdinSerializer;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.StateMachine.Runtime.Definitions;

namespace VladislavTsurikov.StateMachine.Runtime.Data
{
    [Name("StateMachine/StateMachineData")]
    public sealed class StateMachineData : EntityComponentData
    {
        [OdinSerialize]
        private NodeStackOnlyDifferentTypes<State> _stateStack = new NodeStackOnlyDifferentTypes<State>();

        [OdinSerialize, HideInInspector]
        private ReactiveProperty<State> _currentState = new();

        [OdinSerialize, HideInInspector]
        private State _previousState;

        [NonSerialized]
        private ReactiveCollection<State> _eligibleStates = new ReactiveCollection<State>();

        public ReactiveProperty<State> CurrentState
        {
            get
            {
                _currentState ??= new ReactiveProperty<State>();
                return _currentState;
            }
        }

        public void SetState(State value)
        {
            _currentState ??= new ReactiveProperty<State>();
            if (_currentState.Value == value)
            {
                return;
            }

            _previousState = _currentState.Value;

            _currentState.Value = value;
        }

        public State PreviousState
        {
            get => _previousState;
            set
            {
                if (_previousState == value)
                {
                return;
            }

            _previousState = value;
        }
        }

        public NodeStackOnlyDifferentTypes<State> StateStack => _stateStack;

        public IReadOnlyReactiveCollection<State> EligibleStates
        {
            get
            {
                _eligibleStates ??= new ReactiveCollection<State>();
                return _eligibleStates;
            }
        }

        protected override void SetupComponent(object[] setupData = null)
        {
            _stateStack.Setup(true, new object[]{setupData[0], this});
        }

        protected override void OnDisableElement()
        {
            _stateStack.OnDisable();
        }

        internal void SetStateEligible(State state, bool eligible)
        {
            if (state == null)
            {
                return;
            }

            if (eligible)
            {
                if (!_eligibleStates.Contains(state))
                {
                    _eligibleStates.Add(state);
                }
            }
            else
            {
                _eligibleStates.Remove(state);
            }
        }
    }
}
