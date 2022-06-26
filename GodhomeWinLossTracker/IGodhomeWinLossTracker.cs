using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Modding;
using UnityEngine;
using GodhomeWinLossTracker.MessageBus;
using GodhomeWinLossTracker.MessageBus.Messages;
using Newtonsoft.Json;
using Vasi;

namespace GodhomeWinLossTracker
{
    internal interface IGodhomeWinLossTracker : Modding.ILogger, IDataHolder, IGlobalSettings<GlobalData>, ILocalSettings<LocalData>, ICustomMenuMod
    {
    }
}
