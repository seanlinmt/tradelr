///
/// See COPYING file for licensing information
///

using System;
using System.ComponentModel;
using System.Reflection;

namespace com.mosso.cloudfiles.utils
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDescription(Enum enumType)
        {
            Type type = enumType.GetType();

            MemberInfo[] memInfo = type.GetMember(enumType.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute) attrs[0]).Description;
            }

            return enumType.ToString();
        }
    }
}