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

namespace MFramework.Infrastructure.ServiceLocator.Native.Maps
{
	public class ResolutionMap
	{
		public FactoryMap FactoryMap { get; private set; }
		public InstanceMap InstanceMap { get; private set; }
		public TypeMap TypeMap { get; private set; }

		public ResolutionMap()
		{
			this.FactoryMap = new FactoryMap();
			this.InstanceMap = new InstanceMap();
			this.TypeMap = new TypeMap();
		}

		public bool Contains(Type type)
		{
			return FactoryMap.Contains(type) || InstanceMap.Contains(type) || TypeMap.Contains(type);
		}

		public List<Type> GetAllRegisteredTypesMatching(Type type)
		{
			var types = new List<Type>();

			types.AddRange(FactoryMap.GetRegisteredTypesMatching(type));
			types.AddRange(InstanceMap.GetRegisteredTypesMatching(type));
			types.AddRange(TypeMap.GetRegisteredTypesMatching(type));

			return types;
		}
	}
}