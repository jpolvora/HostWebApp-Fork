using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Frankstein.EntityFramework
{
    public class DecimalMappingAttributeConvention : Convention
    {
        public DecimalMappingAttributeConvention()
        {
            Properties().Having(x => x.GetCustomAttributes(false).OfType<DecimalMappingAttribute>().FirstOrDefault())
                .Configure((cfg, att) => cfg.HasPrecision(att.Precision, att.Scale));
        }
    }
}