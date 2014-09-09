using System.Data.Entity;
using System.Web.WebPages;

namespace Frankstein.Common.Mvc.ExtJs
{
    public class ExtJsCreator<TContext, TEntity> : ExtJsCrudBase<TContext, TEntity>
        where TContext : DbContext, new()
        where TEntity : class, new()
    {
        public ExtJsCreator(WebPageRenderingBase page)
            : base(page)
        {
        }

        public override ExtJsResult ExecuteQuery()
        {
            Context.Entry(Entity).State = EntityState.Added;
            Context.SaveChanges();

            return CreateResult();
        }
    }
}