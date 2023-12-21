using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TaxesDemo.BL;

namespace TaxesDemo.Controllers
{
    public class HomeController : Controller
    {
        private string _shaam_url = "https://openapi.taxes.gov.il/shaam/tsandbox";
        private string _shaam_ita_url = "https://ita-api.taxes.gov.il/shaam/tsandbox";

        //The key and secret are only valid if you run the demo under localhost:44327
        //If you run it under anothe port (say 34567) then you need to create an APP in the Sandbox portal and
        //set the Redirect URI of said App to https://localhost:34567
        const string client_id_localhost_44327     = "6d2e5958492712a68a9a126377ca9ae0";
        const string client_secret_localhost_44327 = "79b2f283ba87650e59a202d42ce98aff";

        const string client_id                     = "5c49dc2bbf8ef34151fa40b9b8030a5c";
        const string client_secret                 = "a3b632650e969a23d6041da689b64b6b";

        public HomeController()
        {
        }
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Globals.Get("shaam_url")))       { Globals.Set("shaam_url", _shaam_url); }
            if (string.IsNullOrEmpty(Globals.Get("shaam_ita_url")))   { Globals.Set("shaam_ita_url", _shaam_ita_url); }
            if (string.IsNullOrEmpty(Globals.Get("redirectUrl")))     {Globals.Set("redirectUrl", $"{Request.Url.Scheme}://{Request.Url.Authority}/OpenAPI"); }
            if (string.IsNullOrEmpty(Globals.Get("client_id")))
            {
                if (Globals.Get("redirectUrl") == "https://demo.open-api.co.il/OpenAPI")
                {
                    Globals.Set("client_id", client_id);
                }
                else if (Globals.Get("redirectUrl").ToLower() == "https://localhost:44327/openapi")
                {
                    Globals.Set("client_id", client_id_localhost_44327);
                }
                else
                {
                    Globals.Set("client_id", "== Set your App Key here ==");
                }
            }
            if (string.IsNullOrEmpty(Globals.Get("client_secret")))
            {
                if (Globals.Get("redirectUrl") == "https://demo.open-api.co.il/OpenAPI")
                {
                    Globals.Set("client_secret", client_secret);
                }
                else if (Globals.Get("redirectUrl").ToLower() == "https://localhost:44327/openapi")
                {
                    Globals.Set("client_secret", client_secret_localhost_44327);
                }
                else
                {
                    Globals.Set("client_secret", "== Set your App Secret here ==");
                }
            }
            if (string.IsNullOrEmpty(Globals.Get("Authorization")))   { Globals.Set("Authorization", "Basic " + Utils.Base64Encode($"{client_id}:{client_secret}")); }
            if (string.IsNullOrEmpty(Globals.Get("invoice_details"))) {
                string json = new JavaScriptSerializer().Serialize(CreateInvoice());
                Globals.Set("invoice_details", json.Replace(",", ",\r\n")); 
            }
            return View();
        }

        public ActionResult OneStep()
        {
            return View();
        }

        public ActionResult DevRegister()
        {
            return View();
        }

        public ActionResult UserRegister()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Documentation()
        {
            return View();
        }
        

        public void SaveKeyAndSecret(string shaam_url, string shaam_ita_url, string key, string secret, string redirectUrl, string state)
        {
            Globals.Set("shaam_url", shaam_url);
            Globals.Set("shaam_ita_url", shaam_ita_url);
            Globals.Set("client_id", key);
            Globals.Set("client_secret", secret);
            Globals.Set("redirectUrl", redirectUrl);
            Globals.Set("Authorization", "Basic " + Utils.Base64Encode($"{Globals.Get("client_id")}:{Globals.Get("client_secret")}"));
            Globals.Set("state", state);
            Globals.Set("SaveKeyAndSecret_response", "Data saved");

            //Reset all other data:
            Globals.Set("GetAuthorization_url", null);
            Globals.Set("code", null); ;
            Globals.Set("TokenResponse", null);
            Globals.Set("access_token", null);
            Globals.Set("refresh_token", null);
            Globals.Set("access_token_expire_at", null);
            Globals.Set("refresh_token_expire_at", null);
            Globals.Set("invoice_number", null);
            Globals.Set("inumberResponse", null);

            Response.Redirect("/?#1");
        }


        public void GetAuthorization()
        {
            Globals.Set("SaveKeyAndSecret_response", null);

            string url = ($"{Globals.Get("shaam_url")}/longtimetoken/oauth2/authorize?response_type=code&client_id={Globals.Get("client_id")}&scope=scope&state={Server.UrlEncode( Globals.Get("state") )}");
            Globals.Set("GetAuthorization_url", url);
            Response.Redirect(url);
        }

        public void SaveCode(string code)
        {
            Globals.Set("code", code); ;
            Globals.Set("SaveKeyAndSecret_response", "Data saved");

            //Reset all other data:
            Globals.Set("GetAuthorization_url", null);
            Globals.Set("code", null); ;
            Globals.Set("TokenResponse", null);
            Globals.Set("access_token", null);
            Globals.Set("refresh_token", null);
            Globals.Set("access_token_expire_at", null);
            Globals.Set("refresh_token_expire_at", null);
            Globals.Set("invoice_number", null);
            Globals.Set("inumberResponse", null);

            Response.Redirect("/?#3");
        }

        public void GetToken()
        {

            Globals.Set("access_token", null);
            Globals.Set("refresh_token", null);
            Globals.Set("access_token_expire_at", null);
            Globals.Set("refresh_token_expire_at", null);
            Globals.Set("TokenResponse", null);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            string url = $"{Globals.Get("shaam_url")}/longtimetoken/oauth2/token";

            headers.Add("Authorization", Globals.Get("Authorization"));

            data.Add("grant_type", "authorization_code");
            data.Add("code", Globals.Get("code"));
            data.Add("redirect_uri", Globals.Get("redirectUrl"));

            string tokenResponse = null;
            string debug = null;
            try
            {
                tokenResponse = Utils.HttpPost(url, data, out debug, headers);
                ProcessTokenResponse(tokenResponse);
                Globals.Set("TokenResponse", $"{debug}\r\n🡆\r\n{tokenResponse}".Replace("\r\n", "<br/>"));
            }
            catch (Exception ex)
            {
                tokenResponse = ex.ToString();

                Globals.Set("TokenResponse", $"<div style='color:red;'>{tokenResponse.Replace("\r\n", "<br/>")}</div>");
            }

            Response.Redirect("/?#4");
        }

        public void RefreshToken(string refreshToken)
        {

            Globals.Set("access_token", null);
            Globals.Set("access_token_expire_at", null);
            Globals.Set("refresh_token", refreshToken);
            Globals.Set("refresh_token_expire_at", null);
            Globals.Set("TokenResponse", null);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            string url = $"{Globals.Get("shaam_url")}/longtimetoken/oauth2/token";

            data.Add("grant_type", "refresh_token");
            data.Add("client_id", Globals.Get("client_id"));
            data.Add("client_secret", Globals.Get("client_secret"));
            data.Add("refresh_token", Globals.Get("refresh_token"));

            string tokenResponse = null;
            string debug = null;
            try
            {
                tokenResponse = Utils.HttpPost(url, data, out debug, headers);
                ProcessTokenResponse(tokenResponse);
                Globals.Set("TokenResponse", null);
                Globals.Set("refreshTokenResponse", $"{debug}\r\n🡆\r\n{tokenResponse}".Replace("\r\n", "<br/>"));
            }
            catch (Exception ex)
            {
                tokenResponse = ex.ToString();

                Globals.Set("refreshTokenResponse", $"<div style='color:red;'>{tokenResponse.Replace("\r\n", "<br/>")}</div>");
            }

            Response.Redirect("/?#6");
        }

        private void ProcessTokenResponse(string tokenResponse)
        {
            /*
            {"token_type":"Bearer",
            "access_token":"AAIgNmQyZTU5NTg0OTI3MTJhNjhhOWExMjYzNzdjYTlhZTCo07GdjnXIY3TuGhbcBFl2B8yxQUnbWcP_VDU6C-dVe0w0pbDesddEQbrENWOXtsZqf5UoI0dTCgOyEPQIeQpR6ywMgHjwzCI6My8e5B5rYA","
            scope":"scope",
            "expires_in":601,"consented_on":1692267162,
            "refresh_token":"AAJ_3Qf5ViXy-X1ZsfHFvG8e2a_gS6AsnMqMfm8VXBS7kiWpe2LUfiFoVQBU2kz76X6zHXnso_-dnZ-Xcmsxg7nVXWsKAli-8ap1gyRGgE_Hkw",
            "refresh_token_expires_in":2592000}            
            */


            Globals.Set("access_token", null);
            Globals.Set("refresh_token", null);
            Globals.Set("access_token_expire_at", null);
            Globals.Set("refresh_token_expire_at", null);
            Globals.Set("TokenResponse", null);

            dynamic tr = JObject.Parse(tokenResponse);

            if (tr.token_type == "Bearer")
            {
                Globals.Set("access_token", tr.access_token.Value);
                Globals.Set("refresh_token", tr.refresh_token.Value);
                Globals.Set("access_token_expire_at", Utils.UnixTimeStampToDateTime(tr.consented_on.Value + tr.expires_in.Value).ToString("yyyy-MM-dd HH:mm"));
                Globals.Set("refresh_token_expire_at", Utils.UnixTimeStampToDateTime(tr.consented_on.Value + tr.refresh_token_expires_in.Value).ToString("yyyy-MM-dd HH:mm"));
            }
        }


        public void GetInvoiceNumber(string invoice_details)
        {
            Globals.Set("invoice_details", invoice_details);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string url = $"{Globals.Get("shaam_ita_url")}/Invoices/v1/Approval";

            headers.Add("Authorization", $"Bearer {Globals.Get("access_token")}");

            string response = null;
            string debug = null;
            try
            {
                response = Utils.HttpPostJson(url, invoice_details, out debug, headers);
                ProcessNumberResponse(response);
                Globals.Set("inumberResponse", $"{debug}\r\n🡆\r\n{response}".Replace("\r\n", "<br/>"));
                Response.Redirect("/?#5.2");
            }
            catch (Exception ex)
            {
                response = ex.ToString();

                Globals.Set("invoice_number", null);
                Globals.Set("inumberResponse", $"<div style='color:red;'>{response.Replace("\r\n", "<br/>")}</div>");
                Response.Redirect("/?#5.1");
            }
        }

        public void ResetInvoice()
        {
            Globals.Set("invoice_details", null);
            Globals.Set("inumberResponse", null);

            Response.Redirect("/?#5");
        }


        private OpenAPIDocument zzzCreateInvoice()
        {
            return new OpenAPIDocument
            {
                Invoice_ID = "72727890",
                Invoice_Reference_Number = "015345367",
                //"מספר תיק מע\"מ (ספק) בשלב זה יש לנסות הקצאת חשבוניות לעוסקים מדומים: 777777715, 777777723, 777777731,777777749"
                Vat_Number = 777777715,
                Branch_ID = "716A",
                Invoice_Date = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd"),
                Invoice_Issuance_Date = DateTime.Today.ToString("yyyy-MM-dd"),
                Invoice_Type = 305,
                Action = 0,

                //Customer details
                Customer_Name = "עלי אבוג'דידה",
                //"מספר תיק מע\"מ (ספק) בשלב זה יש לנסות הקצאת חשבוניות לעוסקים מדומים: 777777715, 777777723, 777777731,777777749"
                Customer_VAT_Number = 777777723,
                Client_Software_Key = "G4255422",
                
                Amount_Before_Discount = 1685.34,
                Discount = 202.24,
                Payment_Amount = 1483.10,
                VAT_Amount = 252.13,
                Payment_Amount_Including_VAT = 1735.22,

                Items = new List<OpenApiItem> 
                {
                    new OpenApiItem { Index  = 1,  Catalog_ID = "אא--72728", Description="דשא דרבן", Measure_Unit_Description="מ\"ר", Price_Per_Unit=22.56, Quantity=60.23, Discount=123.45, Total_Amount=1235.34, /*VAT_Amount=216.18,*/ VAT_Rate=17.00 },
                    new OpenApiItem { Index  = 2,  Catalog_ID = "הובלה 2679", Description="הובלה", Measure_Unit_Description="יח'", Price_Per_Unit=450.00, Quantity=1, Discount=0, Total_Amount=450, /*VAT_Amount=78.75,*/ VAT_Rate=17.00 },
                },
                Accounting_Software_Number = 4324243,
                Additional_Information = 4365,
                Invoice_Note = "לתאם הספקה עם אחמד 052-9290009",
            };
        }

        private OpenAPIDocument CreateInvoice()
        {
            var invoice = new OpenAPIDocument
            {
                Invoice_ID = "72727890",
                Invoice_Reference_Number = "015345367",
                //"מספר תיק מע\"מ (ספק) בשלב זה יש לנסות הקצאת חשבוניות לעוסקים מדומים: 777777715, 777777723, 777777731,777777749"
                Vat_Number = 777777715,
                Branch_ID = "716A",
                Invoice_Date = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd"),
                Invoice_Issuance_Date = DateTime.Today.ToString("yyyy-MM-dd"),
                Invoice_Type = 305,
                Action = 0,

                //Customer details
                Customer_Name = "עלי אבוג'דידה",
                //Use any VAT number but it must be valid (i.e. ספרת ביקורת חייבת להיות נכונה)
                Customer_VAT_Number = 111111118,
                Client_Software_Key = "G4255422",

                Accounting_Software_Number = 207703,
                Additional_Information = 4365,
                Invoice_Note = "לתאם הספקה עם אחמד 052-9290009",

                Items = new List<OpenApiItem>()
            };

            double vatRate = 17.00;

            for (int i = 1; i <= 10; i++)
            {
                double pricePerUnit = 2.89 * i;
                double quantity = 9.526 * i;
                double discount = 1.654 * i;
                double total = pricePerUnit * quantity - discount;

                //You must round all sums to 2 digits:
                var line = new OpenApiItem { Index = i, Catalog_ID = $"אא--{i+72728}", Description = "דשא דרבן", Measure_Unit_Description = "מ\"ר", Price_Per_Unit = pricePerUnit.Round2(),
                    Quantity = quantity.Round2(), Discount = discount.Round2(), Total_Amount = total.Round2(), VAT_Amount= (total * vatRate / 100.00).Round2(), VAT_Rate = vatRate  };

                invoice.Items.Add(line);
            }

            invoice.Amount_Before_Discount       = invoice.Items.Sum(e => e.Total_Amount).Round2();
            invoice.Discount                     = 202.24;
            invoice.Payment_Amount               = (invoice.Amount_Before_Discount - invoice.Discount).Round2();
            invoice.VAT_Amount                   = (invoice.Payment_Amount * vatRate / 100.00).Round2();
            invoice.Payment_Amount_Including_VAT = (invoice.Payment_Amount + invoice.VAT_Amount).Round2();

            return invoice;
        }

        private string ProcessNumberResponse(string response)
        {
            /*
             {
                "Status": 200,
                "Message": "Invoice approved",
                "Confirmation_Number": "20230817135055647229010608"
            }
            */

            /*
             {
                "Invoice_ID":"72727890","Invoice_Type":305,"Vat_Number":111111118,"Union_Vat_Number":0,"Invoice_Reference_Number":"015345367","Customer_VAT_Number":11111118,"Customer_Name":"עלי אבוגדידה","Invoice_Date":"2023-08-15","Invoice_Issuance_Date":"2023-08-17","Branch_ID":"716A","Accounting_Software_Number":4324243,"Client_Software_Key":"G4255422","Amount_Before_Discount":1314.09,"Discount":200,"Payment_Amount":1114.09,"VAT_Amount":194.97,"Payment_Amount_Including_VAT":1309.06,"Invoice_Note":"לתאם הספקה עם אחמד 052-9290009","Action":0,"Vehicle_License_Number":0,"Transition_Location":0,"Delivery_Address":"","Additional_Information":4365,"Items":[{"Index":1,"Catalog_ID":"אא--72728","Category":199999,"Description":"דשא דרבן","Measure_Unit_Description":"מ\"ר","Quantity":60.23,"Price_Per_Unit":22.56,"Discount":123.45,"Total_Amount":1235.34,"VAT_Rate":17,"VAT_Amount":216.18},{"Index":2,"Catalog_ID":"הובלה 2679","Category":199999,"Description":"הובלה","Measure_Unit_Description":"","Quantity":1,"Price_Per_Unit":450,"Discount":0,"Total_Amount":450,"VAT_Rate":17,"VAT_Amount":78.75}]
             }

                {"Status":200,
                    "Message":[{"errors":[{"code":432,"message":"Customer VAT Number is incorrect","param":"Customer_VAT_Number","location":"validation"}]},{"errors":[{"code":436,"message":"VAT amount is greater than the calculated VAT","param":"VAT_Amount","location":"validation"}]}],"Confirmation_Number":0}
             */
            dynamic tr = JObject.Parse(response);

            if (response.Contains("errors"))
            {
                throw new Exception(response);
            }

            if (tr["Status"] == "200" && tr["Message"] == "Invoice approved")
            {
                string inumber = tr["Confirmation_Number"];
                Globals.Set("invoice_number", inumber);
                return inumber;
            }

            throw new Exception(response);
        }
    }
}