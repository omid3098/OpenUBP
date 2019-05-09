using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EyeFollow : MonoBehaviour
{
    [Header("Eye")]
    [SerializeField] Transform[] eyePupils = new Transform[0];
    [SerializeField] Transform eyeBlinkParent = null;
    [SerializeField] float EyeRadius = 0.5f;
    [SerializeField] int updateInterval = 3;
    [Tooltip("Set target for eyes use null to follow mouse/touch")]
    [SerializeField] Transform eyeTarget;

    [Space(10)]
    [Header("Lip")]
    [SerializeField] Transform lipTransform = null;
    [Range(-1, 1)]
    [SerializeField] float happyValue = 1f;
    [SerializeField] int lipUpdateInterval = 30;

    [Space(10)]
    [Header("Hand")]
    [SerializeField] Transform[] hands = new Transform[0];
    [Range(0, 360)] [SerializeField] float bybyAngle = 20f;
    [SerializeField] float bybyDuration = 1f;
    Quaternion[] handStartRotation;
    private bool handInTween;
    private Camera _camera;

    private float randomBlinkTime;
    private float ellapsedTime;
    private float startScaleY;
    private bool centered;

    private void Awake()
    {
        _camera = Camera.main;
        randomBlinkTime = Random.Range(1f, 4f);
        ellapsedTime = 0;
        startScaleY = eyeBlinkParent.localScale.y;
        if (lipTransform != null) lipTransform.localScale = new Vector3(lipTransform.localScale.x, happyValue, 1);

        handStartRotation = new Quaternion[hands.Length];
        for (int i = 0; i < hands.Length; i++) handStartRotation[i] = hands[i].localRotation;

        // Hand tweens
        for (int i = 0; i < hands.Length; i++)
        {
            Transform hand = hands[i];
            var localByBy = bybyAngle;
            if (i % 2 == 0) localByBy *= -1;
            var tween = hand.DOBlendableLocalRotateBy(new Vector3(0, 0, localByBy), bybyDuration);
            tween.SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void SetTarget(Transform target)
    {
        this.eyeTarget = target;
    }

    void Update()
    {
        ellapsedTime += Time.deltaTime;
        if (hasInput)
        {
            if (Time.frameCount % updateInterval == 0)
            {
                centered = false;
                var tmpTarget = targetPosition;
                foreach (var pupil in eyePupils)
                    pupil.localPosition = EyeRadius * (Quaternion.Inverse(transform.rotation) * (tmpTarget - pupil.position)).normalized;
                for (int i = 0; i < hands.Length; i++)
                {
                    Transform hand = hands[i];
                    var norm = tmpTarget;
                    float rot_z = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg;
                    hand.rotation = Quaternion.Euler(0f, 0f, rot_z);
                }
            }
        }
        else if (releaseInput)
        {
            if (!centered)
            {
                foreach (var insideEye in eyePupils)
                    insideEye.localPosition = Vector3.zero;
                for (int i = 0; i < hands.Length; i++)
                    hands[i].localRotation = handStartRotation[i];
                centered = true;
            }
        }

        // blink mechanism
        if (ellapsedTime > randomBlinkTime)
        {
            ellapsedTime = 0;
            randomBlinkTime = Random.Range(1f, 4f);
            var tween = eyeBlinkParent.DOScaleY(0, 0.05f);
            tween.onComplete += () =>
            {
                var scaleUpTween = eyeBlinkParent.DOScaleY(startScaleY, 0.3f);
                scaleUpTween.SetEase(Ease.OutQuart);
            };
        }

        // lip update interval
        if (lipTransform != null)
        {
            if (Time.frameCount % lipUpdateInterval == 0)
            {
                lipTransform.localScale = new Vector3(lipTransform.localScale.x, happyValue, 1);
            }
        }
    }


    private Vector3 targetPosition
    {
        get
        {
            if (eyeTarget != null) return eyeTarget.position;
#if UNITY_EDITOR || UNITY_STANDALONE
            return _camera.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0) return _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
        else return Vector3.zero;
#endif
        }
    }

    private bool hasInput
    {
        get
        {
            if (eyeTarget != null) return true;
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButton(0);
#elif UNITY_ANDROID || UNITY_IOS
        return Input.touchCount > 0;
#endif
        }
    }
    private static bool releaseInput
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IOS
            return Input.touchCount == 0;
#endif
        }
    }
}
