using UnityEngine;

// "�̼� ������ ����"�� ���� (���� ����)
[System.Serializable]
public class MissionInfo
{
    public string title;         // �̼� ����
    [TextArea(2, 5)]
    public string description;   // �̼� ����(���� ��)
    public string location;      // ��ġ ����
    public string hint;          // ��Ʈ (���� ����)
}
