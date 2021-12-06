using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode(), RequireComponent(typeof(RectTransform))]
public class TooltipWindow : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;
    public LayoutElement layoutElement;
    public int characterWrapLimit;

    private RectTransform _rectTransform;

    void Update()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        if (Application.isEditor)
        {
            UpdateSize();
        }
        UpdatePosition();
    }

    public void SetText(string a_content, string a_header = "")
    {
        if (string.IsNullOrEmpty(a_header))
        {
            header.gameObject.SetActive(false);
        }
        else
        {
            header.gameObject.SetActive(true);
            header.text = a_header;
        }
        content.text = a_content;
        UpdateSize();
    }

    void UpdateSize()
    {
        int headerLength = header.text.Length;
        int contentLength = content.text.Length;
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    void UpdatePosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        _rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}