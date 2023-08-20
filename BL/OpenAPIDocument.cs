using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxesDemo.BL
{
    /// <summary>
    /// See https://www.gov.il/BlobFolder/generalpage/hor-software-other/he/IncomeTax_software-houses-040723.pdf
    /// </summary>
    public class OpenAPIDocument
    {
        [Required]
        /// <summary>
        /// ה-ID של המסמך בכספית
        /// </summary>        
        public string Invoice_ID { get; set; }

        [Required]
        /// <summary>
        /// 0 חשבונית/חשבונית עסקה לא
        /// 305 חשבונית-מס כן
        /// 306 ** חשבונית-מס I לא עתידי, לא קיים בקובץ במבנה אחיד
        /// 310 חשבונית מס ריכוז כן
        /// 320 חשבונית מס / קבלה כן
        /// 330 חשבונית מס זיכוי כן
        /// 331 ** חשבונית מס זיכוי I לא עתידי, לא קיים בקובץ במבנה אחיד
        /// 332 ** חשבון עסקה/פרופורמה כן ראה הסבר בסעיף 3.5
        /// </summary>
        public int Invoice_Type { get; set; } = 305;

        [Required]
        /// <summary>
        /// מספר עוסק מורשה של מפיק המסמך
        /// </summary>
        public int Vat_Number { get; set; }
        public int Union_Vat_Number { get; set; }
        public string Invoice_Reference_Number { get; set; }
        /// <summary>
        /// מספר ע"מ או ח.פ של הלקוח
        /// </summary>
        public int Customer_VAT_Number { get; set; }
        /// <summary>
        /// שם הלקוח
        /// </summary>
        public string Customer_Name { get; set; } = "";

        [Required]
        /// <summary>
        /// תאריך המסמך YYYY-MM-DD. עד שבועיים לפני התאריך הנוכחי
        /// </summary>
        public string Invoice_Date { get; set; }

        [Required]
        /// <summary>
        /// מועד הפקת המסמך YYYY-MM-DD
        /// </summary>
        public string Invoice_Issuance_Date { get; set; }

        /// <summary>
        /// מספר סניף
        /// </summary>
        public string Branch_ID { get; set; } = "";

        [Required]
        /// <summary>
        /// מספר הרישום של כספית
        /// </summary>
        public int Accounting_Software_Number { get; set; } = 0;

        /// <summary>
        /// ה-ID של הלקוח בכספית
        /// </summary>
        public string Client_Software_Key { get; set; } = "";

        [Required]
        /// <summary>
        /// סכום לפני הנחת המסמך
        /// </summary>
        public double Amount_Before_Discount { get; set; } = 0;

        [Required]
        /// <summary>
        /// הנחת המסמך. תמיד חיובי. מה עושעם אם ההנחה היא חיובית (עיגול כלפי מעלה)?
        /// </summary>
        public double Discount { get; set; } = 0;

        [Required]
        /// <summary>
        /// סכום סופי ללא מע"מ
        /// </summary>
        public double Payment_Amount { get; set; } = 0;

        [Required]
        /// <summary>
        /// סכום המע"מ
        /// </summary>
        public double VAT_Amount { get; set; } = 0;

        [Required]
        /// <summary>
        /// סכום סופי כולל מע"מ
        /// </summary>
        public double Payment_Amount_Including_VAT { get; set; }

        public string Invoice_Note { get; set; } = "";
        public int Action { get; set; } = 0;
        public int Vehicle_License_Number { get; set; }

        /// <summary>
        /// מספר הנייד של הנה
        /// לפי הסכימה צריך להיות string אבל המציאות דורשים int
        /// {"Status":400,"Message":{"errors":[{"value":"","msg":"value should be numeric of type int ","param":"Phone_Of_Driver","location":"body"}]},"Error_Id":"11889014972"} 
        /// </summary>
        //public int Phone_Of_Driver { get; set; } = 0;

        /// <summary>
        /// תאריך הגעה YYYY-MM-DD
        /// </summary>
        //public string Arrival_Date { get; set; } = "";

        ///<summary>
        ///שעת Estimated_Arrival_Time משוערת HH:MM
        ///</summary>
        //public string Estimated_Arrival_Time { get; set; } = "";

        /// <summary>
        /// מיקום המעבר על פי טבלת קודים - עתידי
        /// </summary>
        public int Transition_Location { get; set; } = 0;
        /// <summary>
        /// כתובת אספקה - עתידי
        /// </summary>
        public string Delivery_Address { get; set; } = "";
        /// <summary>
        /// לשימוש עתידי
        /// </summary>
        public int Additional_Information { get; set; }
        /// <summary>
        /// שורות החשבונית
        /// </summary>
        public OpenApiItem[] Items { get; set; }
    }

    public class OpenApiItem
    {
        /// <summary>
        /// מספר השורה
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// מק"ט
        /// </summary>
        public string Catalog_ID { get; set; }
        /// <summary>
        /// קטגורית הפריט. מורכב מ-6 תווים. הראשון 1 עבור טובין או 2 עבור שירות. 5 נוספים:
        /// 00001 בעלי חיים ומוצריהם
        /// 00002 מוצרי ירקות/פירות
        /// 00003 מאכלים
        /// 00004 מוצרים מינרליים
        /// 00005 כימיקלים
        /// 00006 מוצרי פלסטיק/גומי
        /// 00007 פרוות, מוצרי עור
        /// 00008 עצים ומוצרי עץ
        /// 00009 מוצרי טקסטיל )בגדים וביגוד(
        /// 00010 אביזרים לראש ולרגל
        /// 00011 מוצרי אבן/זכוכית
        /// 00012 מתכות
        /// 00013 מכונות/מוצרי חשמל
        /// 00014 תחבורה
        /// 99999 שונות
        /// </summary>
        //public int Category { get; set; } = 199999;
        /// <summary>
        /// תיאור הפריט
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// יחידת המידה: ק"ג, שק, מ"ר וכו'
        /// </summary>
        [MaxLength(30)]
        public string Measure_Unit_Description { get; set; }
        /// <summary>
        /// כמות. לדוגמה 5.67
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// מחיר ליחידה בש"ח
        /// </summary>
        public double Price_Per_Unit { get; set; }
        /// <summary>
        /// הנחת שורה בש"ח
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// סכום השורה ללא מע"מ פחות הנחת שורה
        /// </summary>
        public double Total_Amount { get; set; }
        /// <summary>
        /// שיעור המע"מ: 17.50 עבור 17.5%
        /// </summary>
        public double VAT_Rate { get; set; }
        /// <summary>
        ///  סכום המע"מ
        /// </summary>
        //public double VAT_Amount { get; set; }
    }
}