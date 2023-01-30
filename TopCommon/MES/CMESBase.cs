using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;
using TopCom.MES;
using XSmNetW;
using WindowsFormsApplication1;

namespace TopCom.MES
{
    public class CMESBase : PropertyChangedNotifier
    {
        #region Privates
        // 공유메모리 변수 선언
        protected XSmNet4CS _xcom = null;
        private bool _ConnectStatus;
        private CMESInfo _MESInfo = new CMESInfo();
        #endregion

        public string[] MesStatusMessage = new string[]
        {
            "",
            "AUTO",
            "IDLE",
            "EQ DOWN",
            "SETUP",
            "PROGRAM END",
        };

        public string ConfigFilePath = Path.Combine(Environment.CurrentDirectory, "EQ1.cfg");

        public CMESInfo MESInfo
        {
            get { return _MESInfo; }
            set
            {
                if (_MESInfo == value) return;

                _MESInfo = value;
                OnPropertyChanged();
            }
        }

        public bool ConnectStatus
        {
            get { return _ConnectStatus; }
            set
            {
                if (_ConnectStatus == value) return;

                _ConnectStatus = value;
                OnPropertyChanged();
            }
        }

        public CMESBase()
        {
            InitMES();
        }

        public void InitMES()
        {
            // 공유메모리 변수 instance
            this._xcom = new XSmNet4CS();

            // 공유메모리 변수 이벤트 핸들러 등록
            this._xcom.OnConnected += _xcom_OnConnected;
            this._xcom.OnDisconnected += _xcom_OnDisconnected;
            this._xcom.OnReceived += _xcom_OnReceived;

            // 공유메모리 Initialize 및 Start 까지 해야함.
            Initialize();
            Start();
        }
        /// <summary>
        /// 공유메모리 dll initialize
        /// </summary>
        private void Initialize()
        {
            // cfg 파일이 있어야 한다. 프로젝트에 포함시켰음. (EQ1.cfg)
            int res = this._xcom.Initialize(ConfigFilePath);
            this._xcom.BinaryType = true;

            MESInfo = CMESInfo.Load();

            ConnectStatus = false;
        }

        /// <summary>
        /// 공유메모리 dll 종료
        /// </summary>
        private void UnInitialize()
        {
            int ret = this._xcom.Terminate();
        }

        /// <summary>
        /// 공유메모리 dll Start
        /// </summary>
        private void Start()
        {
            int ret = this._xcom.Start();
        }

        private void Stop()
        {
            int ret = this._xcom.Stop();
        }

