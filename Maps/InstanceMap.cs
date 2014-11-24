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
	public class InstanceMap : AbstractMap<InstanceMapList>
	{
		public void Add(Type from, object to, string key)
		{
			if(!entries.ContainsKey(from))
			{
				var list = new InstanceMapList(from);
				list.Add(to, key);
				entries.Add(from, list);
			}
		}

		public bool Contains(Type type)
		{
		    return entries.ContainsKey(type);
		}

		public object Get(Type type, string name)
		{
            var entry = (InstanceMapList)entries[type];
            var instances = entry.MappedInstances;

            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];

                if (instance.Name == name) return instance.To;
            }

            return instances[0].To;
		}
	}

    public class InstanceMapList : AbstractMapList
	{
		private List<object> registeredInstances = new List<object>();
		public List<MappedInstance> MappedInstances { get; private set; }

		public InstanceMapList(Type type)
		{
			this.Type = type;
			this.MappedInstances = new List<MappedInstance>();
		}

		public void Add(object to, string name)
		{
			if (this.registeredInstances.Contains(to)) return;

			this.registeredInstances.Add(to);
			this.MappedInstances.Add(new MappedInstance(this.Type, to, name));
		}
	}

	public class MappedInstance
	{
		public string Name { get; private set; }
		public Type From { get; private set; }
		public object To { get; private set; }

		public MappedInstance(Type from, object to, string name)
		{
			this.Name = name;
			this.From = from;
			this.To = to;
		}
	}
}