using System.Collections.Generic;
using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using Respawning;
using PlayerRoles;
using MEC;
using GameCore;
using UnityEngine;

namespace SimpleUtilities
{
    public class EventHandlers
    {
        //Welcome message.
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(Player player)
        {
            Config config = SimpleUtilities.Singleton.Config;          
            Broadcast.Singleton.TargetAddElement(player.ReferenceHub.characterClassManager.connectionToClient, config.WelcomeMessage, config.WelcomeMessageTime, Broadcast.BroadcastFlags.Normal);
        }
        
        //Cassie announcement on Chaos Insurgency Spawn.
        [PluginEvent(ServerEventType.TeamRespawn)]
        public void OnRespawn(SpawnableTeamType team)
        {
            Config config = SimpleUtilities.Singleton.Config;

            if (team == SpawnableTeamType.ChaosInsurgency)
            {
                Cassie.Message(config.CassieMessage, true, config.CassieNoise, config.CassieText);
            }
        }

        //Chaos Insurgency spawn on round start.
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void OnChangeRole(Player plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            Config config = SimpleUtilities.Singleton.Config;
            int chance = config.ChaosChance;
            System.Random random = new System.Random();

            if (random.Next(1, 100) > chance)
            {
                return;
            }
            Timing.CallDelayed(0.25f, () =>
            {
                foreach (var plyr in Player.GetPlayers())
                {
                    //This way you can still spawn Facility Guards from RA, if you need to.
                    if (newRole == RoleTypeId.FacilityGuard && (reason == RoleChangeReason.RoundStart || reason == RoleChangeReason.LateJoin))
                    {
                        plyr.SetRole(RoleTypeId.ChaosRifleman);
                    }
                }
            });
        }

        //Auto Friendly Fire on Round end. FF detector is disabled by default.
        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            if (!SimpleUtilities.Singleton.Config.FFOnEnd)
            {
                return;
            }
            
            Server.FriendlyFire = true;
            float restartTime = ConfigFile.ServerConfig.GetFloat("auto_round_restart_time");

            Timing.CallDelayed(restartTime-0.5f, () =>
            {
                Server.FriendlyFire = false;
            });
        }

        //Cuffed change teams.
        [PluginEvent(ServerEventType.PlayerHandcuff)]
        public void OnPlayerHandcuffed(Player player, Player target)
        {          
            if (!SimpleUtilities.Singleton.Config.CuffedChangeTeams)
            {
                return;
            }

            Timing.RunCoroutine(CuffedChangeTeams());

            IEnumerator<float> CuffedChangeTeams()
            {
                for (; ; )
                {
                    yield return Timing.WaitForSeconds(1.0f);

                    foreach (var plr in Player.GetPlayers())
                    {
                        if (!target.IsDisarmed || !(target.Team == Team.FoundationForces || target.Team == Team.ChaosInsurgency) || Vector3.Distance(target.Position, new Vector3(124, 988, 23)) > 5)
                        {
                            continue;
                        }

                        switch (target.Team)
                        {
                            case Team.ChaosInsurgency:
                                target.SetRole(RoleTypeId.NtfPrivate, RoleChangeReason.Escaped);
                                break;
                            case Team.FoundationForces:
                                target.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Escaped);
                                break;
                        }
                    }
                }
            }
        }
    }
}