using BenQGuru.GAIA.BPM.Common.Utility;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Serialization;
using BenQGuru.GAIA.BPM.DevelopmentHelp.Utility;

namespace JLF.Form.WebService
{
    public class PayrollAPI
    {
        string m_url = string.Empty;

        /// <summary>
        /// 系统数据源的配置地址 @Add by Tim.Liu @2015-03-21
        /// </summary>
        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }
        string m_WebserviceID = string.Empty;

        /// <summary>
        /// 系统数据源配置的webservice ID
        /// </summary>
        public string WebserviceID
        {
            get { return m_WebserviceID; }
            set { m_WebserviceID = value; }
        }

        string _XmlNs = string.Empty;
        string is_proxy=string.Empty;
        string proxy_host=string.Empty;
        string proxy_port=string.Empty;
        string proxy_domain=string.Empty;
        string proxy_user_name=string.Empty;
        string proxy_password = string.Empty;

        public PayrollAPI()
        {
           DataTable dt = this.Get_CommonWebservice_source("Payroll接口");
           if (dt != null && dt.Rows.Count>0)
            {
                this.m_WebserviceID = dt.Rows[0]["id"].ToString();
                m_url = dt.Rows[0]["url"].ToString();
                is_proxy = dt.Rows[0]["is_proxy"].ToString();
                proxy_host = dt.Rows[0]["proxy_host"].ToString();
                proxy_port = dt.Rows[0]["proxy_port"].ToString();
                proxy_domain = dt.Rows[0]["proxy_domain"].ToString();
                proxy_user_name = dt.Rows[0]["proxy_user_name"].ToString();
                proxy_password = dt.Rows[0]["proxy_password"].ToString();
            }
            _XmlNs = "http://tempuri.org/";
        }

        #region 执行回写方法

        /// <summary>
        /// 执行回写方法
        /// </summary>
        /// <param name="_Url">Webservice Url 传null，系统默认url</param>
        /// <param name="MethodType">方法类型 POST GET SOAP</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">参数集合</param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        public ReturnMessage Execute_WriteBack_Method(string _Url,string MethodType, string MethodName, Hashtable Parms, out string Msg)
        {
            Msg = string.Empty;
            ReturnMessage RMsg = new ReturnMessage();
            XmlDocument xd=new XmlDocument();
            if (!string.IsNullOrEmpty(_Url))
            {
                m_url = _Url;
            }
            try
            {
                switch (MethodType)
                {
                    case WebMethodType.GET: xd = this.Execute_GetWebService(MethodName, Parms, out Msg); break;
                    case WebMethodType.POST: xd = this.Execute_PostWebService(MethodName, Parms, out Msg); break;
                    default: xd = this.Execute_SoapWebService(MethodName, Parms, null, out Msg); break;
                }
                string strReturnValue = xd.InnerText;

                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(strReturnValue);
                    XmlNode objHeadNode = xdoc.SelectSingleNode("//Head");
                    RMsg.form_no = objHeadNode.SelectSingleNode("form_no").InnerText.Trim();
                    RMsg.Status = Convert.ToBoolean(objHeadNode.SelectSingleNode("Status").InnerText.Trim());
                    RMsg.Message = objHeadNode.SelectSingleNode("Message").InnerText.Trim();

                    XmlNode objBodyNode = xdoc.SelectSingleNode("//Body");
                    if (objBodyNode.ChildNodes.Count > 0)
                    {
                        RMsg.Entity = objBodyNode.InnerXml.ToString().Trim();
                    }
                }
                catch (Exception ex)
                {
                    Msg = @"返回值格式不正确，请检查!正确格式：
                                                        <FlowER>
            			                                  <Head>       
            				                                  <form_no>表单号</form_no>
                                                              <Status>状态</Status>
                                                              <Message>消息</Message>
            			                                  </Head>    
            			                                  <Body> 
                                                                  <ContractMaterial>
                                                                        <OID>编号</OID>
                                                                        <NAME>姓名</NAME>
                                                                 </ContractMaterial>
    		                                                 </Body>    
    	                                               </FlowER>";
                    throw new Exception(Msg);
                }
            }
            catch (Exception ex)
            {
                RMsg.Status = false;
                RMsg.Message = ex.Message;
                Msg = ex.Message;
            }

