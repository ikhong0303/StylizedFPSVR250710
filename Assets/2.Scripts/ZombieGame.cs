using UnityEngine;
using System.Collections;
using TMPro;
using Random = UnityEngine.Random;

namespace MikeNspired.XRIStarterKit
{
    public class ZombieGame : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshPro gameOverText;  // 게임 오버 텍스트
        [SerializeField] private TextMeshPro scoreText;     // 점수 표시 텍스트
        [SerializeField] private AudioSource gameOverAudio; // 게임 오버 사운드
        [SerializeField] private AudioSource gameAudio;     // 게임 배경 음악

        [Header("Level Settings")]
        [SerializeField] private int spawnIncreasePerLevel = 2;          // 웨이브당 추가 좀비 수
        [SerializeField] private float spawnDurationIncreasePerLevel = 2f; // 좀비 생성 지속 시간 증가량
        [SerializeField] private float timeBetweenWaves = 1.5f;          // 웨이브 간 대기 시간

        [Header("Spawn Points")]
        [SerializeField] private Transform spawnPoint1; // 스폰 라인의 시작
        [SerializeField] private Transform spawnPoint2; // 스폰 라인의 끝
        [SerializeField] private GameObject zombiePrefab; // 생성할 좀비 프리팹

        [Header("Lighting")]
        [SerializeField] private SetWorldLighting worldLighting;  // 월드 라이트 제어
        [SerializeField] private ListActivator lightListActivator; // 스폰 시 끌 라이트

        [Header("Spawn Randomization")]
        [Tooltip("스폰 라인에 수직으로 좀비를 퍼트릴 범위")]
        [SerializeField] private float spawnVariation = 1f;
        [Tooltip("좀비 스케일 변형 범위 (±20% 등)")]
        [SerializeField] private float scaleVariationAmount = 0.2f;

        [Header("Zombie Movement Randomization")]
        [SerializeField] private float minSpeed = 0.65f;         // 최소 속도
        [SerializeField] private float maxSpeed = 1f;            // 최대 속도
        [SerializeField] private float speedVariation = 0.1f;    // 랜덤 속도 편차
        [SerializeField] private float timeToSpeedMin = 1f;      // 최소 가속 시간
        [SerializeField] private float timeToSpeedMax = 3f;      // 최대 가속 시간

        // 내부 상태 변수
        private bool gameRunning;
        private int currentLevel = 1;
        private int score;
        private int zombiesToSpawn;
        private int zombiesRemaining;
        private float currentSpawnDuration;
        private Camera playerCamera;

        private void Start()
        {
            // 메인 카메라 참조 저장
            playerCamera = Camera.main;
        }

        /// <summary>
        /// 게임 시작. 점수 초기화, UI 세팅, 좀비 웨이브 시작
        /// </summary>
        public void StartGame()
        {
            gameOverText.transform.parent.gameObject.SetActive(false); // 게임오버 UI 숨김
            scoreText.gameObject.SetActive(true);                      // 점수 UI 표시
            gameRunning = true;
            currentSpawnDuration = spawnDurationIncreasePerLevel;

            currentLevel = 1;
            score = 0;
            scoreText.text = "Score: 0";

            SetLighting(false); // 조명 끄기
            StartCoroutine(SpawnWave()); // 첫 웨이브 시작
            gameAudio.Play(); // 배경음 재생
        }

        /// <summary>
        /// 게임 시작 시 조명 설정 변경
        /// </summary>
        private void SetLighting(bool isOn)
        {
            if (!isOn)
            {
                worldLighting.DarkenWorld();   // 어둡게 만들고
                lightListActivator.Deactivate(); // 불 끄기
                return;
            }

            worldLighting.ReturnToStartingColor();
            lightListActivator.Activate();
        }

        /// <summary>
        /// 좀비 웨이브 생성 루틴
        /// </summary>
        IEnumerator SpawnWave()
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            // 현재 레벨에 따라 생성 수 계산
            zombiesToSpawn = spawnIncreasePerLevel + (currentLevel - 1) * 2;
            zombiesRemaining = zombiesToSpawn;

            // 일정 시간에 걸쳐 좀비 생성
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                SpawnZombie();
                yield return new WaitForSeconds(currentSpawnDuration / zombiesToSpawn);
            }
        }

        /// <summary>
        /// 하나의 좀비를 랜덤 위치에 생성
        /// </summary>
        private void SpawnZombie()
        {
            // spawnPoint1 ~ spawnPoint2 사이 랜덤 위치 선택
            float t = Random.Range(0f, 1f);
            Vector3 lineDirection = (spawnPoint2.position - spawnPoint1.position);
            Vector3 baseSpawnPos = spawnPoint1.position + lineDirection * t;

            // 수직(좌우) 방향 오프셋 추가
            Vector3 perpendicular = Vector3.Cross(lineDirection.normalized, Vector3.up).normalized;
            float offset = Random.Range(-spawnVariation, spawnVariation);
            Vector3 spawnPosition = baseSpawnPos + perpendicular * offset;

            // 좀비 생성
            GameObject zombieObj = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            // 생성 시 플레이어 방향으로 회전


            /// <summary>
            /// 생성된 좀비를 플레이어 방향으로 회전
            /// </summary>

            /// <summary>
            /// 에디터에서 스폰 위치 시각화
            /// </summary>
            void OnDrawGizmosSelected()
            {
                Gizmos.color = Color.green;
                if (spawnPoint1 && spawnPoint2)
                {
                    Gizmos.DrawSphere(spawnPoint1.position, 0.2f);
                    Gizmos.DrawSphere(spawnPoint2.position, 0.2f);
                    Gizmos.DrawLine(spawnPoint1.position, spawnPoint2.position);
                }
            }
        }
    }
}
