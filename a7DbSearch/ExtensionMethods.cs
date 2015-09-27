﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace a7DbSearch
{
    static class ExtensionMethods
    {

        public static bool ContainsSearchedValue(this List<a7SearchedValue> list, string valueName)
        {
            foreach (a7SearchedValue sv in list)
            {
                if (sv.Value == valueName)
                    return true;
            }
            return false;
        }
    }
}
