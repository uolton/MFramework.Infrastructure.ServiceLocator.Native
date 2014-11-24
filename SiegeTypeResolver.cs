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
using System.Linq;
using MFramework.Infrastructure.ServiceLocator.Exceptions;
using MFramework.Infrastructure.ServiceLocator.Native.ConstructionStrategies;
using MFramework.Infrastructure.ServiceLocator.Native.Maps;
using MFramework.Infrastructure.ServiceLocator.Resolution;

namespace MFramework.Infrastructure.ServiceLocator.Native
{
    public class SiegeTypeResolver
	{
		private readonly IConstructionStrategy strategy;
		protected ResolutionMap resolutionMap = new ResolutionMap();

		public SiegeTypeResolver(IConstructionStrategy strategy)
		{
			this.strategy = strategy;
		}

		public void Register(Type from, Type to)
		{
			Register(from, to, null);
		}

		public void Register(Type from, object instance)
		{
			Register(from, instance, null);
		}

		public virtual void Register(Type from, object instance, string key)
		{
			this.resolutionMap.InstanceMap.Add(from, instance, key);
		}

		public virtual void Register(Type from, Type to, string key)
		{
			this.resolutionMap.TypeMap.Add(from, to, key);
		    var mappedType = resolutionMap.TypeMap.GetMappedType(to, key);
			if(mappedType != null) strategy.Register(to, mappedType, resolutionMap);
		}

		public virtual void RegisterWithFactoryMethod(Type from, Func<object> to, string key)
		{
			this.resolutionMap.FactoryMap.Add(from, to, key);
		}

		public void RegisterWithFactoryMethod(Type from, Func<object> to)
		{
			RegisterWithFactoryMethod(from, to, null);
		}

		public object Get(Type type, string key, ConstructorParameter[] parameters)
		{
			if (resolutionMap.FactoryMap.Contains(type))
			{
			    var instance = resolutionMap.FactoryMap.Get(type, key);
                if (instance != null) return instance;
			}
			if (resolutionMap.InstanceMap.Contains(type)) return resolutionMap.InstanceMap.Get(type, key);

			if (!resolutionMap.TypeMap.Contains(type) && type.IsClass)
			{
				resolutionMap.TypeMap.Add(type, type, null);
			}

			var mappedType = resolutionMap.TypeMap.GetMappedType(type, key);

            if (mappedType == null) return null;

			var candidate = SelectConstructor(mappedType, resolutionMap, parameters);

			var constructorArgs = new object[candidate.Parameters.Count];
            var candidateParameters = candidate.Parameters;
		    var parameterCount = candidateParameters.Count;
			for(int i = 0; i < parameterCount; i++)
			{
                var arg = candidateParameters[i];
			    object value = null;
                value = parameters.All(x => x.Name != arg.Name) ? Get(arg.ParameterType, null, new ConstructorParameter[0]) : parameters.First(x => x.Name == arg.Name).Value;

				if (value != null)
				{
					constructorArgs[arg.Position] = value;
				}
				else
				{
                    if (parameters.Length == 0) throw new RegistrationNotFoundException(type);

					for (int j = 0; j < parameters.Length; j++)
					{
					    var parameter = parameters[j];
						if (parameter.Name == arg.Name)
						{
							constructorArgs[arg.Position] = parameter.Value;
						}
					}
				}
			}

			if (!strategy.CanConstruct(candidate))
			{
			    strategy.Register(candidate.Type, mappedType, resolutionMap);
			}

			return strategy.Create(candidate, constructorArgs);  
		}

		public object Get(Type type, ConstructorParameter[] parameters)
		{
			return Get(type, null, parameters);
		}

		public bool IsRegistered(Type type)
		{
			return resolutionMap.Contains(type);
		}

		public IEnumerable<object> GetAll(Type type)
		{
            var list = new List<object>();
            var registrations = resolutionMap.GetAllRegisteredTypesMatching(type);

            for (int i = 0; i < registrations.Count; i++)
            {
                var registration = registrations[i];
				list.Add(Get(registration, null, new ConstructorParameter[] { }));
			}

			return list;
		}

		public IEnumerable<TService> GetAll<TService>()
		{
			var list = new List<TService>();

		    var registrations = resolutionMap.GetAllRegisteredTypesMatching(typeof (TService));
			for (int i = 0; i < registrations.Count; i++)
			{
			    var registration = registrations[i];
				list.Add((TService)Get(registration, null, new ConstructorParameter[] { }));
			}

			return list;
		}

		private static ConstructorCandidate SelectConstructor(MappedType type, ResolutionMap map, ConstructorParameter[] parameters)
		{
			var candidates = type.Candidates;
			var candidateCount = candidates.Count;
			ConstructorCandidate candidate = null;

			for (int i = 0; i < candidateCount; i++)
			{
				candidate = candidates[i];
				var summaries = candidate.Parameters;
				var summaryCount = summaries.Count;

				for (int j = 0; j < summaryCount; j++)
				{
					var summary = summaries[j];
					var parameterType = summary.ParameterType;

					if (!map.Contains(parameterType))
					{
						int parameterCount = parameters.Length;

						for (int k = 0; k < parameterCount; k++)
						{
							var parameter = parameters[k];
							if (parameterType.IsAssignableFrom(parameter.Value.GetType())) return candidate;
						}
					}
					else
					{
						return candidate;
					}
				}
			}

			return candidate;
		}
	}
}