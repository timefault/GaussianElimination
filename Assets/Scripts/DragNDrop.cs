using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class DragNDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    Canvas canvas;
    static GameObject clone;

    private void OnEnable()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        rectTransform = clone.GetComponent<RectTransform>();
        canvasGroup = clone.GetComponent<CanvasGroup>();
        canvasGroup.alpha = .4f;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop");
        SumRows(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
        clone = Instantiate(gameObject, transform.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("PointerUP");
        clone.SetActive(false);
        Destroy(clone,1);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SumRows(GameObject target)
    {
        Debug.Log(target.name);
        // add clone values to drop target values, set to drop target values
        // target.value =  clone + target
        for (int i = 0; i < target.transform.childCount; i++)
        {
            //Debug.Log(clone);   // do scripts change after ondrop?? I think the closure changed, made clone static for current fix
            //Debug.Log(target.transform.GetChild(i));
            if (!target.transform.GetChild(i).CompareTag("Matrix Element")) return;

            Text targetText = target.transform.GetChild(i).GetComponent<Text>();
            Text cloneText = clone.transform.GetChild(i).GetComponent<Text>();
            //TMP_Text cloneText = clone.transform.GetChild(i).GetComponent<TMP_Text>();
            //TMP_Text cloneText = clone.transform.GetChild(i).GetComponent<TMP_Text>();

            Debug.Log(" ========== ");
            string s1="", s2="";        // wtf, why is there an unprintable, unparsable character in the Text Component's text field
                                        // honestly, I think it is a datatype thing
            foreach (char c in targetText.text)
            {
                Debug.Log(c);
                if (!Regex.IsMatch(c.ToString(), @"[0-9\.\-]")) continue;
                s1 += c;
            } 
            foreach (char c in cloneText.text)
            {
                Debug.Log(c);
                if (!Regex.IsMatch(c.ToString(), @"[0-9\.\-]")) continue;
                s2 += c;
            } 
            Debug.Log("s1.Length "+s1.Length);
            Debug.Log("3".Length);
            float num1 = 0, num2 = 0;
            Debug.Log(float.TryParse(s1, out num1));
            Debug.Log(float.TryParse(s2, out num2));
            Debug.Log("Length "+targetText.text.Length);
            Debug.Log("num1 "+num1);
            Debug.Log("num2 "+num2);


            //targetText.SetText(int.Parse(targetText.text) + int.Parse(cloneText.text).ToString());

            targetText.text = (num1 + num2).ToString();
        }
    }
}
