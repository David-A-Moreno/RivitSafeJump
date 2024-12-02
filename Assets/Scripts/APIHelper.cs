using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using RestSharp;
using System.Text;

public class APIHelper : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:8091";
    private ShowMessage showMessageLogin;
    private ShowMessageRegister showMessageRegister;
    private MorePanel _morePanel;
    public MorePanel MorePanel { set { _morePanel = value; } }
    private Token _token;
    public Token Token { get { return _token; } }
    public static event Action<bool> UserAccess = delegate { };
    //public bool IsFirstSession { get { return _isFirstSession; } set { _isFirstSession = value; } }
    //private bool _isFirstSession;

    [Serializable]
    private class Access
    {
        public string Message { get; set; }
    }

    private void Start() {
        //_isFirstSession = true;
        _token = new Token(BASE_URL);
        _token.ExistAccessToken = false;
        showMessageLogin = FindObjectOfType<ShowMessage>();
        //showMessageRegister = FindObjectOfType<ShowMessageRegister>();
    } 

    void Awake()
    {
        /*
        ** Comentado el 22/11/2022
        * Motivo: Al cerrar sesión e iniciar sesión de nuevo, lanzaba la excepción:
        MissingReferenceException: The object of type 'APIHelper' has been destroyed but you are still trying to access it.
        **
        
        GameObject[] objs = GameObject.FindGameObjectsWithTag("APIHelper");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        */
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator Login(string username, string password)
    {
        yield return AuthUser(username, password);
    }

    public void RegisterUser(string username, string password, string email)
    {
        Debug.Log("The user registration functionality should only be performed from the web application.");
        //StartCoroutine(RegisterUserData(username, password, email));
    }

    public void Sync(string json)
    {
        SyncronizeData(json);
    }

    private void ConfirmAccess(bool confirmed)
    {
        if (confirmed)
            UserAccess.Invoke(false);
        else
        {
            UserAccess.Invoke(true);
            Debug.Log("Access denied");
        }
    }

    private IEnumerator AuthUser(string username, string password)
    {
        const string URL = BASE_URL + "/oauth/token";
        Dictionary<string, string> body = new Dictionary<string, string>();
        //Fill key and value
        body.Add("grant_type", "password");
        body.Add("username", username);
        body.Add("password", password);

        UnityWebRequest www = UnityWebRequest.Post(URL, body);
        www.SetRequestHeader("Authorization", "Basic d2ViLWNsaWVudDp3M2JjbDEzbjc=");
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            UpdateShowMessageLogin("Los datos son incorrectos");
        }
        else
        {
            /* Content json response:
                access_token
                token_type
                refresh_token
                expires_in
                scope
            */
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(www.downloadHandler.text);
            _token.TokenData = data;
            _token.ExistAccessToken = true;
            ConfirmAccess(true);
            UpdateShowMessageLogin("");

            // Se guarda el refresk token en las preferencias
            _token.SetTokenPlayerPrefs();

            //Update user lastconnection
            Debug.Log("Updating last connection");
            StartCoroutine(UpdateUserLastConnection(username));
        }
    }

    private IEnumerator UpdateUserLastConnection(string username)
    {
        const string URL = BASE_URL + "/api/user/lastconnection";
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("username", username);

        string tokenData = _token.TokenType + _token.TokenData["access_token"];
        UnityWebRequest www = UnityWebRequest.Post(URL, body);
        www.SetRequestHeader("Authorization", tokenData);

        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log("Error: " + www.error);
        else
            Debug.Log("Success: " + www.downloadHandler.text);
    }
    
    /*
    This method must not be used
    The user registration functionality should be only performed from the web application.
    */
    private IEnumerator RegisterUserData(string username, string password, string email)
    {
        const string URL = BASE_URL + "/api/user/register";
        string msg = "";
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("username", username);
        body.Add("password", password);
        body.Add("email", email);

        UnityWebRequest www = UnityWebRequest.Post(URL, body);
        yield return www.SendWebRequest();
        msg = www.downloadHandler.text;
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Usuario no registrado");
        }
        else
        {
            Debug.Log("Usuario registrado");
            //IsFirstSession = true;
            StartCoroutine(Login(username, password));
        }
        showMessageRegister.UpdateText(msg);
    }

    private void SyncronizeData(string json)
    {
        _token.SetBASE_URL = BASE_URL;
        StartCoroutine(SetSyncronizeData(json));
        /*
        if(_isFirstSession) //Si es la primera sincronización de la sesión
        {
            StartCoroutine(SetSyncronizeData(json));
        }
        else //Si ha sincronizado más de una vez durante la misma sesión
        {
            StartCoroutine(UpdateSyncronizeData(json, idSession));
        }
        */
    }

    private IEnumerator SetSyncronizeData(string json)
    {
        const string URL = BASE_URL + "/api/metric/json";
        string msg = "";
        //string newJson = json.Remove(json.Length - 1, 1);
        //newJson += ",\"PlaySessionsSaved\":" + _playSessionsSaved + "}";

        UnityWebRequest www = RequestSyncronizeData(URL, json);

        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            //msg = "Los datos no han sido sincronizados";
            string responseText = www.downloadHandler.text;
            Debug.Log("error: " + responseText);

            yield return _token.GenerateRefreshToken(responseText);
            msg = RefreshTokenForSyncronizeData(json, responseText);
        }
        else
        {
            msg = "Datos sincronizados exitosamente";

            if(_morePanel != null) //MorePanel es null cuando se guardan sesiones al iniciar la sesión en Login
                _morePanel.ResetPlayerSessions();

            /*
            try
            {
                int idSession = int.Parse(www.downloadHandler.text);

                if(_morePanel != null) //MorePanel es null cuando se guardan sesiones al iniciar la sesión en Login
                {
                    _isFirstSession = false;
                    _morePanel.ResetPlayerSessions();
                }
                else
                    _isFirstSession = true;
                
            }
            catch(FormatException fe)
            {
                Debug.Log("Error en la conversión del idSession");
            }
            */
        }
        Debug.Log("Sync Success: " + www.downloadHandler.text);
        UpdateShowMessageMorePanel(msg);
    }

    /*
    private IEnumerator UpdateSyncronizeData(string json, int idSession)
    {
        const string URL = BASE_URL + "/api/metric/update";
        string msg = "";

        string newJson = json.Remove(json.Length - 1, 1);
        newJson += ",\"SessionId\":" + idSession + "}";
        //newJson += ",\"SessionId\":" + idSession;
        //newJson += ",\"PlaySessionsSaved\":" + _playSessionsSaved + "}";

        UnityWebRequest www = RequestSyncronizeData(URL, newJson);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            //msg = "Los datos no han sido sincronizados";
            string responseText = www.downloadHandler.text;
            Debug.Log("Update error: " + responseText);
            yield return token.GenerateRefreshToken(responseText);
            RefreshTokenForSyncronizeData(json, responseText);
        }
        else
        {
            msg = "Datos sincronizados exitosamente";

            if(_morePanel != null)
            {
                _isFirstSession = false;
                _morePanel.ResetPlayerSessions(idSession);
            }
            else
                _isFirstSession = true;

            Debug.Log("Update Success: " + www.downloadHandler.text);
        }
        UpdateShowMessageMorePanel(msg);
    }
    */

    private UnityWebRequest RequestSyncronizeData(string URL, string json)
    {
        if(_token.TokenData == null)
            LoadTokenPlayerPrefs(); //Carga los datos del último token guardado

        string tokenData = _token.TokenType + _token.TokenData["access_token"];

        UnityWebRequest www = new UnityWebRequest(URL, "POST");
        www.SetRequestHeader("Authorization", tokenData);
        www.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        return www;
    }

    private string RefreshTokenForSyncronizeData(string json, string msg = "")
    {
        string msgResponse = "";

        if(_token.TokenRefreshed)
        {
            if(_token.SignOut)
            {
                //La última sincronización, antes de hacer logout, se guarda como una actualización de la última sesión
                //Ya no se guarda como una actualización (marzo 2023)
                //_isFirstSession = false;
                SyncronizeData(json);
                _morePanel.SignOutWithoutSync();
            }
            else
            {
                //_isFirstSession = true;
                SyncronizeData(json);
            }
        }
        else
        {
            Debug.Log("Error SyncronizeData not saved: " + msg);
            msgResponse = "Los datos no pueden ser sincronizados, intente nuevamente en otro momento";
        }

        return msgResponse;
    }

    public void UpdateShowMessageLogin(string msg) {
        showMessageLogin.messageValue = msg;
    }

    public void UpdateShowMessageMorePanel(string msg)
    {
        if(_morePanel != null)
            _morePanel.UpdateShowMessage(msg);
        else
            Debug.Log("The morePanel object is null");
    }

    public IEnumerator WaitExucutionForSeconds(int time = 3)
    {
        // suspend execution for some seconds
        yield return new WaitForSeconds(time);
        UpdateShowMessageMorePanel("");
    }

    public static implicit operator APIHelper(GameObject v)
    {
        throw new NotImplementedException();
    }

   public void LoadTokenPlayerPrefs()
   {
        string tokenPlayerPrefs = PlayerPrefs.GetString("token");

        if(tokenPlayerPrefs != "")
            _token.TokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenPlayerPrefs);
        else
            UpdateShowMessageMorePanel("No se pueden sincronizar los datos, intente iniciar sesión nuevamente");
   }
}
