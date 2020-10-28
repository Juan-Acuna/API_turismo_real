using API_TurismoReal.Conexiones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace API_TurismoReal.KhipuApiClient
{
    public class KhipuClient
    {
        static int receiverId = Temp.T_RESEIVER_ID;
        static String baseUrl = "https://khipu.com/api/2.0";
        static String userAgent = "khipu-api-dotnet-client/2.9.1";
        static HttpClient http = new HttpClient();
        public static KhipuClient Peticiones = new KhipuClient();

        private KhipuClient()
        {
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
            http.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            http.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
        }
        public object GetPayments()
        {
            
            return new { };
        }
        public async Task<KhipuResponse> Pay(String subject, String currency, double amount)
        {
            Dictionary<String, object> data = new Dictionary<string, object>();
            data.Add("amount", amount);
            /*data.Add("bank_id", "");
            data.Add("body", "");
            data.Add("cancel_url", "");
            data.Add("collect_account_uuid", "");
            data.Add("confirm_timeout_date", "");
            data.Add("contract_url", "");*/
            data.Add("currency", currency);
            /*data.Add("custom", "");
            data.Add("expires_date", "");
            data.Add("fixed_payer_email", "");
            data.Add("integrator_fee", "");
            data.Add("mandatory_payment_method", "");
            data.Add("notify_api_version", "");
            data.Add("notify_url", "");
            data.Add("payer_email", "");
            data.Add("payer_name", "");
            data.Add("picture_url", "");
            data.Add("responsible_user_email", "");
            data.Add("return_url", "");
            data.Add("send_email", "");
            data.Add("send_reminders", "");*/
            data.Add("subject", subject);
            //data.Add("transaction_id","");
            var firma =  Firmar("POST",data,"/payments");
            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/payments");// + Concatenar(data) + "&hash=" + firma);
            m.Headers.Authorization = new AuthenticationHeaderValue("Basic", Temp.T_RESEIVER_ID.ToString() + ":" + firma);
            m.Content = new StringContent(Concatenar(data)+"&hash="+firma, Encoding.UTF8, "Application/x-www-form-urlencoded");
            var r = await http.SendAsync(m);
            PaymentsCreateResponse pago;
            try
            {
                pago = JsonConvert.DeserializeObject<PaymentsCreateResponse>(await r.Content.ReadAsStringAsync());
            }catch(Exception e)
            {
                pago = null;
            }
            var kr = new KhipuResponse
            {
                StatusCode = r.StatusCode.ToString(),
                PaymentsCreateResponse = pago
            };
            return new KhipuResponse();
        }
        public object CallApi(String resourcePath, HttpMethod method, String headerParams, String postData)
        {

            return new { };
        }
        private String Concatenar(Dictionary<String, object> data)
        {
            String s = "";
            for (int i = 0; i < data.Count; i++)
            {
                s += (i==0?"":"&") + Tools.UrlEncode(data.Keys.ElementAt(i)) + "=" + Tools.UrlEncode(data[data.Keys.ElementAt(i)].ToString());
            }
            return s;
        }
        private String Firmar(String metodo, Dictionary<String, object> data, String url)
        {
            String str = metodo + "&" + Tools.UrlEncode(baseUrl + url) + Concatenar(data);
            str = Tools.EncriptarHmacSHA256(Temp.T_SECRET_KEY, str);
            return str;
        }
        /*
        private async Task<Firma> Firmar(String metodo, Dictionary<String,object> data, String url)
        {
            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Post, "http://localhost:81/AAAAAAAAA/HMACSHA256.php");
            var carga = JsonConvert.SerializeObject(
            new CargaFirma
            {
                Contenido = data,
                Metodo = metodo.ToUpper(),
                Url = url
            });
            m.Content = new StringContent(carga, Encoding.UTF8, "Application/json");
            var r = await http.SendAsync(m);
            String res = await r.Content.ReadAsStringAsync();
            var f = JsonConvert.DeserializeObject<Firma>(res);
            return f;
        }
        public class Firma
        {
            public String Hash { get; set; }
        }
        public class CargaFirma
        {
            public Dictionary<String,object> Contenido { get; set; }
            public String Metodo { get; set; }
            public String Url { get; set; }
        }*/
    }
}
