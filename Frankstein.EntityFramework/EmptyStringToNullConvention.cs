using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Frankstein.EntityFramework
{
    public class EmptyStringToNullConvention : Convention
    {
        public EmptyStringToNullConvention()
        {
            Properties().Having(x => x.GetCustomAttributes(false).OfType<EmptyStringToNull>().FirstOrDefault())
                            .Configure((cfg, att) => cfg.IsOptional());
        }
    }
}