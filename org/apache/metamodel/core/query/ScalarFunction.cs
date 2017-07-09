﻿using org.apache.metamodel.data;
using org.apache.metamodel.query;
using System;
using System.Collections.Generic;
using System.Text;
/**
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
namespace org.apache.metamodel.query
{
    /**
     * Interface that contains scalar specific methods.
     *
     * Scalar functions returns only a single value, based on the input value.
     *
     */
    public interface ScalarFunction : FunctionType
    {
        /**
         * Applies and evaluates the function on a particular row of data.
         * 
         * @param row
         *            the row containing data
         * @param parameters
         *            any parameters associated with the function call
         * @param operandItem
         *            the select item which is the argument to this function. If a
         *            function takes multiple select items, this will be the primary
         *            one.
         * @return
         */
        Object evaluate(Row row, Object[] parameters, SelectItem operandItem);
    } // ScalarFunction
} // org.apache.metamodel.schema namespace