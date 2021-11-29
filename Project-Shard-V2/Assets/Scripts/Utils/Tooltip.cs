using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static GameObject _window;

    public string header;
    [TextArea(minLines: 5, maxLines: 10)]
    public string content;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_window == null) { return; }
        Vector2 pos = eventData.position;
        _window.transform.Find("header").GetComponent<TextMeshProUGUI>().text = header;
        _window.transform.Find("content").GetComponent<TextMeshProUGUI>().text = content;
        if (header.Length == 0) { return; }
        _window.SetActive(true);
        float dx = _window.GetComponent<RectTransform>().rect.width * 0.6f;
        float dy = _window.GetComponent<RectTransform>().rect.height * 0.6f;
        if ((pos.x - Screen.width/2.0f) > 0) { dx *= -1; }
        if ((pos.y - Screen.height/2.0f) > 0) { dy *= -1; }
        _window.transform.position = new Vector2( pos.x + dx, pos.y + dy);
        _window.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_window == null) { return; }
        _window.SetActive(false);
    }
}
