﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker
{
    [Serializable]
    public class FolderData
    {
        public List<RawWinLoss> RawRecords = new();
    }

    [Serializable]
    public class GlobalData
    {
        public bool AutoExport = false;
    }

    [Serializable]
    public class LocalData
    {
        public int ProfileID = 0;
    }
}
