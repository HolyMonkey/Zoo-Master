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

    private Animator _animator;
    private Outline _outline;
    private NavMeshAgent _agent;
    private Damping _damp = new Damping(0.5f, 2, 0, 1);
    private Damping _errorDamp= new Damping(0.1f, 5, 0, 1);

    private Coroutine _moveTask;
    private Coroutine _pressTask;
    private Coroutine _shakeTask;

    public int ID => _id;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _outline = GetComponent<Outline>();
        _agent = GetComponent<NavMeshAgent>();
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

    public void Go(Vector3 targetPosition, float duration, float delay = 0)
    {
        if (_moveTask != null)
            StopCoroutine(_moveTask);

        _moveTask = StartCoroutine(Move(targetPosition, delay, duration));
    }

    public void Navigate(Vector3 targetPosition)
    {
        _agent.enabled = true;
        _agent.SetDestination(targetPosition);
    }

    private IEnumerator Move(Vector3 targetPosition, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float time = 0;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 delta = targetPosition - position;
        Quaternion targetRotation = delta.magnitude > 0.1f ? Quaternion.LookRotation(targetPosition - position, Vector3.up) : rotation;
        //_animator.SetBool("isWalking", true);
        while (time < duration)
        {
            transform.position = Vector3.Lerp(position, targetPosition, time / duration);
            transform.rotation = Quaternion.Lerp(rotation, targetRotation, time / duration * 2);
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = transform.position;
        //_animator.SetBool("isWalking", false);


        rotation = transform.rotation;
        targetRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
        duration = 0.25f;
        time = 0;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(rotation, targetRotation, time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        transform.rotation = targetRotation;
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent.enabled)
            _agent.SetDestination(position);
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
