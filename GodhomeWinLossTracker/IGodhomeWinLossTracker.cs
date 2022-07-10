using Modding;

namespace GodhomeWinLossTracker
{
    internal interface IGodhomeWinLossTracker : Modding.ILogger, IDataHolder, IGlobalSettings<GlobalData>, ILocalSettings<LocalData>, ICustomMenuMod
    {
    }
}
