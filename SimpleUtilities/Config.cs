using System.ComponentModel;

namespace SimpleUtilities
{
    public class Config
    {
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not to show debug messages.")]
        public bool Debug { get; set; } = false;

        [Description("Welcome message which is displayed for the player. (Leave it empty to disable.)")]
        public string WelcomeMessage { get; set; } = "<color=red>Welcome to the server!</color> <color=blue>Please read our rules.</color>";

        [Description("Welcome message duration.")]
        public ushort WelcomeMessageTime { get; set; } = 7;

        [Description("CASSIE announcement when Chaos Insurgency spawns. Leave it empty to disable. (Please note that you can only use CASSIE approved words.)")]
        public string CassieMessage { get; set; } = "Warning Chaos Insurgency has been spotted";

        [Description("Whether or not to play the announcement's sound effect.")]
        public bool CassieNoise { get; set; } = true;

        [Description("Whether or not CASSIE should display the text of the announcement.")]
        public bool CassieText { get; set; } = true;

        [Description("Chance (1-100) for Chaos Insurgency to spawn at round start instead of Facility Guards.")]
        public int ChaosChance { get; set; } = 25;

        [Description("Whether or not to enable friendly fire when the round ends. (You can change the friendly_fire_multiplier in your config_gameplay.txt)")]
        public bool FFOnEnd { get; set; } = true;

        [Description("Whether or not cuffed NTF / CI should change teams.")]
        public bool CuffedChangeTeams { get; set; } = true;
    }
}