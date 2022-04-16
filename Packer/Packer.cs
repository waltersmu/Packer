using System.Text.RegularExpressions;

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
                var str = File.ReadAllLines(fileLocation);


                foreach (var line in str)
                {
                    var items = Regex
                        .Matches(line, @"\((.*?)\)")
                        .Select(x => x.Groups[1].Value)
                        .ToList();
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new APIException(ex);
            }
            
           
        }
    }
}