using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(NavMeshAgent))]
public class Animal : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private Color _countColor;

    private Animator _animator;
    private Outline _outline;
    private NavMeshAgent _agent;
    private Damping _damp = new Damping(0.5f, 2, 0, 1);
    private Damping _errorDamp= new Damping(0.1f, 5, 0, 1);

    private Coroutine _moveTask;
    private Coroutine _rotateTask;
    private Coroutine _pressTask;
    private Coroutine _shakeTask;

    public int ID => _id;
    public Color CountColor => _countColor;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _outline = GetComponent<Outline>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(RandomIdle());
    }

    private IEnumerator RandomIdle()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            string[] triggers = new string[] { "lookLeft", "lookRight", "munch", "munch", "munch", "munch", "munch", "munch" };
            _animator.SetTrigger(triggers[Random.Range(0, triggers.Length)]);

        }
    }

    public void Select()
    {
        _outline.enabled = true;
        if (_pressTask != null)
            StopCoroutine(_pressTask);

        _pressTask = StartCoroutine(ShowPress());
    }

    public void Unselect()
    {
        _outline.enabled = false;
    }

    public void Shake()
    {
        if (_shakeTask != null)
            StopCoroutine(_shakeTask);

        _shakeTask = StartCoroutine(ShowShake());
    }

    public void Go(Vector3 targetPosition, float duration)
    {
        if (_moveTask != null)
            StopCoroutine(_moveTask);

        if (_rotateTask != null)
            StopCoroutine(_rotateTask);

        _moveTask = StartCoroutine(Move(targetPosition, duration));
        _rotateTask = StartCoroutine(RotateBack(0.25f, duration));
    }

    public void Navigate(Vector3 targetPosition)
    {
        _agent.enabled = true;
        _agent.SetDestination(targetPosition);
    }

    private IEnumerator Move(Vector3 targetPosition, float duration, float delay = 0, string ease = "easeInOut")
    {
        yield return new WaitForSeconds(delay);

        float time = 0;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 delta = targetPosition - position;
        Quaternion targetRotation = delta.magnitude > 0.1f ? Quaternion.LookRotation(targetPosition - position, Vector3.up) : rotation;

        while (time < duration)
        {
            float value = time / duration;
            switch (ease)
            {
                case "easeInOut":
                    value = Ease.EaseInEaseOut(value);
                    break;
                case "easeIn":
                    value = Ease.EaseIn(value);
                    break;
                case "easeOut":
                    value = Ease.EaseOut(value);
                    break;
            }
            transform.position = Vector3.Lerp(position, targetPosition, value);
            transform.rotation = Quaternion.Lerp(rotation, targetRotation, value * 2);
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = transform.position;
    }

    private IEnumerator RotateBack(float duration, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Quaternion rotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
        float time = 0;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(rotation, targetRotation, time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        transform.rotation = targetRotation;
    }

    public void MoveToAviary(Aviary aviary)
    {
        Vector3 delta = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 0.2f;
        float distanceToDoor = Vector3.Distance(transform.position, aviary.DoorPosition);
        float distanceToAviary = Vector3.Distance(aviary.DoorPosition, aviary.transform.position + aviary.transform.forward * -1.5f + delta);
        float totalDuration = 0.4f;
        float totalDistance = distanceToDoor + distanceToAviary;
        float toDoorDuration = totalDuration * distanceToDoor / totalDistance;
        float toAviaryDuration = totalDuration * distanceToAviary / totalDistance;


        StartCoroutine(Move(aviary.DoorPosition, toDoorDuration, 0, "easeIn"));
        StartCoroutine(Move(aviary.transform.position + delta, toAviaryDuration, toDoorDuration, "easeOut"));
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent.enabled)
            _agent.SetDestination(position);
    }

    public void PlayAnimation(string name)
    {
        _animator.SetTrigger(name);
    }

    private IEnumerator ShowPress()
    {
        float duration = 0.3f;
        float time = 0;
        Vector3 scale = transform.localScale;
        Vector3 targetScale = new Vector3(1.5f, 0.6f, 1.3f);

        while (time < duration)
        {
            float value = _damp.GetValue(time / duration);
            float x = scale.x + (targetScale.x - scale.x) * value;
            float y = scale.y + (targetScale.y - scale.y) * value;
            float z = scale.z + (targetScale.z - scale.z) * value;
            transform.localScale = new Vector3(x, y, z);
            yield return null;
            time += Time.deltaTime;
        }

        transform.localScale = scale;
    }

    private IEnumerator ShowShake()
    {
        float duration = 0.5f;
        float time = 0;
        Vector3 position = transform.position;
        Vector3 targetPosition = position + Vector3.right * 0.5f;

        while (time < duration)
        {
            float value = _errorDamp.GetValue(time / duration);
            float x = position.x + (targetPosition.x - position.x) * value;
            float y = position.y + (targetPosition.y - position.y) * value;
            float z = position.z + (targetPosition.z - position.z) * value;
            transform.position = new Vector3(x, y, z);
            yield return null;
            time += Time.deltaTime;
        }

        transform.position = position;
    }
}
