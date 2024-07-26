using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities
{
    internal enum EBaseAddressList : int
    {
        GAME_VERSION = 0x004ED7F8,
        LOBBY_DATA = 0x00632CC0,
        RESULT_DATA = 0x00892418,
        PLAYER_NAME = 0x00632D94,
        CLIENT_STATUS = 0x004ED818, 
        PLAY_TIME = 0x0097591C, 
        HOST = 0x00C0D12C,
        INGAME_PLAYER_NUMBER = 0x00956771,
        INGAME_NATION = 0x00956770,
        SELECTED_NAME_DATA = 0x0066966C,
        NAME_DATA = 0x0066966E,
        MAP_NAME = 0x00632D56,

        GAME_VERSION_READSIZE = 16,
        LOBBY_READSIZE = 48,
        RESULT_READSIZE = 160,
        PLAYER_NAME_READSIZE = 120,
        CLIENT_STATUS_READSIZE = 1,
        PLAY_TIME_READSIZE = 4,
        HOST_READSIZE = 1,
        INGAME_PLAYER_NUMBER_READSIZE = 1,
        INGAME_NATION_READSIZE = 1,
        SELECTED_NAME_DATA_READSIZE = 1,
        NAME_DATA_READSIZE = 14,
        MAPNAME_READSIZE = 60
    }
}
