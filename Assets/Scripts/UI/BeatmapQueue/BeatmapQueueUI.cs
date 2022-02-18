using DragonFruit.Orbit.Api.Beatmaps.Entities;
using Koapower.KoafishTwitchBot.Common.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using OsuBeatmapset = Koapower.KoafishTwitchBot.Module.OsuWebApi.OsuBeatmapset;

namespace Koapower.KoafishTwitchBot.UI.BeatmapQueue
{
    public class BeatmapQueueUI : UIPage
    {
        [SerializeField] Slot slotPrefab;

        ObjectPool<Slot> slotPool;
        List<Slot> slots = new List<Slot>();

        public void Enqueue(OsuBeatmapset mapset, OsuBeatmap map = null)
        {
            var slot = slotPool.Get();
            slot.gameObject.SetActive(true);
            slot.ResetUI();
            slot.Setup(this, mapset, map);
        }

        private void Awake()
        {
            slotPool = new ObjectPool<Slot>(() => GameObject.Instantiate(slotPrefab, slotPrefab.transform.parent));
            slotPrefab.gameObject.SetActive(false);
        }

        internal void CloseSlot(Slot slot)
        {
            slot.ResetUI();
            slot.gameObject.SetActive(false);
            slotPool.Return(slot);
            slots.Remove(slot);
        }
    }
}
