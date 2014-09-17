using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Frankstein.DOP
{
    /// <summary>
    /// A class for Fluent configuration of a proxy object
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class FluentBuilder<TInterface> where TInterface : class
    {
        private readonly TInterface _target;
        private readonly List<Action<AspectContext<TInterface>>> _preAspects = new List<Action<AspectContext<TInterface>>>();
        private readonly List<Action<AspectContext<TInterface>>> _postAspects = new List<Action<AspectContext<TInterface>>>();
        private object _parameters;
        private Action<AspectException> _exceptionsCallBack;
        private string[] _methodsFilter;
        private Expression<Action<TInterface>>[] _lambdas;
        private readonly bool _supressErrors;

        internal FluentBuilder(TInterface target, bool supressErrors = true)
        {
            _target = target;
            _supressErrors = supressErrors;
        }

        public FluentBuilder<TInterface> AddPreDecoration(Action<AspectContext<TInterface>> preAspect)
        {
            _preAspects.Add(preAspect);
            return this;
        }
        public FluentBuilder<TInterface> AddPostDecoration(Action<AspectContext<TInterface>> postAspect)
        {
            _postAspects.Add(postAspect);
            return this;
        }

        public FluentBuilder<TInterface> SetParameters(object parameters)
        {
            _parameters = parameters;
            return this;
        }

        public FluentBuilder<TInterface> FilterMethods(params string[] methods)
        {
            _lambdas = null;
            _methodsFilter = methods;
            return this;
        }

        public FluentBuilder<TInterface> FilterMethods(params Expression<Action<TInterface>>[] methods)
        {
            _methodsFilter = null;
            _lambdas = methods;
            return this;
        }

        public FluentBuilder<TInterface> SetCallBack(Action<AspectException> exceptionsCallback)
        {
            _exceptionsCallBack = exceptionsCallback;
            return this;
        }

        public TInterface CreateProxy()
        {
            if (_lambdas != null)
            {
                _methodsFilter = ObjectProxyHelper.GetMethodNames(_lambdas);
            }

            var objectProxy = new ObjectProxy<TInterface>(
                _target,
                _preAspects,
                _postAspects,
                _parameters,
                _exceptionsCallBack,
                _supressErrors,
                _methodsFilter);

            return objectProxy.Proxy; //GetTransparentProxy()
        }
    }
}