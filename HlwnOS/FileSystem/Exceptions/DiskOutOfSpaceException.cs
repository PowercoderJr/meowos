﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlwnOS.FileSystem.Exceptions
{
    class DiskOutOfSpaceException : Exception
    {
        public DiskOutOfSpaceException() : base("На диске нет места")
        {

        }
    }
}
