using System.Net.Security;
using System.Linq.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
    [SerializeField] private APIHelper apiHelper;
    [SerializeField] private UserDataManager userDataManager;
    private ShowMessage showMessageLogin;
    private ShowMessageRegister showMessageRegister;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMP_InputField userTextFieldLogin;
    [SerializeField] private TMP_InputField passwordTextFieldLogin;
    [SerializeField] private TMP_InputField userTextFieldRegister;
    [SerializeField] private TMP_InputField passwordTextFieldRegister;
    [SerializeField] private TMP_InputField emailTextFieldRegister;
    [SerializeField] private TMP_InputField confirmPasswordTextFieldRegister;
    [SerializeField] private TMP_Text textShowMessageRegister;
    [SerializeField] private TMP_Text textShowMessageLogin;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    private  bool enableRegisterPanel = false;


    // Start is called before the first frame update
    void Start()
    {
        APIHelper.UserAccess += ConfirmAccess;
        apiHelper = FindObjectOfType<APIHelper>();
        showMessageLogin = FindObjectOfType<ShowMessage>();
        showMessageRegister = FindObjectOfType<ShowMessageRegister>();
        passwordTextFieldLogin.contentType = TMP_InputField.ContentType.Password;
        passwordTextFieldRegister.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordTextFieldRegister.contentType = TMP_InputField.ContentType.Password;

        IsUserLogged();
    }

    private void OnDestroy()
    {
        APIHelper.UserAccess -= ConfirmAccess;
    }

    public void IsUserLogged()
    {
        string username = userDataManager.PersonalData.PlayerName;
        string password = userDataManager.PersonalData.Password;

        if(username != null && username != "" && password != null && password != "")
        {
            ConfirmAccess(true);
            //userDataManager.RegisterEmailAndPassword(username, Encryption.Encrypt(password));
        }
    }

    public void Auth()
    {
        StartCoroutine(VerifyUser());
    }

    public IEnumerator VerifyUser()
    {
        string username = userTextFieldLogin.text;
        string password = passwordTextFieldLogin.text;
        bool checkOldCredentials = true;
        
        if(username != "" && password != "")
        {
            yield return apiHelper.Login(username, password);

            if(apiHelper.Token.ExistAccessToken)
            {
                if (userDataManager.PersonalData.PlaySessionsData.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(userDataManager.PersonalData);
                    //apiHelper.IsFirstSession = isUserFirstSession();
                    apiHelper.Sync(jsonString);
                }
                userDataManager.RegisterEmailAndPassword(username, Encryption.Encrypt(password));
                checkOldCredentials = false;
            }
            else if(checkOldCredentials)
            {
                string usernameSaved = userDataManager.PersonalData.PlayerName;
                string passwordSaved = userDataManager.PersonalData.Password;

                if(usernameSaved != null && usernameSaved == username && passwordSaved != null && Encryption.Decrypt(passwordSaved) == password)
                {
                    ConfirmAccess(true);
                    apiHelper.LoadTokenPlayerPrefs();
                    //apiHelper.IsFirstSession = isUserFirstSession();
                }
                else
                {
                    UpdateShowMessageLogin("Debe ingresar los mismos datos de la sesión anterior");
                }
            }
            else
                UpdateShowMessageLogin("No se puede iniciar sesión");
        }
        else
            UpdateShowMessageLogin("Ingrese los datos");
    }

    /*
    private bool isUserFirstSession()
    {
        return userDataManager.PersonalData.IsUpdatingSession ? false : true;
    }
    */

    public void VerifyUser2()
    {
        string username = userTextFieldLogin.text;
        string password = passwordTextFieldLogin.text;

        if(username != "" && password != "")
        {
            if(username == "admin" && password == "admin")
            {
                ConfirmAccess(true);
                userDataManager.RegisterEmailAndPassword(username, Encryption.Encrypt(password));
            }
            else
            {
                UpdateShowMessageLogin("Usuario y/o contraseña incorrectos");
            }
        }
        else
            UpdateShowMessageLogin("Ingrese los datos");
    }

    public void RegisterNewUser()
    {
        string username = userTextFieldRegister.text;
        string password = passwordTextFieldRegister.text;
        string confirmPassword = confirmPasswordTextFieldRegister.text;
        string email = emailTextFieldRegister.text;

        if(checkRegisterFields(username, password, confirmPassword, email))
        {
            apiHelper.RegisterUser(username, password, email);
            userDataManager.RegisterEmailAndPassword(username, password);
        }
    }

    private void ConfirmAccess(bool access)
    {
        SceneManager.LoadScene("Home");
    }

    public void ChangePanel()
    {
        if (!enableRegisterPanel)
        {
            UpdateShowMessageLogin("");
            CleanLoginForm();
            CreateNewAccount();
            enableRegisterPanel = true;
        }
        else
        {
            UpdateShowMessageRegister("");
            CleanRegisterForm();
            LoginWithAccount();
            enableRegisterPanel = false;
        }
    }

    public void CreateNewAccount()
    {
        loginPanel.gameObject.SetActive(false);
        registerPanel.gameObject.SetActive(true);
    }

    public void LoginWithAccount()
    {
        loginPanel.gameObject.SetActive(true);
        registerPanel.gameObject.SetActive(false);
    }

    private bool checkRegisterFields(string username, string password, string confirmPassword, string email)
    {
        bool isValid = false;

        if(username != "" && password != "" && confirmPassword != "" && email != "")
        {
            if(password == confirmPassword)
            {
                Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");
                if(validateEmailRegex.IsMatch(email) || email == username)
                {
                    UpdateShowMessageRegister("");
                    isValid = true;
                }
                else
                    UpdateShowMessageRegister("El correo no es válido");
            }
            else
                UpdateShowMessageRegister("Las contraseñas no coinciden");
        }
        else
            UpdateShowMessageRegister("Ingrese todos los datos");
        
        return isValid;
    }

    private void UpdateShowMessageLogin(string msg)
    {
        showMessageLogin.messageValue = msg;
    }

    private void UpdateShowMessageRegister(string msg) {
        showMessageRegister.UpdateText(msg);
    }

    private void CleanLoginForm()
    {
        userTextFieldLogin.text = "";
        passwordTextFieldLogin.text = "";
    }

    private void CleanRegisterForm()
    {
        userTextFieldRegister.text = "";
        passwordTextFieldRegister.text = "";
        confirmPasswordTextFieldRegister.text = "";
        emailTextFieldRegister.text = "";
    }
}
