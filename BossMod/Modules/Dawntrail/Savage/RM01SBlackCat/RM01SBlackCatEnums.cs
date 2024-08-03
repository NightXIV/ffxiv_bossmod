﻿namespace BossMod.Dawntrail.Savage.RM01SBlackCat;

public enum OID : uint
{
    Boss = 0x4329, // R3.993, x1
    Helper = 0x233C, // R0.500, x50, Helper type
    //_Gen_CopyCat = 0x432A, // R3.993, x1
    LeapTarget = 0x432B, // R1.000, x0 (spawn during fight)
    Soulshade = 0x432C, // R5.610, x3, boss clone
    //_Gen_Actor1ea1a1 = 0x1EA1A1, // R0.500-2.000, x1, EventObj type
    //_Gen_Exit = 0x1E850B, // R0.500, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 39152, // Boss->player, no cast, single-target
    Teleport = 37640, // Boss->location, no cast, single-target
    BiscuitMaker = 38037, // Boss->player, 5.0s cast, single-target, tankbuster
    BiscuitMakerSecond = 38038, // Boss->player, no cast, single-target, tankbuster second hit
    BloodyScratch = 38036, // Boss->self, 5.0s cast, range 100 circle, raidwide
    NineLives = 37985, // Boss->self, 3.0s cast, single-target, visual (clones that repeat mechanics)
    Soulshade = 37986, // Boss->self, 3.0s cast, single-target, visual (create clones)
    SoulshadeActivate = 37987, // Soulshade->self, no cast, single-target, visual (clone activation)

    QuadrupleCrossingFirst = 37948, // Boss->self, 5.0s cast, single-target, visual (proteans)
    QuadrupleCrossingMid = 37949, // Boss->self, 1.0s cast, single-target, visual (intermediate set)
    QuadrupleCrossingLast = 37950, // Boss->self, 1.0s cast, single-target, visual (last set)
    QuadrupleCrossingProtean = 37951, // Helper->self, no cast, range 100 45-degree cone (protean 1/2 sets)
    QuadrupleCrossingAOE = 37952, // Helper->self, 1.6s cast, range 100 45-degree cone (cone 3/4 sets)
    LeapingQuadrupleCrossingBossR = 38959, // Boss->self, 5.0s cast, single-target, visual (jump right + proteans)
    LeapingQuadrupleCrossingBossL = 37975, // Boss->self, 5.0s cast, single-target, visual (jump left + proteans)
    LeapingQuadrupleCrossingBossFirst = 37976, // Boss->self, no cast, single-target, visual (first set)
    LeapingQuadrupleCrossingBossMid = 37977, // Boss->self, 1.0s cast, single-target, visual
    LeapingQuadrupleCrossingBossLast = 37978, // Boss->self, 1.0s cast, single-target, visual
    LeapingQuadrupleCrossingBossProtean = 37979, // Helper->self, no cast, range 100 45-degree cone
    LeapingQuadrupleCrossingBossAOE = 37980, // Helper->self, 1.6s cast, range 100 45-degree cone
    LeapingQuadrupleCrossingShadeR = 38995, // Soulshade->self, 5.0s cast, single-target, visual
    LeapingQuadrupleCrossingShadeL = 38009, // Soulshade->self, 5.0s cast, single-target, visual
    LeapingQuadrupleCrossingShadeFirst = 38010, // Soulshade->self, no cast, single-target, visual
    LeapingQuadrupleCrossingShadeMid = 38011, // Soulshade->self, 1.0s cast, single-target, visual
    LeapingQuadrupleCrossingShadeLast = 38012, // Soulshade->self, 1.0s cast, single-target, visual
    LeapingQuadrupleCrossingShadeProtean = 38013, // Helper->self, no cast, range 100 45-degree cone
    LeapingQuadrupleCrossingShadeAOE = 38014, // Helper->self, 1.6s cast, range 100 45-degree cone

    OneTwoPawBossRL = 37942, // Boss->self, 5.0s cast, single-target, visual (two cleaves)
    OneTwoPawBossAOERFirst = 37943, // Helper->self, 6.0s cast, range 100 180-degree cone
    OneTwoPawBossAOELSecond = 37944, // Helper->self, 9.0s cast, range 100 180-degree cone
    OneTwoPawBossLR = 37945, // Boss->self, 5.0s cast, single-target, visual (two cleaves)
    OneTwoPawBossAOELFirst = 37947, // Helper->self, 6.0s cast, range 100 180-degree cone
    OneTwoPawBossAOERSecond = 37946, // Helper->self, 9.0s cast, range 100 180-degree cone
    OneTwoPawShadeRL = 37988, // Soulshade->self, 5.0s cast, single-target, visual (two cleaves)
    OneTwoPawShadeAOERFirst = 37989, // Helper->self, 6.0s cast, range 100 180-degree cone
    OneTwoPawShadeAOELSecond = 37990, // Helper->self, 9.0s cast, range 100 180-degree cone
    OneTwoPawShadeLR = 37991, // Soulshade->self, 5.0s cast, single-target, visual (two cleaves)
    OneTwoPawShadeAOELFirst = 37993, // Helper->self, 6.0s cast, range 100 180-degree cone
    OneTwoPawShadeAOERSecond = 37992, // Helper->self, 9.0s cast, range 100 180-degree cone

