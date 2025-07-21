using UnityEngine;

// "미션 데이터 구조"만 정의 (로직 없음)
[System.Serializable]
public class MissionInfo
{
    public string title;         // 미션 제목
    [TextArea(2, 5)]
    public string description;   // 미션 설명(여러 줄)
    public string location;      // 위치 정보
    public string hint;          // 힌트 (생략 가능)
}
