using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _message;
    private static MessageBox instance ;
    
    public static MessageBox Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<MessageBox>("MessageBox"), FindObjectOfType<Canvas>().transform);
            }
            return instance;
        }
    }
    
    public enum Action
    {
        Missiled,
        Eaten
    }

    public void CreateText(string owner , string owned , Action actionType )
    {
        switch(actionType)
        {
            case Action.Missiled:
                _message.text = owner + " hit " + owned + " with a missile.";
                break;
            case Action.Eaten:
                _message.text = owner + " killed " + owned + ".";
                break;
        }
        //Instantiate text to messagebox if null debug.
        if (_message != null)
        {
            TextMeshProUGUI txtObject = Instantiate(_message, transform.Find("Content").transform);
            txtObject.transform.SetAsLastSibling();
            txtObject.gameObject.SetActive(true);
            Destroy(txtObject.gameObject, 10f);
        } 
        else Debug.Log("nothing to add to messagebox");
    }

    public void Initialize()
    {
        //init - do nothing
    }

}

