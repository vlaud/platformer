using UnityEngine;
using System;

namespace WindowsInput
{
    [Serializable]
    /// <summary>
    /// InputSystem에서 Input.GetAxis() 함수를 대체하는 클래스
    /// </summary>
    public class Axis
    {
        [Header("Axis Settings")]
        /// <summary>
        /// 키를 누르지 않을 때 0으로 돌아가는 속도
        /// </summary>
        public float autoReturnSpeed = 3f;

        /// <summary>
        /// 키를 누를 때 움직이는 속도
        /// </summary>
        public float digitalReturnSpeed = 3f;

        /// <summary>
        /// 키를 누르지 않을 때 val이 이 값 이하면 0f로 초기화
        /// </summary>
        public float dead = 0.001f;

        public Axis() { }

        public Axis(float dead, float autoReturnSpeed, float digitalReturnSpeed)
        {
            this.dead = dead;
            this.autoReturnSpeed = autoReturnSpeed;
            this.digitalReturnSpeed = digitalReturnSpeed;
        }

        /// <summary>
        /// 값이 들어올 때, 움직임을 업데이트하는 함수
        /// </summary>
        /// <param name="input">입력 값</param>
        /// <param name="val">움직임 반환 값</param>
        public void UpdateAxis(float input, ref float val)
        {
            // 반대 방향 입력 감지: 현재 움직임(value)과 새 입력(input)의 방향이 반대일 경우
            if ((input > 0 && val < 0) || (input < 0 && val > 0))
            {
                val = 0f; // 값을 0으로 초기화하여 즉시 방향을 전환하도록 함
            }

            // 입력이 양수 방향이면, 움직임 값 상승
            if (input > Mathf.Epsilon)
            {
                val += digitalReturnSpeed * Time.deltaTime;
            }
            // 입력이 음수 방향이면, 움직임 값 하강
            else if (input < -Mathf.Epsilon)
            {
                val -= digitalReturnSpeed * Time.deltaTime;
            }
            // 아무런 입력이 없으면
            else
            {
                // 움직임이 양수 방향이면
                if (val > 0)
                {
                    // 움직임 값 자동 하강
                    val -= autoReturnSpeed * Time.deltaTime;
                    if (val < 0) val = 0;
                }
                // 움직임이 음수 방향이면
                else if (val < 0)
                {
                    // 움직임 값 자동 상승
                    val += autoReturnSpeed * Time.deltaTime;
                    if (val > 0) val = 0;
                }
            }

            // 입력이 없으면서 움직임 절대값이 Dead보다 작으면 0으로 초기화
            if (input == 0 && Mathf.Abs(val) < dead)
            {
                val = 0f;
            }

            // 움직임의 값은 -1 ~ 1 사이로 제한
            val = Mathf.Clamp(val, -1f, 1f);
        }

        /// <summary>
        /// Input Manager에서 Axis를 커스텀할 때 사용하는 함수
        /// </summary>
        /// <returns></returns>
        public float UpdateAxisFromLegacyInput(KeyCode positive, KeyCode negative, ref float value)
        {
            float input = GetValueRaw(positive, negative);
            UpdateAxis(input, ref value);
            return value;
        }

        /// <summary>
        /// 여러 키 Axis 커스터마이징
        /// </summary>
        /// <param name="positive"></param>
        /// <param name="negative"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public float UpdateAxisFromLegacyInput(KeyCode[] positive, KeyCode[] negative, ref float value)
        {
            float input = GetValueRaw(positive, negative);
            UpdateAxis(input, ref value);
            return value;
        }

        /// <summary>
        /// GetAxisRaw 커스텀 함수
        /// </summary>
        /// <returns></returns>
        private float GetValueRaw(KeyCode positive, KeyCode negative)
        {
            bool negativeHeld = Input.GetKey(negative);
            bool positiveHeld = Input.GetKey(positive);

            return (negativeHeld ? -1f : 0f) + (positiveHeld ? 1f : 0f);
        }

        /// <summary>
        /// GetAxisRaw 여러 키 전용
        /// </summary>
        /// <param name="positive"></param>
        /// <param name="negative"></param>
        /// <returns></returns>
        private float GetValueRaw(KeyCode[] positive, KeyCode[] negative)
        {
            bool negativeHeld = false;
            foreach (var key in negative)
            {
                if (Input.GetKey(key))
                {
                    negativeHeld = true;
                    break;
                }
            }
            
            bool positiveHeld = false;
            foreach (var key in positive)
            {
                if (Input.GetKey(key))
                {
                    positiveHeld = true;
                    break;
                }
            }
            return (negativeHeld ? -1f : 0f) + (positiveHeld ? 1f : 0f);
        }
    }
}