        /// <summary>
        /// XValueNet dll 을 이용하여 Parsing
        /// </summary>
        /// <param name="pcValue"></param>
        /// <param name="nSize"></param>
        private void _xcom_OnReceived(byte[] pcValue, int nSize)
        {
            int count = 0;
            string msgName = "";
            string temp = "";
            // XValueClr 변수 선언
            XValue xValue = new XValue();

            // XValueClr 을 이용하여 전송받은 byte[] 를 XValueClr 클래스에 Load
            int ret = xValue.LoadFromSECS2(ref pcValue, 0, nSize);

            // 사양서에 맞게 코딩
            count = xValue.GetCount();          // LIST 2
            {
                XValue val1 = new XValue();
                val1 = xValue.GetFirstItem();
                msgName = val1.GetString();

                // 메시지 이름으로 분류하여 Parsing
                if (msgName == "MES_STANDARD_INFO")
                {
                    val1 = xValue.GetNextItem(); count = val1.GetCount();              // LIST 3
                    {
                        XValue val2 = new XValue();
                        val2 = val1.GetFirstItem(); MESInfo.Head1.DeviceCode = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.OperationCode = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.EQPCode = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.ProductTypeCode = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.Device = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.Operation = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.EQPName = val2.GetString();
                        val2 = val1.GetNextItem(); MESInfo.Head1.ProductType = val2.GetString();

                        MESInfo.Save();
                    }
                }
                else if (msgName == "MES_CONNECTION_STATUS")
                {
                    val1 = xValue.GetNextItem(); count = val1.GetCount();              // LIST 3
                    {
                        byte[] tempbyte;
                        XValue val2 = new XValue();
                        val2 = val1.GetFirstItem(); temp = val2.GetString();
                        val2 = val1.GetNextItem(); tempbyte = val2.GetU1();

                        if (tempbyte.Length > 0)
                            ConnectStatus = tempbyte[0] == 0 ? false : true;
                    }
                }

                //if (msgName == "WORK_POSSIBLE_RESULT")
                //{
                //    xValue.GetNextItem(ref val1); count = val1.GetCount();              // LIST 3
                //    {
                //        XValueClr val2 = new XValueClr();
                //        val1.GetFirstItem(ref val2); temp = val2.GetString();        // EQUIPMENT_ID
                //        val1.GetNextItem(ref val2); temp = val2.GetString();        // MATERIAL_NAME
                //        val1.GetNextItem(ref val2); temp = val2.GetString();        // BARCODE
                //    }
                //}
                //else if (msgName == "RECIPE_REGISTER")
                //{
                //    xValue.GetNextItem(ref val1); count = val1.GetCount();            // LIST 2
                //    {
                //        XValueClr val2 = new XValueClr();
                //        val1.GetFirstItem(ref val2); temp = val2.GetString();        // EQUIPMENT_ID
                //        val1.GetNextItem(ref val2); count = val2.GetCount();           // LIST 1
                //        {
                //            XValueClr val3 = new XValueClr();
                //            val2.GetFirstItem(ref val3); temp = val3.GetString();    // RESULT
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// 공유메모리 연결종료 이벤트 (아래 코드를 넣어준다. 그렇지 않을 경우 연결이 끊어졌을 때 이벤트 계속 발생)
        /// </summary>
        private void _xcom_OnDisconnected()
        {
            Stop();
            UnInitialize();

            System.Threading.Thread.Sleep(5000);

            Initialize();
            Start();
        }

        /// <summary>
        /// 공유메모리 연결 이벤트
        /// </summary>
        private void _xcom_OnConnected()
        {
            // TODO
            Send_MESOtpion();
            Send_PCInfoData();

            Send_EquipStatus(EMESEqpStatus.IDLE);
            Send_AlarmStatus(EAlarmStatus.AllClear, 0, "");
        }

        public void Send_EquipStatus(EMESEqpStatus eqpStatus)
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("COLLECTION_EVENT"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람
                    xTemp.SetU1(1); xList1.Add(xTemp);                              // CEID = 1 
                    xList2.SetList();                                               // LIST 2
                    {
                        xTemp.SetU1(100); xList2.Add(xTemp);                       // RPTID
                        xList3.SetList();                                           // LIST 1
                        {
                            byte st = (byte)eqpStatus;
                            xTemp.SetU1(st); xList3.Add(xTemp);                      // NEW_STATE

                            xTemp.SetString(MesStatusMessage[(int)eqpStatus]); xList3.Add(xTemp);            //추가 상태 정보를 전송 함
                        }

                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            this._xcom?.SendMsg(sendData, sendData.Length);
        }
        public void Send_PMStatus(EPMStatus pmStatus)
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("COLLECTION_EVENT"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람
                    xTemp.SetU1(2); xList1.Add(xTemp);                              // CEID = 2
                    xList2.SetList();                                               // LIST 2
                    {

                        xTemp.SetU1(101); xList2.Add(xTemp);                       // RPTID
                        xList3.SetList();                                           // LIST 1
                        {
                            byte st = (byte)(pmStatus + 1);
                            xTemp.SetU1(st); xList3.Add(xTemp);                      // NEW_STATE

                            xTemp.SetString(pmStatus.ToString()); xList3.Add(xTemp);            //추가 상태 정보를 전송 함
                        }

                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }
        public void Send_AlarmStatus(EAlarmStatus AlarmStatus, int AlarmID, string strMsg)
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("COLLECTION_EVENT"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람
                    xTemp.SetU1(3); xList1.Add(xTemp);                              // CEID = 3
                    xList2.SetList();                                               // LIST 2
                    {

                        xTemp.SetU1(102); xList2.Add(xTemp);                       // RPTID
                        xList3.SetList();                                           // LIST 1
                        {
                            byte st = (byte)(AlarmStatus + 1);
                            xTemp.SetU1(st); xList3.Add(xTemp);                      // NEW_STATE
                            xTemp.SetU2((ushort)AlarmID); xList3.Add(xTemp);                      // NEW_STATE

                            xTemp.SetString(strMsg); xList3.Add(xTemp);            //추가 상태 정보를 전송 함
                        }

                        xList2.Add(xList3);
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }
        public void Send_MESOtpion()
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("MES_USE_OPTION"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람

                    if (MESInfo.UseMES)
                    {
                        xTemp.SetU1(1); xList1.Add(xTemp);                              //MES USE
                    }
                    else
                        xTemp.SetU1(0); xList1.Add(xTemp);                              //MES USE

                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }
        public void Send_RecipeChange()
        {
            MESInfo.Head1.Device = "";
            MESInfo.Head1.Operation = "";
            MESInfo.Head1.ProductType = "";
            MESInfo.Head1.EQPName = "";
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("RECIPE_CHANGE_EVENT"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람

                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }

        public virtual void Send_PCInfoData()
        {
            XValue xValue = new XValue();
            XValue xList1 = new XValue();
            XValue xList2 = new XValue();
            XValue xList3 = new XValue();
            XValue xTemp = new XValue();

            // 함수테스트
            xValue.SetList();           // LIST 2
            {
                xTemp.SetString("EQUIP_PC_INFO"); xValue.Add(xTemp);             // MESSAGE NAME //고정
                xList1.SetList();                                                   // LIST 3
                {
                    xTemp.SetString(MESInfo.Head1.DeviceCode); xList1.Add(xTemp);      // EQUIPMENT ID //장비 아이디를 1번이라도 못 받으면 알람
                    xList2.SetList();                                               // LIST 2
                    {

                        xTemp.SetString(""); xList2.Add(xTemp);            //OS
                        xTemp.SetString(""); xList2.Add(xTemp);            //BIT
                        xTemp.SetString(""); xList2.Add(xTemp);            //IP
                        xTemp.SetString(""); xList2.Add(xTemp);            //MAC ADDRESS
                        xTemp.SetString(""); xList2.Add(xTemp); //핸들러 버전
                        xTemp.SetString(""); xList2.Add(xTemp); //비전 버전
                    }
                    xList1.Add(xList2);
                }
                xValue.Add(xList1);
            }

            int len = xValue.GetByteLength();
            byte[] sendData = new byte[len];

            xValue.MakeSECS2Stream(ref sendData, 0, len);

            int ret = this._xcom.SendMsg(sendData, sendData.Length);
        }

        /// <summary>
        /// PD File write function
        /// </summary>
        /// <param name="headNo">Head Number start with 0</param>
        public virtual void SavePD(int headNo)
        {
            return;
        }

        /// <summary>
        /// TT File write function
        /// </summary>
        /// <param name="headNo">Head Number start with 0</param>
        public virtual void SaveTT(int headNo)
        {
            return;
        }
    }
}
