using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button increaseVarsButton, increaseEquationsButton, decreaseVarsButton, decreaseEquationsButton;

    SetupEquations se;

    // Start is called before the first frame update
    void Start()
    {
        se = GetComponent<SetupEquations>();

        increaseVarsButton.onClick.AddListener(HandleIncreaseVariables);
        increaseEquationsButton.onClick.AddListener(HandleIncreaseEquations);
        decreaseVarsButton.onClick.AddListener(HandleDecreaseVariables);
        decreaseEquationsButton.onClick.AddListener(HandleDecreaseEquations);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandleIncreaseVariables() {
        if (se.vars < 4)
        { 
            Debug.Log("increased variables");
            se.vars++;
            Debug.Log(se.vars);
            se.Init();
        }
    }
    public void HandleIncreaseEquations() {
        if (se.eqs < 4) { 
            se.eqs++;
            Debug.Log("increased equations");
            Debug.Log("# of eqs: "+se.eqs);
            se.Init();
        }
    }
    public void HandleDecreaseVariables() {
        if( se.vars > 1)
        { 
            Debug.Log("decreased variables");
            se.vars--;
            Debug.Log(se.vars);
            se.Init();
        }
    }
    public void HandleDecreaseEquations() {
        if(se.eqs > 1)
        { 
            se.eqs--;
            Debug.Log("decreased equations");
            Debug.Log(se.eqs);
            se.Init();
        }
    }
}
