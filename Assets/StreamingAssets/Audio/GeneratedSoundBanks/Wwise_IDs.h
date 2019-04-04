/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID ACHIEVEMENT_GOT = 3617657745U;
        static const AkUniqueID BGM = 412724365U;
        static const AkUniqueID BGM_STOP = 2090192256U;
        static const AkUniqueID COMBO_LOST = 1537667210U;
        static const AkUniqueID COMBO_TICK = 2058410279U;
        static const AkUniqueID COMBO_TICK_STOP = 2494296290U;
        static const AkUniqueID FEVER_ENTER = 3717465430U;
        static const AkUniqueID FEVER_EXIT = 515876060U;
        static const AkUniqueID FEVER_SUSTAIN = 2376035311U;
        static const AkUniqueID FEVER_SUSTAIN_STOP = 931870426U;
        static const AkUniqueID GEM_DROPPED = 865053637U;
        static const AkUniqueID GEM_LINK = 3840965213U;
        static const AkUniqueID GEM_LINK_FAIL = 2157605860U;
        static const AkUniqueID GEM_LINK_SUCCEED = 2568054828U;
        static const AkUniqueID GEM_TOUCHED = 1099372665U;
        static const AkUniqueID MENU_CLICK = 760777789U;
        static const AkUniqueID MENU_START = 3908297853U;
        static const AkUniqueID SCORE_TICK = 3671835971U;
        static const AkUniqueID SCORE_TICK_STOP = 4249226670U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace BGM_STATE
        {
            static const AkUniqueID GROUP = 3086301423U;

            namespace STATE
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID MENU = 2607556080U;
            } // namespace STATE
        } // namespace BGM_STATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GEMLINK_SWITCH
        {
            static const AkUniqueID GROUP = 642576873U;

            namespace SWITCH
            {
                static const AkUniqueID LINK1 = 1819160130U;
                static const AkUniqueID LINK2 = 1819160129U;
                static const AkUniqueID LINK3 = 1819160128U;
                static const AkUniqueID LINK4 = 1819160135U;
                static const AkUniqueID LINK5 = 1819160134U;
                static const AkUniqueID LINK6 = 1819160133U;
                static const AkUniqueID LINK7 = 1819160132U;
            } // namespace SWITCH
        } // namespace GEMLINK_SWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID BGM_VOL = 1622127507U;
        static const AkUniqueID HIGHCOMBOINDEX = 350534619U;
        static const AkUniqueID LINKCOUNT = 1221228806U;
        static const AkUniqueID SFX_VOL = 42448320U;
        static const AkUniqueID SS_AIR_FEAR = 1351367891U;
        static const AkUniqueID SS_AIR_FREEFALL = 3002758120U;
        static const AkUniqueID SS_AIR_FURY = 1029930033U;
        static const AkUniqueID SS_AIR_MONTH = 2648548617U;
        static const AkUniqueID SS_AIR_PRESENCE = 3847924954U;
        static const AkUniqueID SS_AIR_RPM = 822163944U;
        static const AkUniqueID SS_AIR_SIZE = 3074696722U;
        static const AkUniqueID SS_AIR_STORM = 3715662592U;
        static const AkUniqueID SS_AIR_TIMEOFDAY = 3203397129U;
        static const AkUniqueID SS_AIR_TURBULENCE = 4160247818U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID BGM_BUS = 2060214130U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MASTER_SECONDARY_BUS = 805203703U;
        static const AkUniqueID SFX_BUS = 213475909U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
