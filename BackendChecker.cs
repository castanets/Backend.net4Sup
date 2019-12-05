using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using static BackEnd.BackendAsyncClass;

// 작성자: 최갱생(chlrodtod@gmail.com)

public class BackendChecker : Singleton<BackendChecker>
{
    #region UNITY_EVENT_FUNCTION
    // List<> 의 Count 가 0 이상인지 검사하고, List<> 내부에 BackendData 가 존재한다면 콜백이 실행될 때 까지 체크합니다.
    private void Update()
    {
        Async_WithSaveToken_DataListCheck();
        Async_WithoutSaveToken_DataListCheck();
        Async_RowDataCheck();
    }
    #endregion

    #region VARIABLES
    private int CurrentExecuteAsyncCount = 0;
    public int GetCurExeAsyncCount
    {
        get { return CurrentExecuteAsyncCount; }
        set
        {
            CurrentExecuteAsyncCount = value;
        }
    }

    /// <summary>
    /// 비동기 함수의 정보와, 해당 함수의 콜백을 담고 있습니다.
    /// </summary>
    public class BackendData
    {
        public string CallingName = null; // List<> 에 담긴 각 함수들의 ID 로 쓰입니다.
                                          // 같은 내용의 비동기함수를 동시다발적으로 호출할 경우가 있나 싶습니다. 
                                          // 만약 그런 경우라면 CallingName으로 인하여 전체 동작에 문제가 발생합니다.
        public string StatusCode = null; // BRO의 상태코드를 왜 따로 빼놨는지 잘 모르겠습니다. ;;
        public bool CallbackComplete = false; // 비동기함수의 동작이 끝나고 콜백을 하기 위해 true 로 변경됩니다.
        public bool ClearMe = false; // 이 것이 true 가 되면 List<>의 삭제대상으로 전환됩니다.
        public BackendReturnObject BRO = null;
        public System.Action SuccessCallback = null; // 비동기함수가 성공시 진행됩니다. 호출쪽에서 설정합니다.
        public System.Action FailCallback = null; // 비동기함수가 실패시 진행됩니다. 호출쪽에서 설정합니다.
        public System.Action<LitJson.JsonData> HasValueCallback = null;

        public BackendData SetCallingName(string CallingName)
        {
            this.CallingName = CallingName;
            return this;
        }
        public BackendData SetStatusCode(string StatusCode)
        {
            this.StatusCode = StatusCode;
            return this;
        }
        public BackendData SetCallbackComplete(bool CallbackComplete)
        {
            this.CallbackComplete = CallbackComplete;
            return this;
        }
        public BackendData SetBRO(BackendReturnObject BRO)
        {
            this.BRO = BRO;
            return this;
        }
        public BackendData SetSuccessCallback(System.Action SuccessCallback)
        {
            this.SuccessCallback = SuccessCallback;
            return this;
        }
        public BackendData SetFailCallback(System.Action FailCallback)
        {
            this.FailCallback = FailCallback;
            return this;
        }
        public BackendData SetHasValueCallback(System.Action<LitJson.JsonData> HasValueCallback)
        {
            this.HasValueCallback = HasValueCallback;
            return this;
        }
    }

    private List<BackendData> Async_WithSaveToken_DataList = new List<BackendData>(); // 데이터 리스트.
    private List<BackendData> Async_WithoutSaveToken_DataList = new List<BackendData>(); // 각 데이터 리스트는
    private List<BackendData> Async_RowCheck_DataList = new List<BackendData>(); // 비동기함수가 실행을 마칠 때 까지 정보를 저장합니다.
    #endregion

