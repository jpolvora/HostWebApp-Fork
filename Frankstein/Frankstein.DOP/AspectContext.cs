using System.Runtime.Remoting.Messaging;

namespace Frankstein.DOP
{
    /// <summary>
    /// The context that will be available during interception.
    /// </summary>
    public class AspectContext
    {
        public dynamic Target { get; protected set; }
        public IMethodCallMessage CallCtx { get; protected set; }
        public dynamic Parameters { get; set; }

        public bool Abort { get; set; }

        public AspectContext(object target, IMethodCallMessage callCtx, object parameters)
        {
            Target = target;
            CallCtx = callCtx;
            Parameters = parameters;
        }
    }

    public class AspectContext<TTarget> : AspectContext where TTarget : class
    {
        public new TTarget Target
        {
            get
            {
                return base.Target as TTarget;
            }
        }


        public AspectContext(TTarget target, IMethodCallMessage callCtx, object parameters)
            : base(target, callCtx, parameters)
        {
        }
    }
}