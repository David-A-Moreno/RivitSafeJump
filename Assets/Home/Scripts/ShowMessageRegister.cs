using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowMessageRegister : MonoBehaviour
{
    [SerializeField] private TMP_Text TextShowMessage;
    public string messageValue;

    // Start is called before the first frame update
    void Start()
    {
        messageValue= "";
        TextShowMessage.text = messageValue;
    }

    // Update is called once per frame
    void Update()
    {
        TextShowMessage.text = messageValue;
    }

    public void UpdateText(string msg)
    {
        messageValue = msg;
        TextShowMessage.text = msg;
    }
}
