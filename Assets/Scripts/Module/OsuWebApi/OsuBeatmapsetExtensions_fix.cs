﻿using DragonFruit.Orbit.Api;
using DragonFruit.Orbit.Api.Beatmaps.Requests;
using System.Threading.Tasks;

namespace Koapower.KoafishTwitchBot.Module.OsuWebApi
{
    public static class OsuBeatmapsetExtensions_fix
    {
        /// <summary>
        /// Get a beatmapset's metadata/info from its id
        /// </summary>
        /// <param name="client">The <see cref="OrbitClient"/> to use</param>
        /// <param name="setId">The id of the set</param>
        public static Task<OsuBeatmapset> GetBeatmapset_fix(this OrbitClient client, uint setId)
        {
            var request = new OsuBeatmapsetRequest
            {
                Id = setId
            };

            return client.PerformAsync<OsuBeatmapset>(request);
        }
    }
}