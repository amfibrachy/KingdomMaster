namespace _Scripts.Core.Animations
{
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class AnimationControllerScript : MonoBehaviour
    {
        private Animator _animator;
        private int _currentStateHash;

        private readonly Dictionary<string, int> _stateHashes = new Dictionary<string, int>();

        // Animations
        public readonly string Idle = "idle";
        public readonly string Walk = "walk";
        public readonly string Run = "run";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetState(string newState)
        {
            if (!_stateHashes.ContainsKey(newState))
            {
                int hash = Animator.StringToHash(newState);
                _stateHashes.Add(newState, hash);
            }

            int newStateHash = _stateHashes[newState];
            
            if (_currentStateHash == newStateHash)
                return;

            _animator.Play(newStateHash);
            _currentStateHash = newStateHash;
        }

        public float GetCurrentStateDuration()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }
}
