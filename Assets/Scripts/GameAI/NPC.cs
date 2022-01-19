using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class NPC : MonoBehaviour
{
    public float speed = 5f;
    public Queue<Vector2> mWayPoints = new Queue<Vector2>();
    public UnityEvent OnMoveToGoal;
    private bool hasAddWaypoint;
    private void Start()
    {
        hasAddWaypoint = false;
    }
    public void Init()
    {
        StartCoroutine(Coroutine_Move());
    }

    public void AddWayPoint(float x, float y)
    {
        AddWayPoint(new Vector2(x, y));
    }
    public void AddWayPoint(Vector2 pt)
    {
        mWayPoints.Enqueue(pt);
        if(hasAddWaypoint == false)
            hasAddWaypoint = true;
    }
    public void SetPostition(float x, float y)
    {
        mWayPoints.Clear();
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public IEnumerator Coroutine_Move()
    {
        while (hasAddWaypoint)
        {
            while (mWayPoints.Count > 0)
            {
                yield return StartCoroutine(
                    Coroutine_MoveToPoint(
                        mWayPoints.Dequeue(), speed));
            }
            OnMoveToGoal?.Invoke();
            yield return null;
        }
    }

    private IEnumerator Coroutine_MoveToPoint(Vector2 vector2, float speed)
    {
        Vector3 endP = new Vector3(vector2.x, vector2.y, transform.position.z);
        float duration = (transform.position - endP).magnitude / speed;
        yield return StartCoroutine(
            Coroutine_MoveOverSeconds(transform.gameObject, endP, duration)
        );
    }

    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 endP, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < duration)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endP, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endP;
    }
}
