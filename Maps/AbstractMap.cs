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
    public abstract class AbstractMap<TMapList> where TMapList : AbstractMapList
    {
        protected Dictionary<Type, TMapList> entries = new Dictionary<Type, TMapList>();

        public List<Type> GetRegisteredTypesMatching(Type type) 
        {
            var list = new Type[entries.Keys.Count];
            entries.Keys.CopyTo(list, 0);
            var types = new List<Type>();

            for (int i = 0; i < list.Length; i++)
            {
                var key = list[i];
                var item = entries[key];
                var itemType = item.Type;

                if (itemType == type || itemType.IsAssignableFrom(type)) types.Add(type);
            }

            return types;
        }
    }

    public abstract class AbstractMapList
    {
        public Type Type { get; set; }
    }
}