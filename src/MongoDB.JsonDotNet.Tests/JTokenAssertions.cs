/* Copyright 2015 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace MongoDB.JsonDotNet.Tests
{
    public static class JTokenExtensions
    {
        public static JTokenAssertions Should(this Newtonsoft.Json.Linq.JToken actualValue)
        {
            return new JTokenAssertions(actualValue);
        }
    }

    public class JTokenAssertions : ReferenceTypeAssertions<Newtonsoft.Json.Linq.JToken, JTokenAssertions>
    {
        // constructors
        public JTokenAssertions(Newtonsoft.Json.Linq.JToken value)
        {
            Subject = value;
        }

        // methods
        public AndConstraint<JTokenAssertions> Be(Newtonsoft.Json.Linq.JToken expected, string because = "", params object[] reasonArgs)
        {
            Execute.Assertion
                .BecauseOf(because, reasonArgs)
                .ForCondition(Subject.IsSameOrEqualTo(expected))
                .FailWith("Expected {context:object} to be {0}{reason}, but found {1}.", expected, Subject);

            return new AndConstraint<JTokenAssertions>(this);
        }

        public AndConstraint<JTokenAssertions> NotBe(Newtonsoft.Json.Linq.JToken unexpected, string because = "", params object[] reasonArgs)
        {
            Execute.Assertion
                .BecauseOf(because, reasonArgs)
                .ForCondition(!Subject.IsSameOrEqualTo(unexpected))
                .FailWith("Did not expect {context:object} to be equal to {0}{reason}.", unexpected);

            return new AndConstraint<JTokenAssertions>(this);
        }

        protected override string Context
        {
            get { return "JToken"; }
        }
    }
}
