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
	public class FactoryMap : AbstractMap<FactoryMapList>
	{
		public void Add(Type from, Func<object> to, string key)
		{
			if (!entries.ContainsKey(from))
			{
				var list = new FactoryMapList(from);
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
            var entry = (FactoryMapList)entries[type];
            var factories = entry.MappedFactories;
			
            for (int i = 0; i < factories.Count; i++)
            {
                var factory = factories[i];

                if (factory.Name == name) return factory.To();
            }

            if(!string.IsNullOrEmpty(name)) return null;

			return factories[0].To();
		}
	}

	public class FactoryMapList : AbstractMapList
	{
		private List<Func<object>> registeredFactories = new List<Func<object>>();
		public List<MappedFactory> MappedFactories { get; private set; }

		public FactoryMapList(Type type)
		{
			this.Type = type;
			this.MappedFactories = new List<MappedFactory>();
		}

		public void Add(Func<object> to, string key)
		{
			if (this.registeredFactories.Contains(to)) return;

			this.registeredFactories.Add(to);
			this.MappedFactories.Add(new MappedFactory(this.Type, to, key));
		}
	}

	public class MappedFactory
	{
		public string Name { get; private set; }
		public Type From { get; private set; }
		public Func<object> To { get; private set; }

		public MappedFactory(Type from, Func<object> to, string key)
		{
			this.From = from;
			this.To = to;
			this.Name = key;
		}
	}
}