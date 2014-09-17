using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Frankstein.DOP
{
    internal class ObjectProxy<TInterface> : RealProxy, IRemotingTypeInfo where TInterface : class
    {
        private readonly IEnumerable<Action<AspectContext<TInterface>>> _preAspects;
        private readonly IEnumerable<Action<AspectContext<TInterface>>> _postAspects;
        private readonly bool _supressErrors;
        private readonly String[] _arrMethods;
        private readonly dynamic _parameters;
        private readonly Action<AspectException> _exceptionsCallback;
        private readonly TInterface _target;

        public TInterface Proxy { get; private set; }

        protected internal ObjectProxy(
            TInterface target,
            IEnumerable<Action<AspectContext<TInterface>>> preAspects,
            IEnumerable<Action<AspectContext<TInterface>>> postAspects,
            object parameters,
            Action<AspectException> exceptionsCallback,
            bool supressErrors = true,
            params string[] arrMethods)
            : base(typeof(TInterface)) //: base(typeof(MarshallByRefObject)) 
        {
            _target = target;
            _preAspects = preAspects;
            _postAspects = postAspects;
            _parameters = parameters;
            _exceptionsCallback = exceptionsCallback;
            _supressErrors = supressErrors;
            _arrMethods = arrMethods;

            Proxy = (TInterface)base.GetTransparentProxy();
            TypeName = string.Format("{0}_{1}", Proxy.GetType().Name, _target.GetType().Name);
        }

        public override sealed object GetTransparentProxy()
        {
            return Proxy;
        }

        public override IMessage Invoke(IMessage message)
        {
            var methodMessage = (IMethodCallMessage)message;
            var method = methodMessage.MethodBase;

            if (!HasMethod(method.Name))
                return CreateReturnMessage(InvokeOriginalMethod(methodMessage, false), methodMessage);

            var aspectContext = new AspectContext<TInterface>(_target, methodMessage, _parameters);

            var exceptions = new List<Exception>();

            // Perform the preprocessing
            foreach (var preAspect in _preAspects)
            {
                var exception = ExecuteAspect(preAspect, aspectContext);
                if (exception != null)
                    exceptions.Add(exception);
            }

            // Perform the call
            var returnValue = InvokeOriginalMethod(methodMessage, aspectContext.Abort);

            // Perform the postprocessing

            foreach (var postAspect in _postAspects)
            {
                var exception = ExecuteAspect(postAspect, aspectContext);
                if (exception != null)
                    exceptions.Add(exception);
            }

            if (_exceptionsCallback != null && exceptions.Count > 0)
            {
                var aspectException = new AspectException(exceptions);
                _exceptionsCallback(aspectException);
            }


            return CreateReturnMessage(returnValue, methodMessage);
        }

        /// <summary>
        /// Executes an aspect
        /// </summary>
        private Exception ExecuteAspect(Action<AspectContext<TInterface>> aspect, AspectContext<TInterface> context)
        {
            if (context.Abort)
                return null;

            try
            {
                if (aspect != null)
                    aspect.Invoke(context);
                return null;
            }
            catch (Exception exception)
            {
                if (_supressErrors)
                    return exception;

                throw;
            }
        }

        /// <summary>
        /// Call the current method being intercepted
        /// </summary>
        private object InvokeOriginalMethod(IMethodMessage methodMessage, bool abort, bool tryWrap = false)
        {
            try
            {
                if (abort)
                    return null;

                if (methodMessage.MethodBase.IsConstructor && tryWrap)
                {

                }

                var result = methodMessage.MethodBase.Name == "GetType"
                    ? typeof(TInterface)
                    : methodMessage.MethodBase.Invoke(_target, methodMessage.Args);

                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        /// <summary>
        /// Creates the return IMessage to the Invoke method
        /// </summary>
        private static ReturnMessage CreateReturnMessage(object returnValue, IMethodCallMessage methodMessage)
        {
            return new ReturnMessage(returnValue,
                methodMessage.Args,
                methodMessage.ArgCount,
                methodMessage.LogicalCallContext,
                methodMessage);
        }

        /// <summary>
        /// Will check if a method is going to be intercepted. Will return true if empty or null or contains the name.
        /// </summary>
        private bool HasMethod(string mtd)
        {
            return _arrMethods == null || !_arrMethods.Any() || _arrMethods.Any(s => s.Equals(mtd));
        }

        public override ObjRef CreateObjRef(Type type)
        {
            throw new NotSupportedException("ObjRef for DynamicProxy isn't supported");
        }

        public bool CanCastTo(Type fromType, object realproxy) // refers to current TransparentProxy object
        {
            var result = fromType == typeof(TInterface) || fromType == _target.GetType() ||
                         typeof(TInterface).IsAssignableFrom(fromType) || _target.GetType().Implements(fromType);
            return result;
        }

        public string TypeName { get; set; }
    }
}