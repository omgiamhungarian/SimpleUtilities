using System.Collections.Generic;
using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using Respawning;
using PlayerRoles;
using MEC;
using GameCore;
using UnityEngine;
using PlayerStatsSystem;
using System;
using HarmonyLib;
using Log = PluginAPI.Core.Log;
using Random = UnityEngine.Random;

namespace SimpleUtilities
{
    public class EventHandlers
    {
        int randomNumber;

        //Welcome message.
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(Player player)
        {
            Config config = SimpleUtilities.Singleton.Config;
            Broadcast.Singleton.TargetAddElement(player.ReferenceHub.characterClassManager.connectionToClient, config.WelcomeMessage, config.WelcomeMessageTime, Broadcast.BroadcastFlags.Normal);
        }

        //Cassie announcement on Chaos Insurgency Spawn.
        [PluginEvent(ServerEventType.TeamRespawn)]
        public void OnRespawn(SpawnableTeamType team, List<Player> players, int time)
        {
            Config config = SimpleUtilities.Singleton.Config;

            if (team == SpawnableTeamType.ChaosInsurgency)
            {
                Cassie.Message(config.CassieMessage, true, config.CassieNoise, config.CassieText);
            }
        }

        //Chaos Insurgency spawn on round start.

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        void WaitingForPlayers()
        {
            randomNumber = Random.Range(1, 100);
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void OnChangeRole(Player plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            int chance = SimpleUtilities.Singleton.Config.ChaosChance;

            if (randomNumber > chance)
                return;

            if (reason != RoleChangeReason.RoundStart && reason != RoleChangeReason.LateJoin)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (newRole == RoleTypeId.FacilityGuard)
                {
                    plr.SetRole(RoleTypeId.ChaosRifleman);
                }
            });
        }

        //Auto Friendly Fire on Round end. FF detector is disabled by default.
        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            if (!SimpleUtilities.Singleton.Config.FfOnEnd)
            {
                return;
            }

            Server.FriendlyFire = true;
            float restartTime = ConfigFile.ServerConfig.GetFloat("auto_round_restart_time");

            Timing.CallDelayed(restartTime - 0.5f, () =>
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

                    if (!target.IsDisarmed)
                    {
                        yield break;
                    }

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
                                yield break;
                            case Team.FoundationForces:
                                target.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Escaped);
                                yield break;
                        }
                    }
                }
            }
        }

        //Hint when player becomes SCP-096's target.
        [PluginEvent(ServerEventType.Scp096AddingTarget)]
        public void OnScp096Target(Player player, Player target, bool IsForLooking)
        {
            target.ReceiveHint(SimpleUtilities.Singleton.Config.TargetMessage, 5f);
        }

        //Coin flip hints.
        [PluginEvent(ServerEventType.PlayerCoinFlip)]
        void OnCoinFlip(Player player, bool isTails)
        {
            Timing.CallDelayed(1.4f, () =>
            {
                if (isTails)
                {
                    player.ReceiveHint(SimpleUtilities.Singleton.Config.CoinTails);
                }
                else
                {
                    player.ReceiveHint(SimpleUtilities.Singleton.Config.CoinHeads);
                }
            });
        }

        //Show HP when looking at Player.
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void InitialHealth(Player plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (!SimpleUtilities.Singleton.Config.ShowHp)
            {
                return;
            }

            if (newRole == RoleTypeId.Spectator || newRole == RoleTypeId.None)
            {
                return;
            }

            Timing.CallDelayed(0.1f, () =>
            {
                plr.CustomInfo = $"HP: {(int)Math.Ceiling(plr.Health)}/{(int)plr.MaxHealth}";
            });
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void DamagedHealth(Player plr, Player attacker, DamageHandlerBase damageHandler)
        {
            if (!SimpleUtilities.Singleton.Config.ShowHp)
            {
                return;
            }

            if (plr.Role == RoleTypeId.Spectator || plr.Role == RoleTypeId.None)
            {
                return;
            }

            Timing.CallDelayed(0.5f, () =>
            {
                plr.CustomInfo = $"HP: {(int)Math.Ceiling(plr.Health)}/{(int)plr.MaxHealth}";
            });
        }

        //There is no event when the player is healed.
        [HarmonyPatch(typeof(HealthStat), "ServerHeal")] //Thanks davidsebesta for the patch!
        public class HealedHealthPatch
        {
            private static float lastUpdate;
            private const float UpdateDelay = 0.75f;
            //When using items that grant regeneration
            //The patch updates every single frame, hence the delay.
            //MEC doesn't seem to work.

            public static void Postfix(HealthStat __instance, ref float healAmount)
            {

                if (!SimpleUtilities.Singleton.Config.ShowHp)
                {
                    return;
                }

                ReferenceHub refHub = GetHub(__instance);
                if (refHub == null)
                {
                    return;
                }

                Player plr = Player.Get(refHub.gameObject);

                if (Time.time - lastUpdate > UpdateDelay)
                {
                    lastUpdate = Time.time;
                    plr.CustomInfo = $"HP: {(int)Math.Ceiling(plr.Health)}/{(int)plr.MaxHealth}";
                }
            }

            private static ReferenceHub GetHub(HealthStat healthStat)
            {
                StatBase statBase = healthStat;
                if (statBase != null)
                {
                    return statBase.Hub;
                }
                return null;
            }
        }
    }
}