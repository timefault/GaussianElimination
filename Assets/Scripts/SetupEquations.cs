using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;


public class SetupEquations : MonoBehaviour
{
    // annotate this all nicely

    public GameObject endSegment, bodySegment;
    ButtonController bc;

    bool inputHasChanged;

    public int vars = 2;
    public int eqs = 2;

    string[] coefficientLabels = { "A", "B", "C", "D", "E" };
    string[] variableLabels = { "x", "y", "z", "u", "v" };
    List<List<float>> system;
    List<GameObject> equalsSigns;

    public Button rowButton;
    public TMP_InputField scalarInput;
    public GameObject term;
    public GameObject equalsSign;
    public GameObject matrixElement;
    public GameObject rowHandler;

    public List<List<GameObject>> terms;
    List<GameObject> matrixElements;
    List<TMP_InputField> inputs;

    int termIndex = 0;


    GameObject canvas;

    // Start is called before the first frame update, not to be confused with the first thing to happen
    //void Start()
    void OnEnable()
    {
        canvas = GameObject.Find("Canvas");
        GameObject firstEqualsSign;
        RectTransform equationsLabelRT = GameObject.Find("Equations").GetComponent<RectTransform>();
        bc = GetComponent<ButtonController>();
        equalsSigns = new List<GameObject>();
        firstEqualsSign = Instantiate(equalsSign, canvas.transform);
        firstEqualsSign.transform.position = equationsLabelRT.position;
        firstEqualsSign.transform.position += new Vector3(50 * vars, equationsLabelRT.anchoredPosition.y + equationsLabelRT.sizeDelta.y, 0);
        equalsSigns.Add(firstEqualsSign);
        //equalsSigns = new List<GameObject>(GameObject.FindGameObjectsWithTag("Equals Sign"));

        Init();
        terms[0][0].transform.Find("Coefficient").GetComponent<TMP_InputField>().Select();
        PopulateTermsForTesting();
    }
    void PopulateTermsForTesting()  // ========== Testing
    { 
        int z = 1;
        terms.ForEach(eq => { eq.ForEach(el => { el.transform.Find("Coefficient").GetComponent<TMP_InputField>().text = (z++).ToString(); }); });
    }
    // Update is called once per frame
    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.CompareTag("Coefficient"))
        {
            
            int i, j;
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                termIndex--;
                if (termIndex == -1) termIndex = terms.Count * terms[0].Count - 1;
                i = termIndex / terms[0].Count;
                j = termIndex % terms[0].Count;

                terms[i][j].transform.Find("Coefficient").GetComponent<TMP_InputField>().Select();

            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                termIndex = (termIndex + 1) % (terms.Count * terms[0].Count);
                i = termIndex / terms[0].Count;
                j = termIndex % terms[0].Count;


                terms[i][j].transform.Find("Coefficient").GetComponent<TMP_InputField>().Select();
            }
        }
    }

    public void Init()
    {
        // destroy terms, until sticky feature
        if (terms != null && terms.Count > 0)
        {
            terms.ForEach(list =>
            {
                list.ForEach(term => Destroy(term));
                list.Clear();
            });
            terms = null;
        }
        system = new List<List<float>>();  // sticky feature?
        terms = new List<List<GameObject>>();


        for (int i = 0; i < eqs; i++)
        {
            terms.Add(new List<GameObject>());
            system.Add(new List<float>());
            for (int j = 0; j < vars + 1 /* add constant coefficient */; j++)
            {
                if (equalsSigns.Count < eqs)
                {
                    equalsSigns.Add(addEqualsSign());
                }
                else if (equalsSigns.Count > eqs)
                {
                    Destroy(equalsSigns[equalsSigns.Count - 1]);
                    equalsSigns.RemoveAt(equalsSigns.Count - 1); 
                }
                // use equals sign as anchor point to place equations
                terms[i].Add(AddTerm(i, j));
            } 
        }
        PopulateTermsForTesting();  // <=================================== Testing
        MakeMatrix();
    }
    GameObject addEqualsSign()
    {
        GameObject nextEqualsSign;
        GameObject lastEqualsSign = equalsSigns[equalsSigns.Count - 1];
        Vector3 lastEqualsSignPosition = lastEqualsSign.transform.position;
        nextEqualsSign = Instantiate(equalsSign, lastEqualsSignPosition + Vector3.down * 75, Quaternion.identity);
        float canvasScaleFactor = canvas.GetComponent<Canvas>().scaleFactor;
        nextEqualsSign.GetComponent<RectTransform>().localScale = new Vector3(canvasScaleFactor,canvasScaleFactor,canvasScaleFactor);

        nextEqualsSign.transform.SetParent(GameObject.Find("Canvas").transform);
        return nextEqualsSign;
    }

    GameObject AddTerm(int eq, int pos)
    {
        Vector2 equalsSignSize = equalsSigns[eq].GetComponent<RectTransform>().sizeDelta;
        Vector3 nextTermPosition;


        //GameObject nextTerm = Instantiate(term, nextTermPosition, Quaternion.identity);
        Debug.Log(" ===== "+equalsSigns[eq].transform);
        GameObject nextTerm = Instantiate(term, equalsSigns[eq].transform);
        //nextTerm.transform.localPosition = Vector3.zero;
        RectTransform coefficientRT = nextTerm.transform.Find("Coefficient").GetComponent<RectTransform>();
        coefficientRT.sizeDelta = new Vector2(50 * 2, 50);
        Vector2 coefficientSize = coefficientRT.sizeDelta;
        if (pos == vars)
        {
            nextTermPosition =  Vector3.right * 2 * coefficientSize.x;
            //nextTermPosition =  Vector3.right * 2 * equalsSignSize.x;
        }
        else nextTermPosition = Vector3.left * 2 * coefficientSize.x * (vars - pos);
        //else nextTermPosition = Vector3.left * 2 * equalsSignSize.x * (vars - pos);
        nextTerm.transform.localPosition = nextTermPosition;
        TextMeshProUGUI variable = nextTerm.transform.Find("Variable").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI placeholder = nextTerm.transform.Find("Coefficient").GetChild(0).Find("Placeholder").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI coefficient = nextTerm.transform.Find("Coefficient").GetChild(0).Find("Text").gameObject.GetComponent<TextMeshProUGUI>();


        coefficient.text = coefficientLabels[pos];
        nextTerm.transform.Find("Coefficient").GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.DecimalNumber;
        placeholder.text = coefficientLabels[pos];
        nextTerm.transform.Find("Coefficient").gameObject.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { MakeMatrix();});
        if (pos == vars)
        {
            variable.text = "";
        }
        else
        {
            // vars> 2
            variable.text = variableLabels[pos];
            if (vars - pos > 1 ) { variable.text += " +"; }
        }

        // set parent
        //nextTerm.transform.SetParent(GameObject.Find("Canvas").transform);
        return nextTerm;
    }

    void MakeMatrix()               
    {
        if (matrixElements == null) matrixElements = new List<GameObject>();
        else
        {
            matrixElements.ForEach(go => Destroy(go));
            matrixElements.Clear();
        }
        for (int i = 0; i < terms.Count; i++) {
            GameObject currentMatrixElement = GetRowHandler(i);
            currentMatrixElement.transform.position += Vector3.down * 75 * i;
            currentMatrixElement.GetComponent<RowHandler>().ResetRow += RebuildRow; //  ============================ subscribe method to event
            for(int j = 0; j < terms[i].Count; j++)
            {
                currentMatrixElement.transform.Find("Row").GetChild(j).gameObject.GetComponent<Text>().text = terms[i][j].transform.Find("Coefficient").GetChild(0).Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text;
            }
            matrixElements.Add(currentMatrixElement);
        }
    }
    void LogTerms()
    {
        int i = 0;
        terms.ForEach( list => list.ForEach( term => Debug.Log($"{++i} |{term.transform.Find("Coefficient").GetChild(0).Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text}|")));
    }
    void IncreaseScalar() {
        // increase by one the text value of scalar input
        Debug.Log("Scalar goes up by one!");
        Debug.Log(this);
    }
    GameObject GetRowHandler(int i) {
        GameObject currentMatrixElement;
        currentMatrixElement = Instantiate(rowHandler, canvas.transform);
        currentMatrixElement.name = $"Row Handler {i}";
        currentMatrixElement.GetComponent<RowHandler>().rowNumber = i;
        currentMatrixElement.GetComponent<RowHandler>().BuildRow();

        RectTransform currRT = currentMatrixElement.GetComponent<RectTransform>();
        GameObject leftBodySegment, rightBodySegment;
        leftBodySegment = Instantiate(bodySegment, currentMatrixElement.transform);
        leftBodySegment.name = "Left Segment";
        int currLength = currentMatrixElement.transform.Find("Row").childCount;
        leftBodySegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(0).position + Vector3.left * currentMatrixElement.transform.Find("Row").GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        rightBodySegment = Instantiate(bodySegment, currentMatrixElement.transform);
        rightBodySegment.name = "Right Segment";
        rightBodySegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(currLength -1).position + Vector3.right * 10;

        if (i == 0)
        {
            GameObject leftHeadSegment, rightHeadSegment;
            leftHeadSegment = Instantiate(endSegment, currentMatrixElement.transform);
            leftHeadSegment.name = "Head Segment Left";
            leftHeadSegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(0).position + Vector3.left * currentMatrixElement.transform.Find("Row").GetChild(0).GetComponent<RectTransform>().sizeDelta.x
                + Vector3.up * 47 + Vector3.right * leftBodySegment.GetComponent<RectTransform>().sizeDelta.x;
            rightHeadSegment = Instantiate(endSegment, currentMatrixElement.transform);
            rightHeadSegment.name = "Head Segment Right";
            rightHeadSegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(currLength -1).position + Vector3.up * 47 + Vector3.right * 5;
        }
        if (i == eqs-1)
        {
            GameObject leftTailSegment, rightTailSegment;
            leftTailSegment = Instantiate(endSegment, currentMatrixElement.transform);
            leftTailSegment.name = "Tail Segment Left";
            leftTailSegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(0).position + Vector3.left * currentMatrixElement.transform.Find("Row").GetChild(0).GetComponent<RectTransform>().sizeDelta.x
                + Vector3.down * 48 + Vector3.right * leftBodySegment.GetComponent<RectTransform>().sizeDelta.x;
            rightTailSegment = Instantiate(endSegment, currentMatrixElement.transform);
            rightTailSegment.name = "Tail Segment Right";
            rightTailSegment.transform.position = currentMatrixElement.transform.Find("Row").GetChild(currLength -1).position + Vector3.down * 48 + Vector3.right * 5;

        }
        return currentMatrixElement;   
    }
    void RebuildRow(int rowNumber) {
        // select row object
        Text[] rowTexts = matrixElements[rowNumber].transform.Find("Row").gameObject.GetComponentsInChildren<Text>();
        // set row values
        if (rowTexts.Length != terms[rowNumber].Count)
        {
            Debug.Log("Length mismatch !!!!");
            return;
        }
        for(int j = 0; j < terms[rowNumber].Count; j++)
            {
               rowTexts[j].text = terms[rowNumber][j].transform.Find("Coefficient").GetChild(0).Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text;
            }
        
    }
}
