using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLF.Form.WebService
{
    [Serializable]
    public class ReturnMessage
    {
        /// <summary>
        /// 表单号
        /// </summary>
        public string form_no { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 返回成功或失败的消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 承载实体
        /// </summary>
        public object Entity { get; set; }
    }
}
