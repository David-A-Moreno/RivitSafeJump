using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Debug = UnityEngine.Debug;

public class MorePanel : MonoBehaviour
{
    private UserDataManager _userDataManager;
    private APIHelper apiHelper;
    private ShowMessage showMessage;
    private bool updateBeforeSignOut;
    [Inject]
    public void Init(UserDataManager dataManager)
    {
        _userDataManager = dataManager;
    }

    private void Start()
    {
        apiHelper = FindObjectOfType<APIHelper>();
        showMessage = GetComponent<ShowMessage>();
        UpdateShowMessage("");
        apiHelper.MorePanel = this;
        updateBeforeSignOut = true;
    }
    
    public void UpdateData()
    {
        string msg = "";
        UpdateShowMessage(msg);

        string jsonString = PlayerPrefs.GetString("UserData");
        int countPlaySessionsData = _userDataManager.PersonalData.PlaySessionsData.Count;

        if(countPlaySessionsData == null || countPlaySessionsData == 0)
        {
            msg = "No hay sesiones de juego para sincronizar";
            UpdateShowMessage(msg);
        }
        else
        {
            apiHelper.Sync(jsonString);

            //if(!isSync)
            //    UpdateShowMessage("Los datos no pueden ser sincronizados");
        }
        
        StartCoroutine(apiHelper.WaitExucutionForSeconds(30)); //Para limpiar el showMessage
    }

    public void SignOut()
    {
        if(updateBeforeSignOut)
            UpdateData();

        updateBeforeSignOut = true;
        apiHelper.Token.ExistAccessToken = false;
        _userDataManager.DeleteUsernameAndPassword(); //Detener login automático iniciando los valores como vacíos.
        
        UpdateShowMessage("");
        SceneManager.LoadScene("Login");
        //_userDataManager.DeleteData();
    }

    public void SignOutWithoutSync()
    {
        updateBeforeSignOut = false;
        SignOut();
    }

    public void UpdateShowMessage(string msg)
    {
        showMessage.messageValue = msg;
    }

    public void ResetPlayerSessions()
    {
        _userDataManager.ResetPlayerSessions();
    }
}
