﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Raven
{
    public static class Constants
    {
        public const int LargeBatchSize = 1024 * 4;
        public const int SmallBatchSize = 512;
        public const string ActionsPluginFolder = @".\Actions";
    }
}