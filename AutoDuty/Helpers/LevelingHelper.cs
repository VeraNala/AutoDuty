﻿using ECommons.GameFunctions;

namespace AutoDuty.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class LevelingHelper
    {
        private static ContentHelper.Content[] levelingDuties = [];

        private static ContentHelper.Content[] LevelingDuties
        {
            get
            {
                if (levelingDuties.Length <= 0)
                {
                    uint[] ids =
                    [
                        1037u, // TamTara Deepcroft
                        1039u, // The Thousand Maws of Toto-Rak
                        1041u, // Brayflox's Longstop
                        1042u, // Stone Vigil
                        1064u, // Sohm Al
                        1142u, // Sirensong Sea
                        1144u, // Doma Castle
                        837u,  // Holminster
                        823u,  // Qitana
                        952u,  // Tower of Zot
                        974u,  // Ktisis Hyperboreia
                    ];
                    levelingDuties = ids.Select(id => ContentHelper.DictionaryContent.GetValueOrDefault(id)).Where(c => c != null).Cast<ContentHelper.Content>().Reverse().ToArray();
                }
                return levelingDuties;
            }
        }

        internal static unsafe ContentHelper.Content? SelectHighestLevelingRelevantDuty()
        {
            ContentHelper.Content? curContent = null;

            short lvl = PlayerHelper.GetCurrentLevelFromSheet();

            if (lvl < 15 || AutoDuty.Plugin.Player!.GetRole() == CombatRole.NonCombat || lvl >= 100)
                return null;

            short ilvl = PlayerHelper.GetCurrentItemLevelFromGearSet();
            
            if(lvl is >= 16 and < 91)
                foreach (ContentHelper.Content duty in LevelingDuties)
                    if (duty.CanRun(lvl, ilvl))
                        return duty;

            foreach ((uint _, ContentHelper.Content? content) in ContentHelper.DictionaryContent)
            {
                if (content.DawnContent)
                {
                    if (curContent == null || curContent.ClassJobLevelRequired < content.ClassJobLevelRequired)
                    {
                        if (content.CanRun(lvl, ilvl) && (content.ClassJobLevelRequired < 50 || content.ClassJobLevelRequired % 10 != 0))
                        {
                            curContent = content;
                        }
                    }
                }
            }

            return curContent ?? null;
        }
    }
}
