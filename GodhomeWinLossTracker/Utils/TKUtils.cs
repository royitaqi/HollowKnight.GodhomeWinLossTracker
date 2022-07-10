using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.Utils
{
    internal class TKUtils
    {
        public static int GetTKStatus()
        {
            bool[] bits = new[]
            {
                //
                HeroController.instance.acceptingInput,
                HeroController.instance.controlReqlinquished,
                PlayerData.instance.atBench,
                false,
                false,
                false,
                false,
                false,
                // action
                HeroController.instance.cState.attacking,
                HeroController.instance.cState.dashing,
                HeroController.instance.cState.jumping || HeroController.instance.cState.doubleJumping || HeroController.instance.cState.falling, // airborne
                HeroController.instance.cState.superDashing,
                HeroController.instance.cState.swimming,
                HeroController.instance.cState.recoiling,
                false,
                false,
                // wall
                HeroController.instance.wallLocked,
                HeroController.instance.cState.wallJumping,
                HeroController.instance.cState.touchingWall,
                HeroController.instance.cState.wallSliding,
                false,
                false,
                false,
                false,
                //
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                // Leave one bit unused
            };
            return bits.Zip(Enumerable.Range(0, bits.Length), (bit, index) => bit ? (1 << index) : 0).Sum();
        }
    }
}
