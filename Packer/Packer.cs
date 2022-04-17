using System.Globalization;
using System.Linq;
using System.Text;
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
                //Some format providers to allow for differences in locale format.
                var numberStyle = NumberStyles.AllowDecimalPoint|NumberStyles.AllowCurrencySymbol;
                var formatProvider = new CultureInfo("fr-FR")
                {
                    NumberFormat =
                    {
                        NumberDecimalSeparator = "."
                    }
                };
                //Localized function to do some string to decimal conversion
                decimal ConvertDecimal(string d)
                {
                    if(!decimal.TryParse(d, numberStyle, formatProvider, out var res))
                        throw new APIException("Invalid number format --" + d.GetType().Name);

                    return res;
                }
                //Get the contents of the file
                var str = File.ReadAllLines(fileLocation,Encoding.UTF8);
                
                //Initiate the output container

                var packages = new PackagesContainer
                {
                    LineItems = new List<ItemsContainer>( )
                };

               

                //Loop and prep each package.
                foreach (var line in str)
                {
                    //Get max weight package can take
                    var packageMaxWeight= ConvertDecimal(line.Substring(0, line.IndexOf(":", StringComparison.Ordinal)).Trim());
                    //Get the line item and filter out the items then project to a model so that we can filter. Also make sure that the weight limit does not exeed 100
                    //Remember tolist at the end to prevent multiple enumerations
                    var items = Regex
                        .Matches(line, @"\((.*?)\)")
                        .Select(x => x.Groups[1].Value?.Split(','))?
                        .Select(x => new PackItemModel
                        {
                            Index = x[0].TryParseInt32(),
                            Weight = ConvertDecimal(x[1]),
                            Cost = ConvertDecimal(x[2]),
                        }).Where(m => m.Weight <= 100&&m.Weight<=packageMaxWeight)
                        .OrderByDescending(o => o.Cost).ThenBy(t=>t.Weight)?.ToList()??Enumerable.Empty<PackItemModel>().ToList();
                    
                    var package = new ItemsContainer
                    {
                        PackageItems = new List<PackItemModel>()
                    };
                    
                    foreach (var pack in items)
                    {
                        package.PackageItems.Add(pack);
                        if (package.PackageItems.Sum(x => x.Weight) > 100)
                            break;
                    }
                    packages.LineItems.Add(package);
                }

                var itemCount = packages.LineItems.SelectMany(x => x.PackageItems).Count();

                var outputString = new StringBuilder();
                outputString.Append(itemCount).Append("\n_\n");
                    
                foreach (var lineItem in packages.LineItems)
                {
                    if (!lineItem.PackageItems.Any())
                        continue;

                    foreach (var (packageItem,Index) in lineItem.PackageItems.Select((packageItem,Index)=>(packageItem,Index)))
                    {
                        outputString.Append(packageItem.Index);
                        if(Index<lineItem.PackageItems.Count()-1)
                            outputString.Append(",");
                    }

                    outputString.Append("\n");
                }

                return outputString.ToString();
            }
            catch (Exception ex)
            {
                throw new APIException(ex);
            }
            
           
        }
    }
}