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

namespace Pesta.Engine.auth
{
    /// <summary>
    /// Summary description for SecurityTokenException
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class SecurityTokenException : Exception
    {
        public SecurityTokenException(String message)
            : base(message)
        {
        }
        public SecurityTokenException(Exception cause)
            : base("", cause)
        {

        }
        public SecurityTokenException(String message, Exception cause)
            : base(message, cause)
        {

        }
    }
}