#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Library;
using tradelr.Library;

namespace tradelr.Libraries.Helpers
{
    public static class CheckBoxListHelper
    {
        public static List<CheckBoxListInfo> ToCheckBoxList<T>(this List<T> obj)
        {
            var type = obj.GetType().GetGenericArguments()[0];
            if (!type.IsEnum)
            {
                throw new InvalidOperationException(type.Name + " is not a list of enumerations");
            }
            var list = new List<CheckBoxListInfo>();
            var values = Enum.GetValues(type);
            foreach (var entry in values)
            {
                bool isChecked = false;
                var e = (T) entry;
                if (obj.Contains<T>(e))
                {
                    isChecked = true;
                }
                var info = new CheckBoxListInfo(((Enum)entry).ToInt().ToString(), ((Enum)entry).ToDescriptionString(), isChecked);
                list.Add(info);
            }
            return list;
        }

    }
}