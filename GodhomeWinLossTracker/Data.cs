using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker
{
    [Serializable]
    public class LocalData
    {
        public List<RawWinLoss> RawRecords = new();
    }

    [Serializable]
    public class GlobalData
    {
        public int myGlobalData = 999;
    }

    public class EmptyLocalData { }
}