    #region Async_WithSaveToken
    public void BroCheckerAsync_WithSaveToken(string CallingName, Func<BackendReturnObject> BackendFunc, 
        System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithSaveToken_DataList.Count; i++)
            {
                if (Async_WithSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithSaveToken<T1>(string CallingName, Func<T1, BackendReturnObject> BackendFunc, 
        T1 GT, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithSaveToken_DataList.Count; i++)
            {
                if (Async_WithSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithSaveToken<T1, T2>(string CallingName, Func<T1, T2, BackendReturnObject> BackendFunc, 
        T1 GT1, T2 GT2, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT1, GT2, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithSaveToken_DataList.Count; i++)
            {
                if (Async_WithSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithSaveToken<T1, T2, T3>(string CallingName, Func<T1, T2, T3, BackendReturnObject> BackendFunc, 
        T1 GT1, T2 GT2, T3 GT3, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT1, GT2, GT3, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithSaveToken_DataList.Count; i++)
            {
                if (Async_WithSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }

    private void Async_WithSaveToken_DataListCheck()
    {
        if (Async_WithSaveToken_DataList.Count != 0)
        {
            // 콜백을 실행함.
            for (int i = 0; i < Async_WithSaveToken_DataList.Count; i++)
            {
                if (true == Async_WithSaveToken_DataList[i].CallbackComplete)
                {
                    if (null != Async_WithSaveToken_DataList[i].BRO)
                    {
                        if (false == Async_WithSaveToken_DataList[i].ClearMe)
                        {
                            Async_WithSaveToken_DataList[i].ClearMe = true;

                            BroChecker(Async_WithSaveToken_DataList[i].CallingName, Backend.BMember.SaveToken(Async_WithSaveToken_DataList[i].BRO),
                            () =>
                            {
                                //Debug.Log(Async_WithSaveToken_DataList[i].CallingName + " Success!");

                                // 최종콜백
                                Async_WithSaveToken_DataList[i].SuccessCallback?.Invoke();
                            },
                            () =>
                            {
                                //Debug.Log(Async_WithSaveToken_DataList[i].CallingName + " 실패: /" + Async_WithSaveToken_DataList[i].StatusCode + " 콜백 후 종료됨!");

                                // 최종콜백
                                Async_WithSaveToken_DataList[i].FailCallback?.Invoke();
                            });
                        }
                    }
                    else
                    {
                        //Debug.Log(Async_WithSaveToken_DataList[i].CallingName + " 가 NULL BRO 를 리턴함! 코드 확인할 것.");
                    }
                }
            }

            // 실행된 콜백을 삭제함. (앞에서부터 하면 Remove()에 의해서 사이즈가 달라지므로 참조오류가 발생함. 뒤에서부터 삭제하며 내려옴)
            for (int i = Async_WithSaveToken_DataList.Count - 1; i >= 0; i--)
            {
                if (true == Async_WithSaveToken_DataList[i].ClearMe)
                {
                    GetCurExeAsyncCount--;

                    Async_WithSaveToken_DataList.Remove(Async_WithSaveToken_DataList[i]);
                }
            }
        }
    }
    #endregion

    #region Async_WithoutSaveToken
    public void BroCheckerAsync_WithoutSaveToken(string CallingName, Func<BackendReturnObject> BackendFunc,
        System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithoutSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithoutSaveToken_DataList.Count; i++)
            {
                if (Async_WithoutSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithoutSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithoutSaveToken<T1>(string CallingName, Func<T1, BackendReturnObject> BackendFunc,
        T1 GT, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithoutSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithoutSaveToken_DataList.Count; i++)
            {
                if (Async_WithoutSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithoutSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithoutSaveToken<T1, T2>(string CallingName, Func<T1, T2, BackendReturnObject> BackendFunc,
        T1 GT1, T2 GT2, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithoutSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT1, GT2, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithoutSaveToken_DataList.Count; i++)
            {
                if (Async_WithoutSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithoutSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void BroCheckerAsync_WithoutSaveToken<T1, T2, T3>(string CallingName, Func<T1, T2, T3, BackendReturnObject> BackendFunc,
        T1 GT1, T2 GT2, T3 GT3, System.Action SuccessCallback = null, System.Action FailCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_WithoutSaveToken_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetSuccessCallback(SuccessCallback)
            .SetFailCallback(FailCallback)
        );

        BackendAsync(BackendFunc, GT1, GT2, GT3, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_WithoutSaveToken_DataList.Count; i++)
            {
                if (Async_WithoutSaveToken_DataList[i].CallingName == CallingName)
                {
                    Async_WithoutSaveToken_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }

    private void Async_WithoutSaveToken_DataListCheck()
    {
        if (Async_WithoutSaveToken_DataList.Count != 0)
        {
            // 콜백을 실행함.
            for (int i = 0; i < Async_WithoutSaveToken_DataList.Count; i++)
            {
                if (true == Async_WithoutSaveToken_DataList[i].CallbackComplete)
                {
                    if (null != Async_WithoutSaveToken_DataList[i].BRO)
                    {
                        if (false == Async_WithoutSaveToken_DataList[i].ClearMe)
                        {
                            Async_WithoutSaveToken_DataList[i].ClearMe = true;

                            BroChecker(Async_WithoutSaveToken_DataList[i].CallingName, Async_WithoutSaveToken_DataList[i].BRO,
                            () =>
                            {
                                //Debug.Log(Async_WithoutSaveToken_DataList[i].CallingName + " Success!");

                                // 최종콜백
                                Async_WithoutSaveToken_DataList[i].SuccessCallback?.Invoke();
                            },
                            () =>
                            {
                                //Debug.Log(Async_WithoutSaveToken_DataList[i].CallingName + " 실패: /" + Async_WithoutSaveToken_DataList[i].StatusCode + " 콜백 후 종료됨!");

                                // 최종콜백
                                Async_WithoutSaveToken_DataList[i].FailCallback?.Invoke();
                            });
                        }
                    }
                    else
                    {
                        //Debug.Log(Async_WithoutSaveToken_DataList[i].CallingName + " 가 NULL BRO 를 리턴함! 코드 확인할 것.");
                    }
                }
            }

            // 실행된 콜백을 삭제함. (앞에서부터 하면 Remove()에 의해서 사이즈가 달라지므로 참조오류가 발생함. 뒤에서부터 삭제하며 내려옴)
            for (int i = Async_WithoutSaveToken_DataList.Count - 1; i >= 0; i--)
            {
                if (true == Async_WithoutSaveToken_DataList[i].ClearMe)
                {
                    GetCurExeAsyncCount--;

                    Async_WithoutSaveToken_DataList.Remove(Async_WithoutSaveToken_DataList[i]);
                }
            }
        }
    }
    #endregion

    #region Sync
    public void BroChecker
        (string CallingName, BackendReturnObject BRO,
        System.Action SuccessCallback = null, System.Action FailedCallback = null)
    {
        if (BRO.IsSuccess())
        {
            ////Debug.Log("BRO.IsSuccess() == true -> " + CallingName + " 성공: "
            //            + BRO.GetStatusCode() + " // "
            //            + BRO.GetErrorCode() + " // "
            //            + BRO.GetMessage());

            SuccessCallback?.Invoke();
        }
        else
        {
            Debug.Log("BRO.IsSuccess() = false -> " + CallingName + " 실패: "
                        + BRO.GetStatusCode() + " // "
                        + BRO.GetErrorCode() + " // "
                        + BRO.GetMessage());

            FailedCallback?.Invoke();
        }
    }
    #endregion

    #region ROW_CHECK
    public void RowReceiverAsync(string CallingName, Func<BackendReturnObject> BackendFunc, 
         System.Action<LitJson.JsonData> HasValueCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_RowCheck_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetHasValueCallback(HasValueCallback)
        );

        BackendAsync(BackendFunc, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_RowCheck_DataList.Count; i++)
            {
                if (Async_RowCheck_DataList[i].CallingName == CallingName)
                {
                    Async_RowCheck_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void RowReceiverAsync<T1>(string CallingName, Func<T1, BackendReturnObject> BackendFunc, T1 GT,
         System.Action<LitJson.JsonData> HasValueCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_RowCheck_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetHasValueCallback(HasValueCallback)
        );

        BackendAsync(BackendFunc, GT, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_RowCheck_DataList.Count; i++)
            {
                if (Async_RowCheck_DataList[i].CallingName == CallingName)
                {
                    Async_RowCheck_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }
    public void RowReceiverAsync<T1, T2>(string CallingName, Func<T1, T2, BackendReturnObject> BackendFunc, T1 GT1, T2 GT2,
            System.Action<LitJson.JsonData> HasValueCallback = null)
    {
        GetCurExeAsyncCount++;

        Async_RowCheck_DataList.Add
        (
            new BackendData()
            .SetCallingName(CallingName)
            .SetHasValueCallback(HasValueCallback)
        );

        BackendAsync(BackendFunc, GT1, GT2, (BRO_Callback) =>
        {
            for (int i = 0; i < Async_RowCheck_DataList.Count; i++)
            {
                if (Async_RowCheck_DataList[i].CallingName == CallingName)
                {
                    Async_RowCheck_DataList[i]
                    .SetCallbackComplete(true)
                    .SetBRO(BRO_Callback)
                    .SetStatusCode(BRO_Callback.GetStatusCode());
                }
                else continue;
            }
        });
    }

    private void Async_RowDataCheck()
    {
        if (Async_RowCheck_DataList.Count != 0)
        {
            // 콜백을 실행함.
            for (int i = 0; i < Async_RowCheck_DataList.Count; i++)
            {
                if (true == Async_RowCheck_DataList[i].CallbackComplete)
                {
                    if (null != Async_RowCheck_DataList[i].BRO)
                    {
                        if (false == Async_RowCheck_DataList[i].ClearMe)
                        {
                            Async_RowCheck_DataList[i].ClearMe = true;

                            //Debug.Log(Async_RowCheck_DataList[i].CallingName + " Success!");

                            if (Async_RowCheck_DataList[i].BRO.GetReturnValuetoJSON().Keys.Contains("rows"))
                            {
                                Async_RowCheck_DataList[i].HasValueCallback?.Invoke
                                (Async_RowCheck_DataList[i].BRO.GetReturnValuetoJSON()["rows"][0]);
                            }
                            else if (Async_RowCheck_DataList[i].BRO.GetReturnValuetoJSON().Keys.Contains("row"))
                            {
                                Async_RowCheck_DataList[i].HasValueCallback?.Invoke
                                (Async_RowCheck_DataList[i].BRO.GetReturnValuetoJSON()["row"]);
                            }
                            else
                            {
                                Async_RowCheck_DataList[i].HasValueCallback?.Invoke
                                    (Async_RowCheck_DataList[i].BRO.GetReturnValuetoJSON());
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log(Async_RowCheck_DataList[i].CallingName + " 가 NULL BRO 를 리턴함! 코드 확인할 것.");
                    }
                }
            }

            // 실행된 콜백을 삭제함. (앞에서부터 하면 Remove()에 의해서 사이즈가 달라지므로 참조오류가 발생함. 뒤에서부터 삭제하며 내려옴)
            for (int i = Async_RowCheck_DataList.Count - 1; i >= 0; i--)
            {
                if (true == Async_RowCheck_DataList[i].ClearMe)
                {
                    GetCurExeAsyncCount--;

                    Async_RowCheck_DataList.Remove(Async_RowCheck_DataList[i]);
                }
            }
        }
    }
    #endregion
}

public class Singleton<AnyClass> : MonoBehaviour
    where AnyClass : MonoBehaviour
{
    private void Awake()
    {
        SingletonInit();
    }

    // 싱글턴 인스턴스와 프로퍼티
    #region Instance

    protected void SingletonInit()
    {
        if (instance == null)
        {
            //Debug.Log(gameObject.name + " 의 Instance 생성됨");
            instance = this as AnyClass;
        }
        else
        {
            Debug.LogError(gameObject.name + " 의 Instance가 이미 생성되어 있음!");
        }
    }

    private static AnyClass instance = null;

    public static AnyClass Instance
    {
        get
        {
            // 인스턴스가 이미 생성된 경우 리턴한다.
            if (instance != null) return instance;
            else
            {
                // 해당 컴포넌트의 유무를 검색한다.
                instance = FindObjectOfType(typeof(AnyClass)) as AnyClass;

                // 검색에 실패했다면 컴포넌트를 담은 '오브젝트'를 새로 생성한다.
                if (instance == null)
                {
                    string Name = typeof(AnyClass).ToString();

                    Debug.LogError(Name + " 컴포넌트를 찾을 수 없으므로 새로 생성됨.");

                    instance = new GameObject(Name, typeof(AnyClass)).GetComponent<AnyClass>();
                }

                return instance;
            }
        }
    }
    #endregion
}

/*
 사용 예제
 
    public void TokenCheck(System.Action SuccessCallback = null, System.Action FailedCallback = null)
    {
        // 액세스 토큰이 살아있는가?
        BackendChecker.Instance.BroCheckerAsync_WithSaveToken("AccessTokenAlive", Backend.BMember.IsAccessTokenAlive,
        () => // 토큰 있음
        {
            //Debug.Log("토큰 있음. 로그인 시도함");

            // 로그인 시도함
            SuccessCallback?.Invoke();
        },
        () => // 토큰 없음 혹은 유효하지 않음
        {
            //Debug.Log("토큰 없거나 유효하지 않음. 토큰 리프레시 시도함");

            // 토큰 리프레시 시도함
            BackendChecker.Instance.BroCheckerAsync_WithSaveToken("RefreshTheBackendTokenAsync", Backend.BMember.RefreshTheBackendTokenAsync,
            () => // 리프레시 성공
            {
                //Debug.Log("토큰 리프레시 성공, 로그인 시도함");

                // 로그인 시도함
                SuccessCallback?.Invoke();
            },
            () => // 리프레시 실패
            {
                //Debug.Log("토큰 리프레시 실패, 페더레이션 인증 시도함");

                // 페더레이션 인증 시도함
                BackendChecker.Instance.BroCheckerAsync_WithSaveToken("AuthorizeFederationAsync", Backend.BMember.AuthorizeFederationAsync,
                    ConnectManager_Google.Instance.GetToken(), FederationType.Google, "",
                () => // 인증 성공
                {
                    //Debug.Log("페더레이션 인증 성공, 로그인 시도함");

                    // 로그인 시도함
                    SuccessCallback?.Invoke();
                },
                () => // 인증 실패
                {
                    //Debug.Log("페더레이션 인증 실패, 재접속 요청함");

                    // 재접속 UI 호출
                    FailedCallback?.Invoke();
                });
            });
        });
    }

    public void Login(System.Action Success_NewUser = null, System.Action Success_ExistingUser = null, System.Action FailedCallback = null)
    {
        // 로그인 시도함
        BackendChecker.Instance.BroCheckerAsync_WithSaveToken("LoginWithTheBackendTokenAsync", Backend.BMember.LoginWithTheBackendTokenAsync,
        () => // 로그인 성공
        {
            //Debug.Log("로그인 성공!");

            if (false == IsLogin)
            {
                IsLogin = true;
            }
            
            BackendChecker.Instance.RowReceiverAsync("nickname_GetUserInfo", Backend.BMember.GetUserInfo,
            (BRO_Callback) =>
            {
                // nickname 데이터가 존재하지 않음 (신규유저로 판별)
                if (null == BRO_Callback["nickname"])
                {
                    // 백엔드 닉네임을 업데이트한다.
                    UpdateNickname(Success_NewUser, null, FailedCallback);
                }
                // nickname 데이터가 존재함 (기존유저로 판별)
                else
                {
                    // 현재 구글 닉네임과 백엔드 닉네임이 동일한지 검사한다. 
                    // 닉네임이 동일함.
                    if (PlayGamesPlatform.Instance.GetUserDisplayName() == BRO_Callback["nickname"].ToString())
                    {
                        Success_ExistingUser?.Invoke();
                    }
                    // 닉네임이 동일하지 않음.
                    else
                    {
                        // 백엔드 닉네임을 업데이트한다.
                        UpdateNickname(null, Success_ExistingUser, FailedCallback);
                    }
                }
            });
        },
        () => // 로그인 실패
        {
            //Debug.Log("로그인 실패, 재접속 요청함");

            // 재접속 UI 호출
            FailedCallback?.Invoke();
        });
    }
    
    private void UpdateNickname(System.Action Success_NewUser = null, System.Action Success_ExistingUser = null, System.Action FailedCallback = null)
    {
        BackendChecker.Instance.BroCheckerAsync_WithoutSaveToken("UpdateNickname_"+ PlayGamesPlatform.Instance.GetUserDisplayName(),
            Backend.BMember.UpdateNickname, PlayGamesPlatform.Instance.GetUserDisplayName(),
        () => // 닉네임 등록 성공
        {
            if (Success_NewUser != null)
            {
                //Debug.Log("신규 유저 로그인 / 닉네임 등록 성공");
                Success_NewUser?.Invoke();
            }
            else if (Success_ExistingUser != null)
            {
                //Debug.Log("기존 유저 로그인 / 닉네임 업데이트 성공");
                Success_ExistingUser?.Invoke();
            }
            
        },
        () => // 닉네임 등록 실패
        {
            //Debug.Log("기존 유저 로그인 / 닉네임 업데이트 실패");

            // 재접속 UI 호출
            FailedCallback?.Invoke();
        });
    }

*/
