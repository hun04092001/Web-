using System.Globalization;

namespace SV20T1080012.Web.AppCodes
{
    public class Converter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? StringToDateTime(string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
