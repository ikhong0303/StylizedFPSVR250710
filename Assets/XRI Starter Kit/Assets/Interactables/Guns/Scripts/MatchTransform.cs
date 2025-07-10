// Author MikeNspired. 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 지정된 Transform의 위치 또는 회전을 지속적으로 따라가는 컴포넌트입니다.
    /// </summary>
    public class MatchTransform : MonoBehaviour
    {
        // 따라갈 위치 (Position과 Rotation을 복사할 대상)
        public Transform positionToMatch;

        // 시작 시 부모(Parent)를 제거할지 여부
        public bool unParent = false;

        // 위치를 따라갈지 여부
        public bool matchPosition = true;

        // 회전을 따라갈지 여부
        public bool matchRotation;

        private void Start()
        {
            // 시작 시 부모를 제거 (월드 기준으로 독립시키기)
            if (unParent)
                transform.parent = null;
        }

        private void FixedUpdate()
        {
            // 따라갈 대상이 없으면 실행 안 함
            if (!positionToMatch) return;

            // 위치 따라가기
            if (matchPosition)
                transform.position = positionToMatch.position;

            // 회전 따라가기
            if (matchRotation)
                transform.rotation = positionToMatch.rotation;
        }
    }
}