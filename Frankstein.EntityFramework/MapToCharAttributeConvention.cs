using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Frankstein.EntityFramework
{
    public class MapToCharAttributeConvention : Convention
    {
        public MapToCharAttributeConvention()
        {
            Properties().Having(x => x.GetCustomAttributes(false).OfType<MapToCharAttribute>().FirstOrDefault())
                .Configure((cfg, att) => cfg.HasColumnType("char").HasMaxLength(att.FixedLength).IsFixedLength());
        }
    }
}