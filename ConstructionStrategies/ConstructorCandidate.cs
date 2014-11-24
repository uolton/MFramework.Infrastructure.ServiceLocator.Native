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

namespace MFramework.Infrastructure.ServiceLocator.Native.ConstructionStrategies
{
    public class ConstructorCandidate
    {
        public ConstructorCandidate()
        {
            Parameters = new List<ParameterSummary>();
        }

        public Type Type { get; set; }
        public List<ParameterSummary> Parameters { get; set; }
        public Func<object[], object> Instantiate { get; set; }
    }

    public class ParameterSummary
    {
        public int Position { get; set; }
        public Type ParameterType { get; set; }
        public string Name { get; set; }
    }
}