using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargeterUI : MonoBehaviour
{
    [SerializeField] private Transform _baseParent;
    private Transform _source;
    private Transform _target;
    private bool _frozen = false;

    private void Update()
    {
        if (_source != null) { SetSource(_source); }
        if (!_frozen)
        {
            SetPosition(Input.mousePosition);
        } else
        {
            SetPosition(_target.position);
        }
    }

    public void SetSource(Transform a_source)
    {
        _source = a_source;
        transform.SetParent(a_source.parent);
        transform.localPosition = _source.localPosition;
        _source.SetAsLastSibling();
        CardUI card = a_source.GetComponent<CardUI>();
        card?.OverrideSorting(true);
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        _frozen = false;
        transform.SetParent(_baseParent);
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0.01f, rect.rect.height);
        if (_source != null)
        {
            CardUI card = _source.GetComponent<CardUI>();
            card?.OverrideSorting(false);
        }
        gameObject.SetActive(false);
    }

    public void Freeze(Transform a_target)
    {
        _target = a_target;
        _frozen = true;
    }

    private void SetPosition(Vector3 a_target)
    {
        Vector2 targetPosition = a_target - _source.position;
        float distance = targetPosition.magnitude;
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(distance, rect.rect.height);
        gameObject.transform.right = targetPosition;
    }
}
