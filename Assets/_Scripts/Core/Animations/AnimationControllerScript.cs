namespace _Scripts.Core.Animations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;

    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class AnimationControllerScript : MonoBehaviour
    {
        public bool IsUninterruptedPlaying { get; private set; }
        public bool IsAnimationLocked { get; private set; }
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        private int _currentAnimation;
        
        // Animations
        public readonly int Idle = Animator.StringToHash("idle");
        public readonly int Walk = Animator.StringToHash("walk");
        public readonly int Run = Animator.StringToHash("run");
        
        public readonly int Attack1 = Animator.StringToHash("attack1");
        public readonly int Attack2 = Animator.StringToHash("attack2");
        public readonly int WalkAttack = Animator.StringToHash("walk_attack");

        [Inject] private IDebug _debug;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            IsUninterruptedPlaying = false;
        }

        public void PlayAnimation(int newAnimation)
        {
            if (_currentAnimation == newAnimation)
                return;

            _currentAnimation = newAnimation;
            _animator.Play(newAnimation, 0, 0);
        }

        public void PlayAnimationUninterrupted(int newAnimation, Action onComplete = null, bool lockAnimation = false)
        {
            if (IsUninterruptedPlaying) 
                return;

            IsAnimationLocked = lockAnimation;
                
            PlayAnimation(newAnimation);
            StartCoroutine(WaitForAnimationFinish(onComplete));
        }

        public void TurnLeft()
        {
            _spriteRenderer.flipX = true;
        }
        
        public void TurnRight()
        {
            _spriteRenderer.flipX = false;
        }

        private IEnumerator WaitForAnimationToSet()
        {
            while (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _currentAnimation)
            {
                yield return null;
            }
        }

        private IEnumerator WaitForAnimationFinish(Action onComplete = null)
        {
            IsUninterruptedPlaying = true;

            yield return WaitForAnimationToSet();
            
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            onComplete?.Invoke();
            IsUninterruptedPlaying = false;
            IsAnimationLocked = false;
        }
    }
}
