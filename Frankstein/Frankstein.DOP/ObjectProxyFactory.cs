using System;
using System.Collections.Generic;
using ImpromptuInterface;

namespace Frankstein.DOP
{
    public static class ObjectProxyFactory
    {
        private static readonly Dictionary<Type, object> Builders = new Dictionary<Type, object>();

        public static FluentBuilder<TInterface> Configure<TInterface>(object target, bool supressErrors = true)
            where TInterface : class
        {
            if (!typeof(TInterface).IsInterface)
                throw new ArgumentException("TInterface");

            /* automatic duck typing if needed */
            var typedTarget = target.GetType().Implements(typeof(TInterface))
                ? (TInterface)target
                : target.ActLike<TInterface>();

            return new FluentBuilder<TInterface>(typedTarget, supressErrors);
        }

        /// <summary>
        /// Extension method to Save the current FluentBuilder configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public static void Save<T>(this FluentBuilder<T> builder) where T : class
        {
            if (builder != null)
                Builders[typeof(T)] = builder;
        }

        /// <summary>
        /// Get the Saved instance of FluentBuilder for the Type T and invokes its Build() method.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public static TInterface CreateProxy<TInterface>(object target = null) where TInterface : class
        {
            if (Builders.ContainsKey(typeof(TInterface)))
            {
                var builder = (FluentBuilder<TInterface>)Builders[typeof(TInterface)];
                return builder.CreateProxy();
            }

            return Configure<TInterface>(target).CreateProxy();
        }

        public static TInterface GetProxy<TInterface>(this object toProxy) where TInterface : class
        {
            toProxy = CreateProxy<TInterface>(toProxy);
            return (TInterface)toProxy;
        }

        public static void CleanUp()
        {
            Builders.Clear();
        }
    }
}