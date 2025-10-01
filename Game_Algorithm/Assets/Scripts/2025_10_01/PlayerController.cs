using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float moveDuration = 0.3f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // GameManager로부터 이동 방향을 직접 받아 부드럽게 움직이는 코루틴
    public IEnumerator ExecuteMoveSmoothly(Vector3 moveDirection)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + moveDirection;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    public void ResetPosition(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }
}