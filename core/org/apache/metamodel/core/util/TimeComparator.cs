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
// https://github.com/apache/metamodel/blob/b0cfe3aed447769f752743ac1753ebed90adaad2/core/src/main/java/org/apache/metamodel/util/TimeComparator.java
using org.apache.metamodel.j2cs.data.date_time;
using org.apache.metamodel.j2cs.data.numbers;
using org.apache.metamodel.j2cs.exceptions;
using org.apache.metamodel.j2cs.slf4j;
using org.apache.metamodel.j2cs.types;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace org.apache.metamodel.util
{
    /**
     * Compares dates of various formats. Since this class has unchecked generic
     * conversion it can compare java.util.Date, java.sql.Date, java.sql.Time,
     * java.util.Calendar, Date-formatted strings etc.
     */
    public sealed class TimeComparator : IComparer<Object>
    {
        private static readonly Logger logger = LoggerFactory.getLogger(typeof(TimeComparator).Name);

        private static readonly String[] prototypePatterns = 
                                         { "yyyy-MM-dd HH:mm:ss.SSS", "yyyy-MM-dd HH:mm:ss",
                                           "yyyy-MM-dd HH:mm", "HH:mm:ss.SSS", "yyyy-MM-dd",
                                           "dd-MM-yyyy", "yy-MM-dd", "MM-dd-yyyy", "HH:mm:ss", "HH:mm"
                                         };

        private static readonly IComparer<Object> _instance = new TimeComparator();

        public static IComparer<Object> getComparator()
        {
            return _instance;
        }

        private TimeComparator()
        {
        }

        //[J2Cs] Implementation helper class which replaces the anonymous interface implementation class in Java
        private class _Time_Comparable_Impl_ : IComparable<object>
        {
            private CsDate _dt1_;
            private IComparer<object> _instance_;

            public _Time_Comparable_Impl_(IComparer<object> instance_arg, CsDate dt1_arg)
            {
                _dt1_      = dt1_arg;
                _instance_ = instance_arg;
            } // constructor

            public bool equals(object obj)
            {
                return _instance.Equals(obj);
            }

            public int CompareTo(object o)
            {
                return _instance.Compare(_dt1_, o);
            }

            public String toString()
            {
                return "TimeComparable[time=" + _dt1_ + "]";
            }
        } // _Time_Comparable_Impl_

        public static IComparable<object> getComparable(object o)
        {
            CsDate dt1 = toDate(o);
            return new _Time_Comparable_Impl_(_instance, dt1);
        } // getComparable()

        public int Compare(Object o1, Object o2)
        {
                if (o1 == null && o2 == null)
                {
                    return 0;
                }
                if (o1 == null)
                {
                    return -1;
                }
                if (o2 == null)
                {
                    return 1;
                }
                try
                {
                    CsDate dt1 = toDate(o1);
                    CsDate dt2 = toDate(o2);
                    return CsDate.CompareTo(dt1, dt2);
                }
                catch (Exception e)
                {
                    logger.error("Could not compare {} and {}", o1, o2);
                    throw new CsSystemException(e.Message);
                }
        } // Compare()

        public static CsDate toDate(object value)
        {
            CsDate result = null;
            if (value == null)
            {
                result = null;
            }
            else if (value is CsDate) {
                result = (CsDate)value;
            }
            else if (value is Calendar)
            {
                // https://www.tutorialspoint.com/java/util/calendar_gettime.htm
                result = new CsDate(DateTime.Now);
            }
            else if (value is String)
            {
                result = convertFromString((String)value);
            }
            else if (value is CsNumber)
            {
                result = convertFromNumber((CsNumber)value);
            }

            if (result == null)
            {
                logger.warn("Could not convert '{}' to date, returning null", value);
            }

            return result;
        }

        public static bool isTimeBased(object o)
        {
            if (o is CsDate || o is Calendar) {
                return true;
            }
            return false;
        }

    private static CsDate convertFromString(String value)
    {
        try
        {
            long long_value = long.Parse(value);
            return convertFromNumber(new CsNumber(long_value));
        }
        //catch (NumberFormatException e)
        #pragma warning disable 0168
        catch (FormatException e)
        {
            // do nothing, proceed to next method of conversion
        }
        #pragma warning restore 0168

        // try with Date.toString() date format first
        try
        {
            // https://msdn.microsoft.com/fr-fr/library/system.globalization.datetimeformatinfo.shortdatepattern(v=vs.110).aspx
            // https://mdsaputra.wordpress.com/2012/02/01/c-convert-datetime-to-formated-string-formated-string-to-datetime-with-datetimeformatinfo/
            // DateFormatSymbols dateFormatSymbols = DateFormatSymbols.getInstance(Locale.US);
            CultureInfo        ci     = new CultureInfo("en-US");
            DateTimeFormatInfo dtfi   = ci.DateTimeFormat;
            dtfi.LongTimePattern = "EEE MMM dd HH:mm:ss zzz yyyy";
            //SimpleDateFormat   dateFormat  = new SimpleDateFormat("EEE MMM dd HH:mm:ss zzz yyyy", dtfi);
            //return dateFormat.parse(value);
            return new CsDate(DateTime.ParseExact(value, "T", dtfi));
        }
        catch (CsParseException e)
        {
            // do noting
        }

        foreach (String prototype_pattern in prototypePatterns)
        {
            if (prototype_pattern.Length == value.Length)
            {
                DateTimeFormatInfo dtfi;
                try
                {
                    dtfi = CsDateUtils.createDateFormat(prototype_pattern);
                    return new CsDate(value, prototype_pattern, dtfi); //  dateFormat.parse(value);
                }
                catch (Exception e)
                {
                    // proceed to next formatter
                }

                if (prototype_pattern.IndexOf('-') != -1)
                {
                    // try with '.' in stead of '-'
                    try
                    {
                        dtfi = CsDateUtils.createDateFormat(prototype_pattern.Replace("\\-", "\\."));
                        return new CsDate(value, prototype_pattern, dtfi); //dateFormat.parse(value);
                    }
                    catch (Exception e)
                    {
                        // proceed to next formatter
                    }

                    // try with '/' in stead of '-'
                    try
                    {
                        dtfi = CsDateUtils.createDateFormat(prototype_pattern.Replace("\\-", "\\/"));
                        return new CsDate(value, prototype_pattern, dtfi); // dateFormat.parse(value);
                    }
                    catch (Exception e)
                    {
                        // proceed to next formatter
                    }
                }
            }
        }
        return null;
    } // convertFromString()

    private static CsDate convertFromNumber(CsNumber value)
    {
        CsNumber numberValue  = (CsNumber)value;
        long     long_value   = numberValue.asLong();

        CsNumber n            = new CsNumber(long_value);
        String   string_value = n.ToString();
        // test if the number is actually a format of the type yyyyMMdd
        if (string_value.Length == 8 && (string_value.StartsWith("1") || string_value.StartsWith("2")))
        {
            try
            {
                string format = "yyyyMMdd";
                DateTimeFormatInfo dtfi = CsDateUtils.createDateFormat(format);
                return new CsDate(string_value, format, dtfi);
            }
            catch (Exception e)
            {
                // do nothing, proceed to next method of conversion
            }
        }

        // test if the number is actually a format of the type yyMMdd
        if (string_value.Length == 6)
        {
            try
            {
                string format = "yyMMdd";
                DateTimeFormatInfo dtfi = CsDateUtils.createDateFormat(format);
                return new CsDate(string_value, format, dtfi);
            }
            #pragma warning disable 0168
            catch (Exception e)
            {
                // do nothing, proceed to next method of conversion
            }
            #pragma warning restore 0168
            }

            if (long_value > 5000000)
            {
                // Java: this number is most probably amount of milliseconds since 1970
                // C#:   this number is most probably amount of milliseconds since 1900
                CsDate d = new CsDate(DateTime.Now);
                return new CsDate(long_value);
            }
            else
            {
                // Java: this number is most probably amount of milliseconds since 1970
                // C#:   this number is most probably amount of milliseconds since 1900
                return new CsDate(long_value * 1000 * 60 * 60 * 24);
            }
    } // convertFromNumber()
    } // TimeComparaorClass
} // org.apache.metamodel.util namespace
