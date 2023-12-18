using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;



namespace StarterAssets
{
    public class HookshotHandle : MonoBehaviour
    {
        private Vector3 hookshotPosition;
        //private float hookshotSize;
        private Vector3 characterVelocityMomentum;
        public LineRenderer lr;
        private GameObject player;
        private CharacterController _controller;
        private ActionManager actionManager;
        private ThirdPersonController _thirdPersonController;

        private float gravity;



        [Header("Hookshot")]
        //[SerializeField] private Transform debugHitPosition;
        [SerializeField] private LayerMask Hookable;
        [SerializeField] private Transform hookshotTransform;
        [SerializeField] private AnimationCurve AnimationCurve;
        public bool hookshotAble;
        public int quality;
        public float waveCount;
        public float waveHeight;




        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
            player = GameObject.Find("PlayerArmature");
            _controller = GetComponent<CharacterController>();
            actionManager = GetComponent<ActionManager>();
            _thirdPersonController = GetComponent<ThirdPersonController>();


        }

        private void Start()
        {
            gravity = _thirdPersonController.Gravity;
        }

        private void Update()
        {

            HandleHookshotStart();
            if (hookshotAble == true)
            {
                HandleHookshotMovement();
            }
        }

        private void LateUpdate()
        {
            DrawRope();
        }


        //Hookshot
        public void HandleHookshotStart()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100f, Hookable))
                {
                    hookshotAble = true;
                    hookshotPosition = hit.point;
                }
            }
        }

        //ロープ描写
        private Vector3 currentHookshot;
        public void DrawRope()
        {
            currentHookshot = Vector3.Lerp(currentHookshot, hookshotTransform.position, Time.deltaTime * 8f);
            lr.SetPosition(0, hookshotTransform.position);
            lr.SetPosition(1, hookshotPosition);
        }

        public bool isreached;
        public void HandleHookshotMovement()
        {
            //print(lr.enabled);
            lr.enabled = true;

            // debugHitPosition の位置を hit.point に設定
            //debugHitPosition.position = hitPoint;
            //hookshotPosition = hitPoint;

            //normalizedはベクトルの正規化(ゼロベクトル）
            Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;
            hookshotDir.y += gravity * Time.deltaTime;

            if (hookshotDir.magnitude > 0.01)
            {
                player.transform.rotation = Quaternion.LookRotation(hookshotDir);
            }

            float hookshotSpeedMin = 10f;
            float hookshotSpeedMax = 40f;
            float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
            float hookshotSpeedMultipulier = 2f;
            _controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultipulier * Time.deltaTime);
           

            float reachedHookshotPositionDistance = 3f;
            isreached = Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance;
            if (isreached)
            {

                HookDelete();
                _controller.Move(new Vector3(0.4f, 3f, 0.4f));
                hookshotAble = false;

                return;
            }

            //キャンセル
            if (Input.GetMouseButtonDown(0))
            {
                float momentumExtraSpeed = 7f;
                characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
                float jumpSpeed = 40f;
                characterVelocityMomentum += Vector3.up * jumpSpeed;
                HookDelete();
                return;
            }
        }

        private void HookDelete()
        {
            if (lr != null)
            {
                lr.enabled = false;
            }
        }
    }
}