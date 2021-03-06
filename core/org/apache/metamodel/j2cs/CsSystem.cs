﻿/**
* Licensed to the Apache Software Foundation (ASF) under one
* or more contributor license agreements.  See the NOTICE file
* distributed with this work for additional information
* regarding copyright ownership.  The ASF licenses this file
* to you under the Apache License, Version 2.0 (the
* "License"); you may not use this file except in compliance
* with the License.  You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied.  See the License for the
* specific language governing permissions and limitations
* under the License.
*/
using org.apache.metamodel.j2cs.types;
using System;

namespace org.apache.metamodel.j2cs
{
    public class CsSystem
    {
        // http://www.java2s.com/Tutorials/CSharp/System.Reflection/MethodInfo/C_MethodInfo_GetBaseDefinition.htm
        public static int IdentityHashCode(object o)
        {
            return o.GetHashCode();
        } // identityHashCode()

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        } // Cast()
    } // CsSystem class
} // org.apache.metamodel.j2cs namespace