    QuadrupleSwipeBoss = 37981, // Boss->self, 4.0+1.0s cast, single-target, visual (pair stacks)
    QuadrupleSwipeBossAOE = 37982, // Helper->players, 5.0s cast, range 4 circle 2-man stack
    DoubleSwipeBoss = 37983, // Boss->self, 4.0+1.0s cast, single-target, visual (light party stacks)
    DoubleSwipeBossAOE = 37984, // Helper->players, 5.0s cast, range 5 circle 4-man stack
    QuadrupleSwipeShade = 38015, // Soulshade->self, 4.0+1.0s cast, single-target, visual (pair stacks)
    QuadrupleSwipeShadeAOE = 38016, // Helper->players, 5.0s cast, range 4 circle 2-man stack
    DoubleSwipeShade = 38017, // Soulshade->self, 4.0+1.0s cast, single-target, visual (light party stacks)
    DoubleSwipeShadeAOE = 38018, // Helper->players, 5.0s cast, range 5 circle 4-man stack

    //_Weaponskill_ = 37955, // Helper->self, 1.0s cast, range 10 width 10 rect
    //_Weaponskill_ = 39276, // Helper->self, 1.0s cast, range 10 width 10 rect
    //_Weaponskill_Mouser = 37953, // Boss->self, 10.0s cast, single-target
    //_Weaponskill_Mouser = 37956, // Helper->location, no cast, single-target
    //_Weaponskill_Mouser = 38054, // Helper->self, no cast, range 10 width 10 rect
    //_Weaponskill_ = 37954, // Boss->self, no cast, single-target
    //_Spell_Copycat = 37957, // Boss->self, 3.0s cast, single-target
    //_Weaponskill_ElevateAndEviscerate = 37958, // 432A->self, 5.6s cast, single-target
    //_Weaponskill_ElevateAndEviscerate = 37959, // 432A->player, no cast, single-target
    //_Weaponskill_Shockwave = 37962, // Helper->self, no cast, range 60 width 10 cross
    //_Weaponskill_Impact = 39251, // Helper->self, no cast, range 10 width 10 rect
    //_Weaponskill_ElevateAndEviscerate = 37960, // 432A->self, 5.6s cast, single-target
    //_Weaponskill_ElevateAndEviscerate = 37961, // 432A->player, no cast, single-target
    //_Weaponskill_Impact = 39252, // Helper->self, no cast, range 10 width 10 rect
    //_Weaponskill_GrimalkinGale = 39811, // Boss->self, no cast, single-target
    //_Spell_Shockwave = 37963, // Boss->self, 6.0+1.0s cast, single-target
    //_Spell_Shockwave = 37964, // Helper->self, 7.0s cast, range 30 circle
    //_Spell_GrimalkinGale = 39812, // Helper->players, 5.0s cast, range 5 circle
    //_Weaponskill_LeapingOneTwoPaw = 37965, // Boss->self, 5.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 37969, // Boss->self, no cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 37970, // Helper->self, 0.8s cast, range 100 ?-degree cone
    //_Weaponskill_LeapingOneTwoPaw = 37971, // Helper->self, 2.8s cast, range 100 ?-degree cone
    //_Ability_ = 34722, // Helper->player, no cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 37999, // Soulshade->self, 5.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38003, // Soulshade->self, no cast, single-target
    //_Spell_TempestuousTear = 38019, // Boss->self, 5.0+1.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38004, // Helper->self, 0.8s cast, range 100 ?-degree cone
    //_Spell_TempestuousTear = 38020, // Helper->players, no cast, range 100 width 6 rect
    //_Weaponskill_LeapingOneTwoPaw = 38005, // Helper->self, 2.8s cast, range 100 ?-degree cone
    //_Weaponskill_LeapingOneTwoPaw = 37966, // Boss->self, 5.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 37972, // Boss->self, no cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 37974, // Helper->self, 0.8s cast, range 100 ?-degree cone
    //_Weaponskill_LeapingOneTwoPaw = 37973, // Helper->self, 2.8s cast, range 100 ?-degree cone
    //_Spell_Nailchipper = 38021, // Boss->self, 7.0+1.0s cast, single-target
    //_Spell_Nailchipper = 38022, // Helper->players, 8.0s cast, range 5 circle
    //_Weaponskill_LeapingOneTwoPaw = 37968, // Boss->self, 5.0s cast, single-target
    //_Ability_ = 26708, // Helper->player, no cast, single-target
    //_Spell_Overshadow = 38039, // Boss->player, 5.0s cast, single-target
    //_Spell_Overshadow = 38040, // Boss->players, no cast, range 100 width 5 rect
    //_Weaponskill_SplinteringNails = 38041, // Boss->self, 5.0s cast, single-target
    //_Weaponskill_SplinteringNails = 38042, // Helper->self, no cast, range 100 ?-degree cone
    //_Weaponskill_RainingCats = 39611, // Boss->self, 6.0s cast, single-target
    //_Spell_RainingCats = 38047, // Helper->players, no cast, range 4 circle
    //_Weaponskill_RainingCats = 38045, // Helper->self, no cast, range 100 ?-degree cone
    //_Weaponskill_RainingCats = 39612, // Boss->self, 5.0s cast, single-target
    //_Weaponskill_RainingCats = 39613, // Boss->self, 5.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38000, // Soulshade->self, 5.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38006, // Soulshade->self, no cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38008, // Helper->self, 0.8s cast, range 100 ?-degree cone
    //_Weaponskill_LeapingOneTwoPaw = 38007, // Helper->self, 2.8s cast, range 100 ?-degree cone
    //_Weaponskill_ = 38026, // Helper->location, 2.0s cast, width 6 rect charge
    //_Weaponskill_ = 38027, // Helper->self, 3.0s cast, range 11 circle
    //_Weaponskill_ = 38028, // Helper->location, 4.0s cast, width 6 rect charge
    //_Weaponskill_ = 38035, // Helper->self, 11.0s cast, range 11 circle
    //_Weaponskill_ = 39633, // Helper->self, 13.0s cast, range 11 circle
    //_Weaponskill_ = 38031, // Helper->self, 7.0s cast, range 11 circle
    //_Weaponskill_ = 38032, // Helper->location, 8.0s cast, width 6 rect charge
    //_Weaponskill_ = 38030, // Helper->location, 6.0s cast, width 6 rect charge
    //_Weaponskill_ = 38034, // Helper->location, 10.0s cast, width 6 rect charge
    //_Weaponskill_PredaceousPounce = 39709, // Helper->self, 14.0s cast, range 11 circle
    //_Weaponskill_PredaceousPounce = 39704, // Helper->location, 13.5s cast, width 6 rect charge
    //_Weaponskill_ = 38029, // Helper->self, 5.0s cast, range 11 circle
    //_Weaponskill_ = 39632, // Helper->location, 12.0s cast, width 6 rect charge
    //_Weaponskill_ = 38033, // Helper->self, 9.0s cast, range 11 circle
    //_Weaponskill_PredaceousPounce = 39635, // 432A->location, 13.0s cast, single-target
    //_Weaponskill_LeapingOneTwoPaw = 38002, // Soulshade->self, 5.0s cast, single-target
    //_Weaponskill_PredaceousPounce = 38024, // 432A->location, no cast, single-target
    //_Weaponskill_PredaceousPounce = 39270, // Helper->location, 1.0s cast, width 6 rect charge
    //_Weaponskill_PredaceousPounce = 38025, // Helper->self, 1.5s cast, range 11 circle
    //_Weaponskill_Mouser = 39822, // Boss->self, 8.0s cast, single-target
}

