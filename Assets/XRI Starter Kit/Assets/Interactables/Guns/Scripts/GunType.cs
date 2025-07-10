using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 이 클래스는 총기의 타입(종류)을 정의하기 위해 사용되는 ScriptableObject입니다.
    // 예: '소총', '권총', '샷건' 등의 총기 타입 정보를 저장하고 관리하는 데 사용됩니다.

    // Unity 에디터 상에서 새로운 GunType 파일을 생성할 수 있도록 메뉴를 추가합니다.
    // "Assets > Create > ScriptableObject > GunType" 메뉴가 생깁니다.
    [CreateAssetMenu(fileName = "GunType", menuName = "ScriptableObject/GunType")]
    public class GunType : ScriptableObject
    {
        // 현재 이 클래스는 아무 데이터도 담고 있지 않지만,
        // 나중에 여기에 총기 이름, 발사 속도, 데미지 등 다양한 속성을 추가할 수 있습니다.

        // 예시로 아래와 같은 변수를 나중에 추가할 수 있습니다:
        // public string gunName;
        // public float fireRate;
        // public int damage;

        // ScriptableObject는 일반 MonoBehaviour처럼 씬에 붙는 컴포넌트가 아니라
        // 독립된 데이터 자산(asset) 파일로 존재하며, 여러 오브젝트에서 공유하여 사용할 수 있습니다.
    }
}