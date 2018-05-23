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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Library;
using tradelr.Library;

namespace tradelr.Libraries.Helpers
{
    public static class SelectListHelper
    {
        public static SelectList ToSelectList(this IEnumerable<SelectListItem> values)
        {
            return values.ToSelectList(null);
        }

        public static SelectList ToSelectList(this IEnumerable<SelectListItem> values, object selectedValue)
        {
            return values.ToSelectList(selectedValue, "None", "");
        }

        public static SelectList ToSelectList(this IEnumerable<SelectListItem> values, 
            object selectedValue, string emptyText, string emptyValue)
        {
            var result = values.ToList();
            result.Insert(0, new SelectListItem { Text = emptyText, Value = emptyValue });
            if (selectedValue == null)
            {
                return new SelectList(result, "Value", "Text");
            }
            return new SelectList(result, "Value", "Text", selectedValue);
        }

        public static SelectList ToSelectList(this Type type)
        {
            return type.ToSelectList(false, null, null, true, null);
        }

        public static SelectList ToSelectList(this Type type, bool order)
        {
            return type.ToSelectList(false, null, null, order, null);
        }

        public static SelectList ToSelectList(this Type type, bool order, bool useNameVal)
        {
            return type.ToSelectList(useNameVal, null, null, order, null);
        }

        public static SelectList ToSelectList(this Type type, bool useNameVal, string emptyText, string emptyValue, string selectedValue = "")
        {
            return type.ToSelectList(useNameVal, emptyText, emptyValue, true, selectedValue);
        }

        public static SelectList ToSelectList(this Type type, bool useNameVal, string emptyText, string emptyValue, bool order, string selectedValue)
        {
            var enumvalues = Enum.GetValues(type);
            List<SelectListItem> values = new List<SelectListItem>();
            foreach (Enum value in enumvalues)
            {
                values.Add(new SelectListItem()
                               {
                                   Text = value.ToDescriptionString(),
                                   Value = useNameVal?value.ToString(): value.ToInt().ToString()
                               });
            }

            List<SelectListItem> includeDash;
            if (order)
            {
                includeDash = values.OrderBy(x => x.Text).ToList();
            }
            else
            {
                includeDash = values.ToList();
            }

            if (emptyText != null)
            {
                includeDash.Insert(0, new SelectListItem {Text = emptyText, Value = emptyValue});    
            }

            return new SelectList(includeDash, "Value", "Text", selectedValue);
        }
    }
}