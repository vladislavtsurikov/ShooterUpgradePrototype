using System.Linq;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.StateMachine.Runtime.Data;
using VladislavTsurikov.StateMachine.Runtime.Definitions;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.StateMachine.Runtime.Actions
{
    [Name("StateMachine/StateMachineAction")]
    public sealed class StateMachineAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions = new CompositeDisposable();
        private StateMachineData _data;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _data = Get<StateMachineData>();

            if (_data.CurrentState.Value == null)
            {
                EvaluateConditions();
            }

            _data.EligibleStates.ObserveAdd().Select(_ => Unit.Default)
                .Merge(_data.EligibleStates.ObserveRemove().Select(_ => Unit.Default))
                .Merge(_data.EligibleStates.ObserveReset())
                .Subscribe(_ => EvaluateConditions())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
            if (_data != null)
            {
                for (int i = 0; i < _data.StateStack.ElementList.Count; i++)
                {
                    var state = _data.StateStack.ElementList[i];
                    if (state == null)
                    {
                        continue;
                    }

                    state.IsEligibleForTransition.Value = false;
                }

                _data = null;
            }
        }

        public void EvaluateConditions()
        {
            var data = Get<StateMachineData>();
            if (data == null)
            {
                return;
            }

            var best = data.EligibleStates
                .Where(state => state != null && !ReferenceEquals(state, data.CurrentState.Value))
                .OrderByDescending(state => state.Priority)
                .FirstOrDefault();

            TrySwitchState(best);
        }

        private bool TrySwitchState(State nextState)
        {
            if (nextState == null)
            {
                return false;
            }

            var data = Get<StateMachineData>();

            var current = data.CurrentState.Value;
            if (current != null && !current.CanExit(EntityMonoBehaviour, nextState))
            {
                return false;
            }

            if (!nextState.CanEnter(EntityMonoBehaviour, current))
            {
                return false;
            }

            current?.Exit(EntityMonoBehaviour);
            data.PreviousState = current;
            data.SetState(nextState);
            nextState.Enter(EntityMonoBehaviour);
            return true;
        }
    }
}
