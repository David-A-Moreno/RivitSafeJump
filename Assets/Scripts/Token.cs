using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

public class Token : MonoBehaviour
{
    private string BASE_URL;
    public string SetBASE_URL { set{ BASE_URL = value; } }
    private const int MAX_REFRESH_COUNT = 100; //El token tiene una duración de 12 horas
    private const string TOKEN_TYPE = "Bearer ";
    public string TokenType { get { return TOKEN_TYPE; }}
    private int refreshTokenCount;
    private bool _tokenRefreshed;
    public bool TokenRefreshed { get { return _tokenRefreshed; } }
    private bool _existAccessToken;
    public bool ExistAccessToken { get { return _existAccessToken; } set { _existAccessToken = value; } }
    private bool _signOut; //Esta variable permite saber si se retorna al login cuando se ha alcanzado el MAX REFRESH
    public bool SignOut { get { return _signOut; } }
    private Dictionary<string, string> _tokenData;
    public Dictionary<string, string> TokenData { get { return _tokenData; } set { _tokenData = value; } }

    // Start is called before the first frame update
    void Start()
    {
        refreshTokenCount = 0;
        _tokenRefreshed = false;
    }

    public Token(string baseURL)
    {
        BASE_URL = baseURL;
    }

    public IEnumerator GenerateRefreshToken(string msg)
    {
        _tokenRefreshed = false;
        yield return RequestRefreshToken(msg);
    }

    private IEnumerator RequestRefreshToken(string msg)
    {
        if(IsTokenRefreshed(msg))
        {
            Debug.Log("access token: " + _tokenData["refresh_token"]);
            string URL = BASE_URL + "/oauth/token";
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("grant_type", "refresh_token");
            body.Add("refresh_token", _tokenData["refresh_token"]);

            UnityWebRequest www = UnityWebRequest.Post(URL, body);
            www.SetRequestHeader("Authorization", "Basic d2ViLWNsaWVudDp3M2JjbDEzbjc=");
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("The token has not been refreshed: " + www.result);
            }
            else
            {
                Debug.Log("Token refreshed");
                _tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(www.downloadHandler.text);
                SetTokenPlayerPrefs();

                refreshTokenCount += 1; //Acumula las veces que se refresca un token
                _tokenRefreshed = true;

                Debug.Log("token refresh: " + _tokenData["access_token"]);
            }
        }
        else
            Debug.Log("The token cannot be refreshed");
    }

    private bool IsTokenRefreshed(string msg)
    {
        bool tokenExpired = IsTokenExpired(msg);
        bool refreshed = false;

        if(tokenExpired && refreshTokenCount < MAX_REFRESH_COUNT)
            refreshed = true;
        else if(tokenExpired && refreshTokenCount >= MAX_REFRESH_COUNT)
        {
            refreshed = true;
            _signOut = true; //Estado para retornar al login
        }

        return refreshed;
    }

    private bool IsTokenExpired(string msg)
    {
        bool tokenExpired = false;

        try
        {
            char firstChar = msg[0];
            char lastChar = msg[msg.Length - 1];

            if(firstChar == '{' && lastChar == '}') //Verifica si es un json
            {
                //Intenta obtener un json del tipo {"error":"invalid_token", "error description": "The access token expired"}
                Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg);
                if(json["error"] == "invalid_token")
                {
                    Debug.Log("El token ha expirado");
                    tokenExpired = true;
                }
            }
        }
        catch(KeyNotFoundException ke)
        {
            Debug.Log("The message is not a valid json token return type");
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        return tokenExpired;
    }

    public void SetTokenPlayerPrefs()
    {
        string jsonToken = JsonConvert.SerializeObject(_tokenData);
        PlayerPrefs.SetString("token", jsonToken);
        PlayerPrefs.Save();
    }
}
