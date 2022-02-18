// Orbit API Copyright(C) 2019-2021 DragonFruit Network
// Licensed under the MIT License - see the LICENSE file at the root of the project for more info

using Newtonsoft.Json;
using System;

namespace Koapower.KoafishTwitchBot.Module.OsuWebApi
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class OsuBeatmapset : DragonFruit.Orbit.Api.Beatmaps.Entities.OsuBeatmapset
    {
        [JsonProperty("hype")]
        public HypeData Hype { get; set; }

        public class HypeData
        {
            public uint? current { get; set; }
            public uint? required { get; set; }
        }
    }
}
