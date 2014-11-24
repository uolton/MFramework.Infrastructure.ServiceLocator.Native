/*   Copyright 2009 - 2010 Marcus Bratton

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
*/

using System;
using System.Collections.Generic;
using MFramework.Infrastructure.ServiceLocator.Exceptions;
using MFramework.Infrastructure.ServiceLocator.ExtensionMethods;
using MFramework.Infrastructure.ServiceLocator.Native.ConstructionStrategies;
using MFramework.Infrastructure.ServiceLocator.Resolution;

namespace MFramework.Infrastructure.ServiceLocator.Native
{
    public class SiegeAdapter : IServiceLocatorAdapter
    {
        private readonly SiegeTypeResolver resolver;

        public SiegeAdapter() : this(new SiegeProxyConstructionStrategy())
        {
        }

        public SiegeAdapter(IConstructionStrategy strategy)
        {
            resolver = new SiegeTypeResolver(strategy);
            resolver.Register(typeof(SiegeTypeResolver), resolver, null);
        }

        public void Dispose()
        {
        }

        public object GetInstance(Type type, string key, params IResolutionArgument[] parameters)
        {
            object value = resolver.Get(type, key, GetConstructorParameters(parameters));

            if (value == null) throw new RegistrationNotFoundException(type);

            return value;
        }

        public object GetInstance(Type type, params IResolutionArgument[] parameters)
        {
            object value = resolver.Get(type, GetConstructorParameters(parameters));
            if (value == null) throw new RegistrationNotFoundException(type);

            return value;
        }

        public TService GetInstance<TService>(Type type, params IResolutionArgument[] arguments)
        {
            return (TService) GetInstance(type, arguments);
        }

        public TService GetInstance<TService>(string key, params IResolutionArgument[] arguments)
        {
            return (TService)GetInstance(typeof(TService), key, arguments);
        }

        public TService GetInstance<TService>(params IResolutionArgument[] arguments)
        {
            return (TService)GetInstance(typeof(TService), arguments);
        }

        public ConstructorParameter[] GetConstructorParameters(IResolutionArgument[] parameters)
        {
            int parameterCount = 0;
            var parameterList = parameters.OfType<ConstructorParameter, IResolutionArgument>();
            for (int i = 0; i < parameterList.Length; i++)
            {
                parameterCount++;
            }

            var constructorParameters = new ConstructorParameter[parameterCount];
            int currentParameterIndex = 0;

            for (int i = 0; i < parameters.Length; i++)
            {
                IResolutionArgument argument = parameters[i];
                if(argument is ConstructorParameter)
                {
                    constructorParameters[currentParameterIndex] = (ConstructorParameter)argument;
                    currentParameterIndex++;
                }
            }

            return constructorParameters;
        }

        public bool HasTypeRegistered(Type type)
        {
            return resolver.IsRegistered(type);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return resolver.GetAll(serviceType);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return resolver.GetAll<TService>();
        }

        public void Register(Type from, Type to)
        {
            if(from != to) resolver.Register(from, to);
            resolver.Register(to, to);
        }

        public void RegisterInstance(Type type, object instance)
        {
            resolver.Register(type, instance);
        }

        public void RegisterWithName(Type from, Type to, string name)
        {
            resolver.Register(from, to, name);
        }

        public void RegisterInstanceWithName(Type type, object instance, string name)
        {
            resolver.Register(type, instance, name);
        }

        public void RegisterFactoryMethod(Type type, Func<object> func)
        {
            resolver.RegisterWithFactoryMethod(type, func);
        }

        public void RegisterFactoryMethodWithName(Type type, Func<object> func, string name)
        {
            resolver.RegisterWithFactoryMethod(type, func, name);
        }
    }
}