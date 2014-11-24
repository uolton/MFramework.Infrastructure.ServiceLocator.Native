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
using MFramework.Infrastructure.ServiceLocator.ExtensionMethods;
using MFramework.Infrastructure.ServiceLocator.Native.Maps;
using Siege.TypeGenerator;

namespace MFramework.Infrastructure.ServiceLocator.Native.ConstructionStrategies
{
	public abstract class SiegeActivator
	{
		public abstract object Instantiate(object[] args);
	}

	public class SiegeProxyConstructionStrategy : IConstructionStrategy
	{
		private readonly Dictionary<Type, SiegeActivator> activators = new Dictionary<Type, SiegeActivator>();

		public bool CanConstruct(ConstructorCandidate candidate)
		{
		    return activators.ContainsKey(candidate.Type);
		}

		public object Create(ConstructorCandidate candidate, object[] parameters)
		{
		    var activator = activators[candidate.Type];
            return activator.Instantiate(parameters);
		}

		public void Register(Type to, MappedType mappedType, ResolutionMap resolutionMap)
		{
            if(activators.ContainsKey(to)) return;

			var generator = new Siege.TypeGenerator.TypeGenerator();

		    var candidates = mappedType.Candidates;
		    int candidateCount = candidates.Count;
            for (int candidateCounter = 0; candidateCounter < candidateCount; candidateCounter++)
            {
                var candidate = candidates[candidateCounter];
                if (activators.ContainsKey(candidate.Type)) continue;

                var activatorType = generator.CreateType(context =>
                {
                    context.Named(Guid.NewGuid() + "Builder");
                    context.InheritFrom<SiegeActivator>();

                    context.OverrideMethod<SiegeActivator>(activator => activator.Instantiate(null), method => method.WithBody(body =>
                    {
                        var instance = body.CreateVariable(to);
                        var array = body.CreateArray(typeof(object));
                        array.AssignFromParameter(new MethodParameter(0));
                        var items = new List<ILocalIndexer>();

                        var parameters = candidate.Parameters;
                        var parameterCount = parameters.Count;
                        for (int i = 0; i < parameterCount; i++)
                        {
                            var info = parameters[i];
                            var arg1 = array.LoadValueAtIndex(info.ParameterType, body, info.Position);
                            items.Add(arg1);
                        }

                        var constructorArgs = new Type[candidate.Parameters.Count];
                        var candidateParameters = candidate.Parameters;
                        var candidateParameterCount = candidateParameters.Count;
                        for (int i = 0; i < candidateParameterCount; i++)
                        {
                            var arg = candidateParameters[i];

                            constructorArgs[arg.Position] = arg.ParameterType;
                        }

                        instance.AssignFrom(body.Instantiate(to, constructorArgs, items.OfType<ILocalIndexer, ILocalIndexer>()));
                        body.Return(instance);
                    }));
                });

                var constructor = activatorType.GetConstructor(new Type[] { });

                activators.Add(candidate.Type, (SiegeActivator)constructor.Invoke(new object[] { }));
            }
		}
	}
}