            return RMsg;
        }

        #endregion

        #region 执行Check方法

        /// <summary>
        /// 执行Check方法
        /// </summary>
        /// <param name="_Url">Webservice Url 传null，系统默认url</param>
        /// <param name="MethodType">方法类型 POST GET SOAP</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">参数集合</param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        public ReturnMessage Execute_Check_Method(string _Url, string MethodType, string MethodName, Hashtable Parms, out string Msg)
        {
            Msg = string.Empty;
            ReturnMessage RMsg = new ReturnMessage();
            XmlDocument xd = new XmlDocument();
            if (!string.IsNullOrEmpty(_Url))
            {
                m_url = _Url;
            }
            try
            {
                LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Check_Method:" + MethodName + "(执行Payroll接口)");
                switch (MethodType)
                {
                    case WebMethodType.GET: xd = this.Execute_GetWebService(MethodName, Parms, out Msg); break;
                    case WebMethodType.POST: xd = this.Execute_PostWebService(MethodName, Parms, out Msg); break;
                    default: xd = this.Execute_SoapWebService(MethodName, Parms, null, out Msg); break;
                }

                string strReturnValue = xd.InnerText;

                try
                {
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Check_Method:" + MethodName + "(调用成功，解析返回结果)");
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(strReturnValue);
                    XmlNode objHeadNode = xdoc.SelectSingleNode("//Head");
                    RMsg.form_no = objHeadNode.SelectSingleNode("form_no").InnerText.Trim();
                    RMsg.Status = Convert.ToBoolean(objHeadNode.SelectSingleNode("Status").InnerText.Trim());
                    RMsg.Message = objHeadNode.SelectSingleNode("Message").InnerText.Trim();
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Check_Method:" + MethodName + "(解析返回结果成功)");

                }
                catch (Exception ex)
                {
                    Msg = @"返回值格式不正确，请检查!正确格式：
                                                        <FlowER>
            			                                  <Head>       
            				                                  <form_no>表单号</form_no>
                                                              <Status>状态</Status>
                                                              <Message>消息</Message>
            			                                  </Head>    
            			                                  <Body> 
    		                                              </Body>    
    	                                               </FlowER>";
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Check_Method:" + MethodName + "(解析返回结果失败：" + Msg + ")");
                    throw new Exception(Msg);
                }
            }
            catch (Exception ex)
            {
                RMsg.Status = false;
                RMsg.Message = ex.Message;
                Msg = ex.Message;
                LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Check_Method:" + MethodName + "(调用异常：" + Msg + ")");
            }

            return RMsg;
        }

        #endregion

        #region 执行Query方法

        /// <summary>
        /// 执行查询方法
        /// </summary>
        /// <param name="_Url">Webservice Url 传null，系统默认url</param>
        /// <param name="MethodType">方法类型 POST GET SOAP</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">参数集合</param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        public ReturnMessage Execute_Get_Method(string _Url, string MethodType, string MethodName, Hashtable Parms, out string Msg)
        {
            Msg = string.Empty;
            ReturnMessage RMsg = new ReturnMessage();
            XmlDocument xd = new XmlDocument();
            if (!string.IsNullOrEmpty(_Url))
            {
                m_url = _Url;
            }
            try
            {
                LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Get_Method:" + MethodName + "(执行Payroll接口)");
                switch (MethodType)
                {
                    case WebMethodType.GET: xd = this.Execute_GetWebService(MethodName, Parms, out Msg); break;
                    case WebMethodType.POST: xd = this.Execute_PostWebService(MethodName, Parms, out Msg); break;
                    default: xd = this.Execute_SoapWebService(MethodName, Parms, null, out Msg); break;
                }
               
                try
                {
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Get_Method:" + MethodName + "(调用成功，解析返回结果)");
                    string strReturnValue = xd.InnerText;
                    RMsg.Status = true;
                    RMsg.Message = "获取数据成功!";
                    RMsg.Entity = strReturnValue;
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Get_Method:" + MethodName + "(解析返回结果成功)");
                }
                catch (Exception ex)
                {
                    Msg = @"返回值格式不正确，请检查!正确格式：
                           <FlowER><Item><TextField>Text A</TextField><ValueField>Value A</ValueField></Item>...</FlowER>";
                    LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Get_Method:" + MethodName + "(解析返回结果失败：" + Msg + ")");
                    throw new Exception(Msg);
                }
            }
            catch (Exception ex)
            {
                RMsg.Status = false;
                RMsg.Message = ex.Message;
                Msg = ex.Message;
                LogHelp.WriteInfoLog("JLFFormWebService", "Execute_Get_Method:" + MethodName + "(调用异常：" + Msg + ")");
            }

            return RMsg;
        }

        #endregion

        #region 执行Post Webservice方法

        /// <summary>
        /// 执行Post Webservice方法
        /// </summary>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">
        /// Key 列名称
        /// Value 列值
        /// </param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        private XmlDocument Execute_PostWebService(string MethodName, Hashtable Parms,out string Msg)
        {
            Msg = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_url + "/" + MethodName);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                SetWebRequest(request);
                byte[] data = EncodePars(Parms);
                WriteRequestData(request, data);
                return ReadXmlResponse(request.GetResponse());
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
                return new XmlDocument();
            }
        }

        #endregion

        #region 执行Get Webservice方法

        /// <summary>
        /// 执行Get Webservice方法
        /// </summary>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">
        /// Key 列名称
        /// Value 列值
        /// </param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        private XmlDocument Execute_GetWebService(string MethodName, Hashtable Parms, out string Msg)
        {
            Msg = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_url + "/" + MethodName + "?" + ParsToString(Parms));
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                SetWebRequest(request);
                Msg = "执行成功";
                return ReadXmlResponse(request.GetResponse());
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
                return new XmlDocument();
            }
        }

        #endregion

        #region 执行Execute SoapWebService方法

        /// <summary>
        /// 执行Execute SoapWebService方法
        /// </summary>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parms">
        /// Key 列名称
        /// Value 列值
        /// </param>
        /// <param name="XmlNs">XmlNs</param>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        private XmlDocument Execute_SoapWebService(string MethodName, Hashtable Parms, string XmlNs, out string Msg)
        {
            Msg = string.Empty;
            if (XmlNs == null)
            {
                XmlNs = _XmlNs;
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_url);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Headers.Add("SOAPAction", "\"" + XmlNs + (XmlNs.EndsWith("/") ? "" : "/") + MethodName + "\"");
                SetWebRequest(request);

                byte[] data = EncodeParsToSoap(Parms, XmlNs, MethodName);
                WriteRequestData(request, data);
                XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
                doc = ReadXmlResponse(request.GetResponse());

                XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
                mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                String RetXml = doc.SelectSingleNode("//soap:Body/*/*", mgr).InnerXml;
                doc2.LoadXml("<root>" + RetXml + "</root>");
                AddDelaration(doc2);
                return doc2;
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
                throw new Exception(Msg);
            }
        }

        #endregion

        #region 参数转换成字符串

        /// <summary>
        /// 参数转换成字符串
        /// </summary>
        /// <param name="Pars"></param>
        /// <returns></returns>
        private static string ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                //sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
            }
            return sb.ToString();
        }

        #endregion

        #region 参数编码 Utf8

        /// <summary>
        /// 参数编码 Utf8
        /// </summary>
        /// <param name="Pars"></param>
        /// <returns></returns>
        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }

        #endregion

        #region 编码Soap

        /// <summary>
        /// 编码Soap
        /// </summary>
        /// <param name="Pars"></param>
        /// <param name="XmlNs"></param>
        /// <param name="MethodName"></param>
        /// <returns></returns>
        private static byte[] EncodeParsToSoap(Hashtable Pars, String XmlNs, String MethodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            AddDelaration(doc);
            //XmlElement soapBody = doc.createElement_x_x("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            //XmlElement soapMethod = doc.createElement_x_x(MethodName);
            XmlElement soapMethod = doc.CreateElement(MethodName);
            soapMethod.SetAttribute("xmlns", XmlNs);
            foreach (string k in Pars.Keys)
            {
                //XmlElement soapPar = doc.createElement_x_x(k);
                XmlElement soapPar = doc.CreateElement(k);
                soapPar.InnerXml = ObjectToSoapXml(Pars[k]);
                soapMethod.AppendChild(soapPar);
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }

        /// <summary>
        /// 添加XML声明
        /// </summary>
        /// <param name="doc"></param>
        private static void AddDelaration(XmlDocument doc)
        {
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
        }

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string ObjectToSoapXml(object o)
        {
            XmlSerializer mySerializer = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            mySerializer.Serialize(ms, o);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(ms.ToArray()));
            if (doc.DocumentElement != null)
            {
                return doc.DocumentElement.InnerXml;
            }
            else
            {
                return o.ToString();
            }
        }

        #endregion

        #region 写请求数据

        /// <summary>
        /// 写请求数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        #endregion

        #region 读取返回数据

        /// <summary>
        /// 读取返回数据
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }

        #endregion

        /// <summary>
        /// 设置凭证与超时时间
        /// </summary>
        /// <param name="request"></param>
        private void SetWebRequest(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 60000;
            if (is_proxy != "0")
            {
                LogHelp.WriteInfoLog("JLF.Form.WebService", "代理服务器设置！");
                ProxySetting(request, "http://" + proxy_host + ":" + proxy_port, proxy_user_name, proxy_password);
            }
            else
            {
                LogHelp.WriteInfoLog("JLF.Form.WebService", "代理服务器不设置！" );
            }
        }

        /// <summary>
        /// 获取公共数据源配置
        /// </summary>
        /// <param name="PayrollName"></param>
        /// <returns></returns>
        public DataTable Get_CommonWebservice_source(string PayrollName)
        {
            string databaseKey = new DatabaseKeyManager().GetConnectionString();
            Database db = DatabaseFactory.CreateDatabase(databaseKey);
            string strSql = @"select a.* from gcore.webservice_source a inner join gcore.system_data_source b
                                on a.data_source_id=b.id where b.name=@PayrollName";

            DbCommand dc = db.GetSqlStringCommand(strSql);
            db.AddInParameter(dc, "@PayrollName", DbType.String, PayrollName);
            DataSet ds = db.ExecuteDataSet(dc);
            using (DataTable dt = ds.Tables[0])
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return new DataTable();
                }
            }
        }

        #region 代理服务器设置

        /// <summary>
        /// 代理服务器设置
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ProxyAddress">代理服务地址</param>
        /// <param name="ProxyUser">用户</param>
        /// <param name="ProxyKey">密码</param>
        public void ProxySetting(WebRequest request, string ProxyAddress, string ProxyUser, string ProxyKey)
        {
            try
            {
                WebProxy proxy = new WebProxy(ProxyAddress, true);

                if (!string.IsNullOrEmpty(ProxyAddress) && !string.IsNullOrEmpty(ProxyUser) && !string.IsNullOrEmpty(ProxyKey))//如果地址为空，则不需要代理服务器
                {
                    proxy.Credentials = new NetworkCredential(ProxyUser, ProxyKey);//从配置封装参数中创建
                }
                request.Proxy = proxy;//赋予 request.Proxy 
                LogHelp.WriteInfoLog("JLF.Form.WebService", "代理服务器设置成功！代理服务host和port:" + ProxyAddress + "用户名：" + ProxyUser);

            }
            catch (Exception ex)
            {
                LogHelp.WriteErrorLog("JLF.Form.WebService", "代理服务器设置失败！代理服务host和port:" + ProxyAddress + "用户名：" + ProxyUser + ",错误原因：" + ex.Message);
            }
        }

        #endregion
    }

}
