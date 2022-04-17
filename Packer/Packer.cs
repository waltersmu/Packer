using System.Globalization;
using System.Text.RegularExpressions;
using Packer;
using static System.Globalization.NumberStyles;

namespace com.mobuquity.packer
{
    /// <summary>
    /// Primary class for the API 
    /// </summary>
    public class Packer
    {
        public static string Pack(string fileLocation)
        {
            if (string.IsNullOrEmpty(fileLocation)) throw new APIException();

            var fileUrl = fileLocation;

            try
            {

                var numberStyle = NumberStyles.AllowDecimalPoint|NumberStyles.AllowCurrencySymbol;
                var formatProvider = new CultureInfo("fr-FR")
                {
                    NumberFormat =
                    {
                        NumberDecimalSeparator = "."
                    }
                };

                decimal ConvertDecimal(string d)
                {
                    if(!decimal.TryParse(d, numberStyle, formatProvider, out var res))
                        throw new APIException("Invalid number format --" + d.GetType().Name);

                    return res;
                }

                var str = File.ReadAllLines(fileLocation);
                foreach (var line in str)
                {
                    var items = Regex
                        .Matches(line, @"\((.*?)\)")
                        .Select(x => x.Groups[1].Value?.Split(','))?
                        .Select(x=> new PackItem
                        {
                            Index = x[0].TryParseInt32(),
                            Weight =ConvertDecimal(x[1]),
                            Cost = ConvertDecimal(x[2]),
                        });
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new APIException(ex);
            }
            
           
        }
    }

    public class PackItem
    {
        public int Index { get; set; }
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }
    }


}