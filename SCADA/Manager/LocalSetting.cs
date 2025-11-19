using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Manager
{
    public class LocalSetting
    {
        public LocalSetting()
        {
            _trustedinitialcatalog = "APTIV_DATABASE";
            //_datasource = "10.243.21.38";
            _datasource = "192.168.3.2";
            _initialcatalog = "APTIV_DATABASE";
            _userid = "sa";
            _pwd = "123456";
            _operatormenubarfilename = "OperatorMenubarSetting.json";
            _homemenubarfilename = "HomeMenubarSetting.json";
            _appsettingfilename = "AppSetting.json";
        }

        #region 属性定义

        private string _appsettingfilename;
        private string _configfilename;
        private string _datasource;
        private string _homemenubarfilename;
        private string _initialcatalog;
        private string _LoginIdNo;
        private string _operatormenubarfilename;
        private string _pwd;
        private bool _trustedconnection;
        private string _trustedinitialcatalog;
        private string _userid;

        public string AppSettingFilename
        {
            get { return _appsettingfilename; }
            set { _appsettingfilename = value; }
        }

        public string HomeMenubarFilename
        {
            get { return _homemenubarfilename; }
            set { _homemenubarfilename = value; }
        }

        public string LogInIdNo
        {
            get { return _LoginIdNo; }
            set { _LoginIdNo = value; }
        }

        public string OperatorMenubarFilename
        {
            get { return _operatormenubarfilename; }
            set { _operatormenubarfilename = value; }
        }

        public string ServerDataSource
        {
            get { return _datasource; }
            set { _datasource = value; }
        }

        public string ServerInitialCatalog
        {
            get { return _initialcatalog; }
            set { _initialcatalog = value; }
        }

        public string ServerPassword
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

        public bool ServerTrustedConnection
        {
            get { return _trustedconnection; }
            set { _trustedconnection = value; }
        }

        public string ServerUserID
        {
            get { return _userid; }
            set { _userid = value; }
        }

        public string TrustedServerInitialCatalog
        {
            get { return _trustedinitialcatalog; }
            set { _trustedinitialcatalog = value; }
        }

        #endregion 属性定义
    }
}