public enum SID : uint
{
    //_Gen_SlashingResistanceDown = 3130, // Helper/Boss->player, extra=0x0
    //_Gen_DamageDown = 3304, // Helper->player, extra=0x1/0x2
    //_Gen_Weakness = 43, // none->player, extra=0x0
    //_Gen_Transcendent = 418, // none->player, extra=0x0
    //_Gen_NineLives = 3931, // none->Boss, extra=0x0
    //_Gen_OneTwoMemory = 4048, // none->Boss, extra=0x0
    //_Gen_ = 2056, // none->Soulshade/Boss, extra=0xE1/0x307
    //_Gen_ = 2193, // Soulshade->Soulshade, extra=0x316/0x317
    //_Gen_SustainedDamage = 2935, // Helper->player, extra=0x0
    //_Gen_PhysicalVulnerabilityUp = 2940, // Helper->player, extra=0x0
    //_Gen_TwoSwipeMemory = 4053, // none->Boss, extra=0x0
    //_Gen_FourSwipeMemory = 4052, // none->Boss, extra=0x0
    //_Gen_Stun = 2656, // none->player, extra=0x0
    //_Gen_DownForTheCount = 783, // 432A->player, extra=0xEC7
    //_Gen_BrinkOfDeath = 44, // none->player, extra=0x0
    //_Gen_MagicVulnerabilityUp = 2941, // Helper->player, extra=0x0
    //_Gen_LeftwardMemory = 4051, // none->Boss, extra=0x0
    //_Gen_FourCrossedMemory = 4049, // none->Boss, extra=0x0
    //_Gen_RightwardMemory = 4050, // none->Boss, extra=0x0
}

public enum IconID : uint
{
    BiscuitMaker = 218, // player
    _Gen_Icon_538 = 538, // player
    _Gen_Icon_376 = 376, // player
    _Gen_Icon_244 = 244, // player
}

public enum TetherID : uint
{
    Soulshade = 102, // Soulshade->Boss
    Leap = 12, // Boss->LeapTarget
    _Gen_Tether_89 = 89, // player->Boss
}