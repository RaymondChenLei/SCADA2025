using SCADA.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SCADA.Service.Helper
{
    public class APIInterfaceHelper
    {
        public static readonly string bsseUrl = "http://192.168.3.2:8088";

        public async Task<string> CallGetRemoteServerCodeAsync(string code)
        {
            // 构造 URL（query 参数需要 UrlEncode）
            var uri = $"/Common/GetRemoteServerCode?code={Uri.EscapeDataString(code)}";
            // 设置 Accept（根据 Swagger 中的 Media type）
            using (var client = new HttpClient { BaseAddress = new(bsseUrl) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                using var resp = await client.GetAsync(uri);
                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}