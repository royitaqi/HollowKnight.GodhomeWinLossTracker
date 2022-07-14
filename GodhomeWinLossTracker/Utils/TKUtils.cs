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
                // System
                HeroController.instance.acceptingInput,
                false,
                false,
                false,
                false,
                false,
                false,
                false,

                // State
                HeroController.instance.hero_state == GlobalEnums.ActorStates.idle,
                HeroController.instance.hero_state == GlobalEnums.ActorStates.running,
                HeroController.instance.hero_state == GlobalEnums.ActorStates.airborne,
                HeroController.instance.hero_state == GlobalEnums.ActorStates.hard_landing,
                false,
                false,
                HeroController.instance.cState.touchingWall,
                false,
                
                // Action
                HeroController.instance.cState.attacking,
                HeroController.instance.cState.nailCharging,
                HeroController.instance.cState.focusing,
                false,
                false,
                false,
                false,
                false,
                
                // Movement
                HeroController.instance.cState.dashing, // This includes normal dash and shadow dash (HeroController.instance.cState.shadowDashing)
                HeroController.instance.cState.jumping || HeroController.instance.cState.doubleJumping,
                HeroController.instance.cState.falling,
                HeroController.instance.cState.superDashing || HeroController.instance.cState.superDashOnWall,
                HeroController.instance.cState.wallSliding,
                false,
                false,
                false, // Leave the highest bit unused
            };
            return bits.Zip(Enumerable.Range(0, bits.Length), (bit, index) => bit ? (1 << index) : 0).Sum();
        }
    }
}
