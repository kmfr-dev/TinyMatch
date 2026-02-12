using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    // 기본 블록 설정값 -> 에디터에서 제어
    [SerializeField]
    private BlockConfig mBlockConfig = null;

    // 실제 블록 데이터
    public BlockData mBlockData { get; private set; } = null;

    private void Awake()
    {
        // 블록데이터 생성 및 초기 위치 세팅
        mBlockData = new BlockData();
        mBlockData.TargetPos = transform.position;
    }

    void Update()
    {
        if (null == mBlockData)
            return;

        // 현재 위치에서 Target위치까지의 거리가 0.01보다 크다면 
        if(Vector2.Distance(transform.position, mBlockData.TargetPos) > 0.01f)
        {
            // 부드러운 이동 애니메이션
            transform.position = Vector2.Lerp(transform.position, mBlockData.TargetPos, mBlockConfig.MoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = mBlockData.TargetPos;
        }
    }

    // 부드럽게 블럭이 삭제되는 코루틴 함수
    public IEnumerator SMmoothDestroyBlock()
    {
        float currentTime = 0f;
        float duration = mBlockConfig.DestroyDuration;

        Vector2 startScale = gameObject.transform.localScale;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            gameObject.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, currentTime / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}