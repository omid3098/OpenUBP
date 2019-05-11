using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace OpenTransition
{
    public enum AnimationType
    {
        None,
        MoveTo,
        MoveFrom,
        RotateTo,
        RotateFrom,
        ScaleTo,
        ScaleFrom,
        ColorTo,
        ColorFrom,
        AlphaTo,
        AlphaFrom,
        Text
    }
    public class BaseTransition : MonoBehaviour
    {
        [SerializeField] AnimationType animationType = AnimationType.None;
        private bool AnimationIsNotNone() { return animationType != AnimationType.None; }
        [ShowIf("AnimationIsNotNone")] [SerializeField] bool autoPlay = true;
        [ShowIf("AnimationIsNotNone")] [SerializeField] float duration = 0.3f;
        [ShowIf("AnimationIsNotNone")] [SerializeField] float delay = 0f;
        [ShowIf("AnimationIsNotNone")] [SerializeField] Ease easeType = Ease.OutQuart;
        [MinValue(-1)] [ShowIf("AnimationIsNotNone")] [SerializeField] int loopCount = 1;
        private bool InfiniteLoop() { return (loopCount == -1 || loopCount > 1); }
        [ShowIf("InfiniteLoop")] [SerializeField] LoopType loopType = LoopType.Restart;
        [ShowIf("ShowRelative")] [SerializeField] bool relative = false;
        bool ShowRelative() { return (animationType == AnimationType.MoveFrom || animationType == AnimationType.MoveTo || animationType == AnimationType.RotateFrom || animationType == AnimationType.RotateTo || animationType == AnimationType.ScaleFrom || animationType == AnimationType.ScaleTo); }
        // Move
        private bool MoveSelected() { return (animationType == AnimationType.MoveTo || animationType == AnimationType.MoveFrom); }
        [ShowIf("MoveSelected")] [SerializeField] Vector3 targetPosition = Vector3.zero;

        // Rotate
        private bool RotateSelected() { return (animationType == AnimationType.RotateTo || animationType == AnimationType.RotateFrom); }
        [ShowIf("RotateSelected")] [SerializeField] Vector3 targetRotation = Vector3.zero;

        // Scale
        private bool ScaleSelected() { return (animationType == AnimationType.ScaleTo || animationType == AnimationType.ScaleFrom); }
        [ShowIf("ScaleSelected")] [SerializeField] Vector3 targetScale = Vector3.zero;

        // Color
        private bool ColorSelected() { return (animationType == AnimationType.ColorTo || animationType == AnimationType.ColorFrom); }
        private bool ImageSelected() { return ((animationType == AnimationType.ColorTo || animationType == AnimationType.ColorFrom) && (targetSpriteRenderer == null && targetMaterial == null)); }
        private bool SpriteRendererSelected() { return ((animationType == AnimationType.ColorTo || animationType == AnimationType.ColorFrom) && (targetImage == null && targetMaterial == null)); }
        private bool MaterialSelected() { return ((animationType == AnimationType.ColorTo || animationType == AnimationType.ColorFrom) && (targetImage == null && targetSpriteRenderer == null)); }
        [ShowIf("ColorSelected")] [SerializeField] Color targetColor = Color.white;
        [InfoBox("Fill one of these slots", InfoBoxType.Error, "ColorFiledsSetup")] [ShowIf("ImageSelected")] [SerializeField] Image targetImage = null;
        [ShowIf("SpriteRendererSelected")] [SerializeField] SpriteRenderer targetSpriteRenderer = null;
        [ShowIf("MaterialSelected")] [SerializeField] Material targetMaterial = null;
        bool ColorFiledsSetup() { return (targetImage == null && targetSpriteRenderer == null && targetMaterial == null); }

        // Alpha
        private bool AlphaSelected() { return (animationType == AnimationType.AlphaTo || animationType == AnimationType.AlphaFrom); }
        private bool AlphaImageSelected() { return ((animationType == AnimationType.AlphaTo || animationType == AnimationType.AlphaFrom) && (targetAlphaSpriteRenderer == null && targetAlphaMaterial == null)); }
        private bool AlphaSpriteRendererSelected() { return ((animationType == AnimationType.AlphaTo || animationType == AnimationType.AlphaFrom) && (targetAlphaImage == null && targetAlphaMaterial == null)); }
        private bool AlphaMaterialSelected() { return ((animationType == AnimationType.AlphaTo || animationType == AnimationType.AlphaFrom) && (targetAlphaImage == null && targetAlphaSpriteRenderer == null)); }
        [ShowIf("AlphaSelected")] [SerializeField] float targetAlpha = 0f;
        [InfoBox("Fill one of these slots", InfoBoxType.Error, "AlphaFiledsSetup")] [ShowIf("AlphaImageSelected")] [SerializeField] Image targetAlphaImage = null;
        [ShowIf("AlphaSpriteRendererSelected")] [SerializeField] SpriteRenderer targetAlphaSpriteRenderer = null;
        [ShowIf("AlphaMaterialSelected")] [SerializeField] Material targetAlphaMaterial = null;
        bool AlphaFiledsSetup() { return (targetAlphaImage == null && targetAlphaSpriteRenderer == null && targetAlphaMaterial == null); }

        private Tweener tween;

        private void Awake()
        {
            if (autoPlay) Play();
        }

        public void Play()
        {
            switch (animationType)
            {
                case AnimationType.MoveTo:
                    MoveToTween();
                    break;
                case AnimationType.MoveFrom:
                    MoveFromTween();
                    break;
                case AnimationType.RotateTo:
                    RotateToTween();
                    break;
                case AnimationType.RotateFrom:
                    RotateFromTween();
                    break;
                case AnimationType.ScaleTo:
                    ScaleTweenTo();
                    break;
                case AnimationType.ScaleFrom:
                    ScaleTweenFrom();
                    break;
                case AnimationType.ColorTo:
                case AnimationType.ColorFrom:
                    ColorTween();
                    break;
                case AnimationType.AlphaFrom:
                case AnimationType.AlphaTo:
                    AlphaTween();
                    break;
                default:
                    break;
            }
            PostTween();
        }

        private void PostTween()
        {
            tween.SetEase(easeType);
            tween.SetLoops(loopCount, loopType);
            tween.SetDelay(delay);
        }

        void MoveToTween()
        {
            tween = transform.DOMove(targetPosition, duration);
            tween.SetRelative(relative);
        }
        private void MoveFromTween()
        {
            if (relative)
            {
                transform.Translate(targetPosition);
                tween = transform.DOMove(-targetPosition, duration);
                tween.SetRelative(true);
            }
            else
            {
                var startPos = transform.position;
                transform.position = targetPosition;
                tween = transform.DOMove(startPos, duration);
            }
        }

        private void RotateToTween()
        {
            tween = transform.DORotate(targetRotation, duration);
            tween.SetRelative(relative);
        }

        private void RotateFromTween()
        {
            if (relative)
            {
                transform.Rotate(targetRotation, Space.Self);
                tween = transform.DORotate(-targetRotation, duration);
                tween.SetRelative(true);
            }
            else
            {
                var startRot = transform.localRotation;
                transform.localRotation = Quaternion.Euler(targetRotation);
                tween = transform.DORotate(startRot.eulerAngles, duration);
            }
        }

        private void ScaleTweenTo()
        {
            tween = transform.DOScale(targetScale, duration);
            tween.SetRelative(relative);
        }

        private void ScaleTweenFrom()
        {
            var startscale = transform.localScale;
            if (relative) transform.localScale = Vector3.Scale(transform.localScale, targetScale);
            else transform.localScale = targetScale;
            tween = transform.DOScale(startscale, duration);
        }

        void ColorTween()
        {
            var finalTarget = targetColor;
            if (targetImage != null)
            {
                if (animationType == AnimationType.ColorFrom)
                {
                    finalTarget = targetImage.color;
                    targetImage.color = targetColor;
                }
                tween = targetImage.DOColor(finalTarget, duration);
            }
            else if (targetSpriteRenderer != null)
            {
                if (animationType == AnimationType.ColorFrom)
                {
                    finalTarget = targetSpriteRenderer.color;
                    targetSpriteRenderer.color = targetColor;
                }
                tween = targetSpriteRenderer.DOColor(finalTarget, duration);
            }
            else if (targetMaterial != null)
            {
                if (animationType == AnimationType.ColorFrom)
                {
                    finalTarget = targetMaterial.color;
                    targetMaterial.color = targetColor;
                }
                tween = targetMaterial.DOColor(finalTarget, duration);
            }
        }

        void AlphaTween()
        {
            var finalTarget = targetAlpha;
            if (targetAlphaImage != null)
            {
                if (animationType == AnimationType.AlphaFrom)
                {
                    finalTarget = targetAlphaImage.color.a;
                    targetAlphaImage.color = new Color(targetAlphaImage.color.r, targetAlphaImage.color.g, targetAlphaImage.color.b, targetAlpha);
                }
                tween = targetAlphaImage.DOFade(finalTarget, duration);
            }
            else if (targetAlphaSpriteRenderer != null)
            {
                if (animationType == AnimationType.AlphaFrom)
                {
                    finalTarget = targetAlphaSpriteRenderer.color.a;
                    targetAlphaSpriteRenderer.color = new Color(targetAlphaSpriteRenderer.color.r, targetAlphaSpriteRenderer.color.g, targetAlphaSpriteRenderer.color.b, targetAlpha);
                }
                tween = targetAlphaSpriteRenderer.DOFade(finalTarget, duration);
            }
            else if (targetAlphaMaterial != null)
            {
                if (animationType == AnimationType.AlphaFrom)
                {
                    finalTarget = targetAlphaMaterial.color.a;
                    targetAlphaMaterial.color = new Color(targetAlphaMaterial.color.r, targetAlphaMaterial.color.g, targetAlphaMaterial.color.b, targetAlpha);
                }
                tween = targetAlphaMaterial.DOFade(finalTarget, duration);
            }
        }
    }
}