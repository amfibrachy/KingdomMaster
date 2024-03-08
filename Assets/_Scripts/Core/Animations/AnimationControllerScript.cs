namespace _Scripts.Core.Animations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;

    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class AnimationControllerScript : MonoBehaviour
    {
        public bool IsUninterruptedPlaying { get; private set; }
        public bool IsAnimationLocked { get; private set; }

        public bool IsFacingRight => _spriteRenderer.flipX == false;
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        private int _currentAnimation;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        /* ANIMATIONS */
        
        // Effects Animations
        public readonly int Dust = Animator.StringToHash("dust");
        
        // General Animations
        public readonly int Idle = Animator.StringToHash("idle");
        public readonly int Walk = Animator.StringToHash("walk");
        public readonly int Run = Animator.StringToHash("run");
        public readonly int Death = Animator.StringToHash("death");
        
        // Player Specific Animations
        public readonly int PickaxeAttack = Animator.StringToHash("pickaxe_attack");
        public readonly int IdleAttack = Animator.StringToHash("idle_attack");
        public readonly int WalkAttack = Animator.StringToHash("walk_attack");
        public readonly int ReverseWalkAttack = Animator.StringToHash("reverse_walk_attack");
        public readonly int ReverseWalk = Animator.StringToHash("reverse_walk");
        
        // Builder Specific Animations
        public readonly int Build = Animator.StringToHash("build");
        
        // Lumberjack Specific Animations
        public readonly int ChopTree = Animator.StringToHash("chop_tree");

        [Inject] private IDebug _debug;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _cancellationTokenSource = new CancellationTokenSource();
            IsUninterruptedPlaying = false;
        }

        public void PlayAnimation(int newAnimation, float time = 0)
        {
            if (_currentAnimation == newAnimation)
                return;

            float normalizedTime = time > 0 ? time : 0;

            _currentAnimation = newAnimation;
            _animator.Play(newAnimation, 0, normalizedTime);
        }
        
        public void TurnLeft()
        {
            _spriteRenderer.flipX = true;
        }
        
        public void TurnRight()
        {
            _spriteRenderer.flipX = false;
        }

        public void PlayAnimationUninterrupted(int newAnimation, Action onComplete = null, bool lockAnimation = false, bool forceInterrupt = false)
        {
            if (IsUninterruptedPlaying && !forceInterrupt) 
                return;

            IsAnimationLocked = lockAnimation;

            float time = 0;

            if (forceInterrupt)
            {
                _cancellationTokenSource?.Cancel();
                time = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }

            PlayAnimation(newAnimation, time);
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            WaitForAnimationFinish(onComplete, _cancellationTokenSource.Token).Forget(); // Forget() to ignore any exception
        }

        public async UniTask WaitForAnimationFinish(Action onComplete = null, CancellationToken cancellationToken = default)
        {
            IsUninterruptedPlaying = true;

            await WaitForAnimationToSet();

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            onComplete?.Invoke();
            IsUninterruptedPlaying = false;
            IsAnimationLocked = false;
        }
        
        private async UniTask WaitForAnimationToSet()
        {
            while (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _currentAnimation)
            {
                await UniTask.Yield();
            }
        }
    }
}
