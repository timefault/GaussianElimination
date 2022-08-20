using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class RowHandler : MonoBehaviour, IPointerClickHandler
{
    // should have made references to each of the row handlers children instead of finding them after enable and start


    bool inputHasChanged;
    GameObject scalarInputField;

    GameObject row;
    GameObject increaseButton, decreaseButton, applyButton;
    GameObject scalarInput;

    public GameObject matrixElement;

    List<List<GameObject>> terms;

    float someVerySmallNumber = 1e-4f; 

    public int rowNumber;
    // Start is called before the first frame update
    private void OnEnable()
    {
        inputHasChanged = false;
        scalarInputField = transform.Find("Scalar Input Field").gameObject;
        terms = GameObject.Find("Main Controller").GetComponent<SetupEquations>().terms;
        // receive row number
        // then build the row
        increaseButton = transform.Find("Increase Button").gameObject;
        increaseButton.GetComponent<Button>().onClick.AddListener(IncreaseScalar);
        decreaseButton = transform.Find("Decrease Button").gameObject;
        decreaseButton.GetComponent<Button>().onClick.AddListener(DecreaseScalar);
        applyButton = transform.Find("Apply Button").gameObject;
        applyButton.GetComponent<Button>().onClick.AddListener(ApplyScalar);
        scalarInput = transform.Find("Scalar Input Field").gameObject;
        scalarInput.GetComponent<InputField>().onValueChanged.AddListener(delegate { HandleInputChange(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (!(EventSystem.current.currentSelectedGameObject == scalarInputField) && inputHasChanged)
        {
            InputBoxArithmetic();
        }
        
    }
    public void IncreaseScalar() {
        InputField inputField = gameObject.GetComponentInChildren<InputField>();
        Debug.Log(inputField.text);
        string inputText = inputField.text;
        int acc = 0;
        if (int.TryParse(inputText, out acc)) { Debug.Log("Success"); } else {Debug.Log("Failure");/*  acc = 0 */  }
        acc++;
        inputField.text = acc.ToString();
        Debug.Log(inputField.text);
        
    }
    public void DecreaseScalar() {
        InputField inputField = gameObject.GetComponentInChildren<InputField>();
        Debug.Log(inputField.text);
        string inputText = inputField.text;
        int acc = 0;
        if (int.TryParse(inputText, out acc)) { Debug.Log("Success"); } else {Debug.Log("Failure");/*  acc = 0 */  }
        acc--;
        inputField.text = acc.ToString();
        Debug.Log(inputField.text);
    }
    public void ApplyScalar() {

        // mutliply row by scalar input
        for (int i = 0; i < row.transform.childCount; i++) {
            GameObject currElement = row.transform.GetChild(i).gameObject;
            string s1 = "", s2 = "";
            s1 = GetTextOnly(currElement.GetComponent<Text>().text);
            s2 = GetTextOnly(scalarInput.GetComponent<InputField>().text);
            Debug.Log(" ========= "+currElement.GetComponent<Text>().text.Length);
            Debug.Log(" +++++++++ "+s1.Length);
            Debug.Log(" --------- "+s2.Length);
            if (float.TryParse(s1, out float scalarInputValue) &&
                float.TryParse(s2, out float currElementValue))
            {
                Debug.Log("Success");
                float result = scalarInputValue * currElementValue;
                if (Mathf.Abs(result - Mathf.Round(result)) < someVerySmallNumber) result = Mathf.Round(result);
                currElement.GetComponent<Text>().text = result.ToString();
            }
            else {
                Debug.Log("Failure");
            }

        }
    }
    string GetTextOnly(string s0) {
        string s1="";
        foreach (char c in s0)
            {
                Debug.Log(c);
                if (!Regex.IsMatch(c.ToString(), @"[0-9\.\-]")) continue;
                s1 += c;
            }
        return s1;
    }
    public void BuildRow() {
        List<GameObject> rowElements = new List<GameObject>();
        GameObject currentRowElement;
        row = new GameObject();
        row.name = "Row";
        row.AddComponent<DragNDrop>();
        row.AddComponent<CanvasGroup>();
        row.AddComponent<RectTransform>();
        row.transform.SetParent(transform);
        RectTransform rowRT = row.GetComponent<RectTransform>();
        rowRT.pivot = new Vector2(1,.5f);
        rowRT.anchorMax = new Vector2(1, .5f);
        rowRT.anchorMin = new Vector2(1, .5f);
        row.transform.localPosition = Vector3.zero;
        for (int i = 0; i < terms[rowNumber].Count; i++) {
            // foreach term make matrix element
            currentRowElement = Instantiate(matrixElement, transform.position + Vector3.right * i *
                matrixElement.GetComponent<RectTransform>().sizeDelta.x
                + new Vector3(300, 0, 0), Quaternion.identity, row.transform);
            currentRowElement.name = $"Row Element {i}";
            rowElements.Add(currentRowElement);
        }
        }

    public GameObject GetRow()
    {
        return row;
    }
    void HandleRightClick()
    {
        BuildRow();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if ( eventData.button == PointerEventData.InputButton.Right)
        {
            // invoke the handler(s) and send the message rowNumber; sometimes the message can be empty, but .Invoke calls handlers AND sends a message; (i know...everything derives from message)
            ResetRow.Invoke(rowNumber);
        } 
    }
    // event is a message
    // make event type
    public delegate void RightClickHandler(int rowNumber);
    // make event of event type
    public event RightClickHandler ResetRow;


    void InputBoxArithmetic() {    // select particular row element
        string text = scalarInput.GetComponent<InputField>().text;
        foreach (char c in text) {
            if (Regex.IsMatch(c.ToString(), @"/") )
            {
                // later sanitize input
                string[] operands = text.Split("/");
                if (float.TryParse(operands[0], out float lhs))
                {
                    if (float.TryParse(operands[1], out float rhs))
                    {
                        float result;
                        result = lhs / rhs;
                        scalarInputField.GetComponent<InputField>().text = result.ToString();
                        inputHasChanged = false;                       
                        return;
                    }
                }
                else { Debug.Log("Failed to parse text."); }
            }
            inputHasChanged = false;
        }
    }
    void HandleInputChange()
    {
        inputHasChanged = true;
    }
}
