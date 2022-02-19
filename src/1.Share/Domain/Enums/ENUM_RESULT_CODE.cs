﻿using eXtensionSharp;

namespace Domain.Enums
{
    public class ENUM_RESULT_CODE : XEnumBase<ENUM_RESULT_CODE>
    {
        public static ENUM_RESULT_CODE OK = Define("0");
        public static ENUM_RESULT_CODE FAIL = Define("-1");
    }
}