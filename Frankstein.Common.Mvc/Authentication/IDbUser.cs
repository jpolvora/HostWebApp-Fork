using System.Collections.Generic;

namespace Frankstein.Common.Mvc.Authentication
{
    public interface IDbUser
    {
        List<string> GetRoles();
    }
}