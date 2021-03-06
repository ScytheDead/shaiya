using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Dyeing;
using Imgeneus.World.Game.Linking;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Imgeneus.World.Game.Notice;
using Imgeneus.World.Game.Guild;
using Imgeneus.Database;
using Imgeneus.World.Game.Time;
using System.Collections.Concurrent;

namespace Imgeneus.World.Tests
{
    public abstract class BaseTest
    {
        protected Mock<ILogger<Character>> loggerMock = new Mock<ILogger<Character>>();
        protected Mock<IGameWorld> gameWorldMock = new Mock<IGameWorld>();
        protected Mock<IBackgroundTaskQueue> taskQueuMock = new Mock<IBackgroundTaskQueue>();
        protected Mock<IDatabasePreloader> databasePreloader = new Mock<IDatabasePreloader>();
        protected Mock<IDatabase> databaseMock = new Mock<IDatabase>();
        protected Mock<ITimeService> timeMock = new Mock<ITimeService>();
        protected Mock<IMapsLoader> mapsLoaderMock = new Mock<IMapsLoader>();
        protected Mock<ICharacterConfiguration> config = new Mock<ICharacterConfiguration>();
        protected Mock<ILogger<Map>> mapLoggerMock = new Mock<ILogger<Map>>();
        protected Mock<ILogger<Mob>> mobLoggerMock = new Mock<ILogger<Mob>>();
        protected Mock<ILogger<Npc>> npcLoggerMock = new Mock<ILogger<Npc>>();
        protected Mock<ILogger<IGuildManager>> guildLoggerMock = new Mock<ILogger<IGuildManager>>();
        protected Mock<IChatManager> chatMock = new Mock<IChatManager>();
        protected Mock<ILinkingManager> linkingMock = new Mock<ILinkingManager>();
        protected Mock<IDyeingManager> dyeingMock = new Mock<IDyeingManager>();
        protected Mock<IWorldClient> worldClientMock = new Mock<IWorldClient>();
        protected Mock<IMobFactory> mobFactoryMock = new Mock<IMobFactory>();
        protected Mock<INpcFactory> npcFactoryMock = new Mock<INpcFactory>();
        protected Mock<IObeliskFactory> obeliskFactoryMock = new Mock<IObeliskFactory>();
        protected Mock<INoticeManager> noticeManagerMock = new Mock<INoticeManager>();
        protected Mock<IGuildManager> guidManagerMock = new Mock<IGuildManager>();

        protected Map testMap => new Map(
                    Map.TEST_MAP_ID,
                    new MapDefinition(),
                    new MapConfiguration() { Size = 100, CellSize = 100 },
                    mapLoggerMock.Object,
                    databasePreloader.Object,
                    mobFactoryMock.Object,
                    npcFactoryMock.Object,
                    obeliskFactoryMock.Object,
                    timeMock.Object);

        private static int CharacterId;
        protected Character CreateCharacter(Map map = null)
        {
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, mapsLoaderMock.Object, chatMock.Object, linkingMock.Object, dyeingMock.Object, mobFactoryMock.Object, npcFactoryMock.Object, noticeManagerMock.Object, guidManagerMock.Object);

            character.Client = worldClientMock.Object;
            character.Id = CharacterId++;

            if (map != null)
                map.LoadPlayer(character);

            return character;
        }

        public BaseTest()
        {
            gameWorldMock.Setup(x => x.Players)
                .Returns(new ConcurrentDictionary<int, Character>());

            config.Setup((conf) => conf.GetConfig(It.IsAny<int>()))
                  .Returns(new Character_HP_SP_MP() { HP = 100, MP = 200, SP = 300 });

            config.Setup((conf) => conf.DefaultStats)
                  .Returns(new DefaultStat[1] {
                      new DefaultStat()
                      {
                          Job = CharacterProfession.Fighter,
                          Str = 12,
                          Dex = 11,
                          Rec = 10,
                          Int = 8,
                          Wis = 9,
                          Luc = 10,
                          MainStat = 0
                      }
                  });

            config.Setup((conf) => conf.GetMaxLevelConfig(It.IsAny<Mode>()))
                .Returns(
                    new DefaultMaxLevel()
                    {
                        Mode = Mode.Ultimate,
                        Level = 80
                    }
                );

            config.Setup((conf) => conf.GetLevelStatSkillPoints(It.IsAny<Mode>()))
                .Returns(
                    new DefaultLevelStatSkillPoints()
                    {
                        Mode = Mode.Ultimate,
                        StatPoint = 9,
                        SkillPoint = 7
                    }
                );

            databasePreloader
                .SetupGet((preloader) => preloader.Mobs)
                .Returns(new Dictionary<ushort, DbMob>()
                {
                    { 1, Wolf },
                    { 3041, CrypticImmortal }
                });

            databasePreloader
                .SetupGet((preloader) => preloader.Skills)
                .Returns(new Dictionary<(ushort SkillId, byte SkillLevel), DbSkill>()
                {
                    { (1, 1) , StrengthTraining },
                    { (14, 1), ManaTraining },
                    { (15, 1), SharpenWeaponMastery_Lvl1 },
                    { (15, 2), SharpenWeaponMastery_Lvl2 },
                    { (35, 1), MagicRoots_Lvl1 },
                    { (273, 100), AttributeRemove },
                    { (732, 1), FireWeapon },
                    { (735, 1), EarthWeapon },
                    { (762, 1), FireSkin },
                    { (765, 1), EarthSkin },
                    { (672, 1), Panic_Lvl1 },
                    { (787, 1), Dispel },
                    { (256, 1), Skill_HealthRemedy_Level1 },
                    { (112, 1), Leadership },
                    { (222, 1), EXP },
                    { (0, 1) , skill1_level1 },
                    { (0, 2) , skill1_level2 },
                    { (418, 11), BlastAbsorbRedemySkill },
                    { (655, 1), Untouchable },
                    { (724, 1), BullsEye }
                });
            databasePreloader
                .SetupGet((preloader) => preloader.Items)
                .Returns(new Dictionary<(byte Type, byte TypeId), DbItem>()
                {
                    { (17, 2), WaterArmor },
                    { (2, 92), FireSword },
                    { (100, 192), PerfectLinkingHammer },
                    { (44, 237), PerfectExtractingHammer },
                    { (100, 139), LuckyCharm },
                    { (17, 59), JustiaArmor },
                    { (30, 1), Gem_Str_Level_1 },
                    { (30, 2), Gem_Str_Level_2 },
                    { (30, 3), Gem_Str_Level_3 },
                    { (30, 7), Gem_Str_Level_7 },
                    { (100, 1), EtainPotion },
                    { (25, 1), RedApple },
                    { (42, 1), HorseSummonStone },
                    { (42, 136), Nimbus1d },
                    { (100, 95), Item_HealthRemedy_Level_1  },
                    { (101, 71), Item_AbsorbRemedy },
                    { (30, 240), Gem_Absorption_Level_4 },
                    { (30, 241), Gem_Absorption_Level_5 },
                    { (43, 3), Etin_100 }
                });

            databasePreloader
                .SetupGet((preloader) => preloader.MobItems)
                .Returns(new Dictionary<(ushort MobId, byte ItemOrder), DbMobItems>());

            databasePreloader
                .SetupGet((preloader) => preloader.Levels)
                .Returns(new Dictionary<(Mode mode, ushort level), DbLevel>()
                {
                    { (Mode.Beginner, 1), Level1_Mode1 },
                    { (Mode.Normal, 1), Level1_Mode2 },
                    { (Mode.Hard, 1), Level1_Mode3 },
                    { (Mode.Ultimate, 1), Level1_Mode4 },
                    { (Mode.Beginner, 2), Level2_Mode1 },
                    { (Mode.Normal, 2), Level2_Mode2 },
                    { (Mode.Hard, 2), Level2_Mode3 },
                    { (Mode.Ultimate, 2), Level2_Mode4 },
                    { (Mode.Beginner, 37), Level37_Mode1 },
                    { (Mode.Normal, 37), Level37_Mode2 },
                    { (Mode.Hard, 37), Level37_Mode3 },
                    { (Mode.Ultimate, 37), Level37_Mode4 },
                    { (Mode.Beginner, 38), Level38_Mode1 },
                    { (Mode.Normal, 38), Level38_Mode2 },
                    { (Mode.Hard, 38), Level38_Mode3 },
                    { (Mode.Ultimate, 38), Level38_Mode4 },
                    { (Mode.Beginner, 79), Level79_Mode1 },
                    { (Mode.Normal, 79), Level79_Mode2 },
                    { (Mode.Hard, 79), Level79_Mode3 },
                    { (Mode.Ultimate, 79), Level79_Mode4 },
                    { (Mode.Beginner, 80), Level80_Mode1 },
                    { (Mode.Normal, 80), Level80_Mode2 },
                    { (Mode.Hard, 80), Level80_Mode3 },
                    { (Mode.Ultimate, 80), Level80_Mode4 },
                });


            databasePreloader
                .SetupGet((preloader) => preloader.NPCs)
                .Returns(new Dictionary<(byte Type, ushort TypeId), DbNpc>()
                {
                    { (1, 1), WeaponMerchant }
                });
        }

        #region Test mobs

        protected DbMob Wolf = new DbMob()
        {
            Id = 1,
            MobName = "Small Ruined Wolf",
            AI = MobAI.Combative,
            Level = 38,
            HP = 2765,
            Element = Element.Wind1,
            AttackSpecial3 = MobRespawnTime.TestEnv,
            NormalTime = 1,
            Exp = 70
        };

        protected DbMob CrypticImmortal = new DbMob()
        {
            Id = 3041,
            MobName = "Cryptic the Immortal",
            AI = MobAI.CrypticImmortal,
            Level = 75,
            HP = 35350000,
            AttackOk1 = 1,
            Attack1 = 8822,
            AttackPlus1 = 3222,
            AttackRange1 = 5,
            AttackTime1 = 2500,
            NormalTime = 1,
            ChaseTime = 1,
            Exp = 3253
        };

        #endregion

        #region Skills

        protected DbSkill StrengthTraining = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 1,
            TypeDetail = TypeDetail.PassiveDefence,
            SkillName = "Strength Training Lv1",
            TypeAttack = TypeAttack.Passive,
            AbilityType1 = AbilityType.PhysicalAttackPower,
            AbilityValue1 = 18,
            SkillPoint = 1
        };

        protected DbSkill ManaTraining = new DbSkill()
        {
            SkillId = 14,
            SkillLevel = 1,
            TypeDetail = TypeDetail.PassiveDefence,
            SkillName = "Mana Training",
            TypeAttack = TypeAttack.Passive,
            AbilityType1 = AbilityType.MP,
            AbilityValue1 = 110
        };

        protected DbSkill SharpenWeaponMastery_Lvl1 = new DbSkill()
        {
            SkillId = 15,
            SkillLevel = 1,
            TypeDetail = TypeDetail.WeaponMastery,
            SkillName = "Sharpen Weapon Mastery Lvl 1",
            TypeAttack = TypeAttack.Passive,
            Weapon1 = 1,
            Weapon2 = 3,
            Weaponvalue = 1
        };

        protected DbSkill SharpenWeaponMastery_Lvl2 = new DbSkill()
        {
            SkillId = 15,
            SkillLevel = 2,
            TypeDetail = TypeDetail.WeaponMastery,
            SkillName = "Sharpen Weapon Mastery Lvl 2",
            TypeAttack = TypeAttack.Passive,
            Weapon1 = 1,
            Weapon2 = 3,
            Weaponvalue = 2
        };

        protected DbSkill MagicRoots_Lvl1 = new DbSkill()
        {
            SkillId = 35,
            SkillLevel = 1,
            TypeDetail = TypeDetail.Immobilize,
            SkillName = "Magic Roots",
            DamageHP = 42,
            TypeAttack = TypeAttack.MagicAttack,
            ResetTime = 10,
            KeepTime = 5,
            DamageType = DamageType.PlusExtraDamage,
        };

        protected DbSkill AttributeRemove = new DbSkill()
        {
            SkillId = 273,
            SkillLevel = 100,
            TypeDetail = TypeDetail.RemoveAttribute,
            SkillName = "Attribute Remove",
            TypeAttack = TypeAttack.MagicAttack,
            DamageType = DamageType.FixedDamage
        };

        protected DbSkill FireWeapon = new DbSkill()
        {
            SkillId = 732,
            SkillLevel = 1,
            SkillName = "Flame Weapon",
            TypeDetail = TypeDetail.ElementalAttack,
            Element = Element.Fire1,
            TypeAttack = TypeAttack.ShootingAttack
        };

        protected DbSkill EarthWeapon = new DbSkill()
        {
            SkillId = 735,
            SkillLevel = 1,
            SkillName = "Earth Weapon",
            TypeDetail = TypeDetail.ElementalAttack,
            Element = Element.Earth1,
            TypeAttack = TypeAttack.ShootingAttack
        };

        protected DbSkill FireSkin = new DbSkill()
        {
            SkillId = 762,
            SkillLevel = 1,
            SkillName = "Flame Skin",
            TypeDetail = TypeDetail.ElementalProtection,
            Element = Element.Fire1,
            TypeAttack = TypeAttack.MagicAttack
        };

        protected DbSkill EarthSkin = new DbSkill()
        {
            SkillId = 765,
            SkillLevel = 1,
            SkillName = "Earth Skin",
            TypeDetail = TypeDetail.ElementalProtection,
            Element = Element.Earth1,
            TypeAttack = TypeAttack.MagicAttack
        };

        protected DbSkill Panic_Lvl1 = new DbSkill()
        {
            SkillId = 672,
            SkillLevel = 1,
            SkillName = "Panic",
            TypeDetail = TypeDetail.SubtractingDebuff,
            AbilityType1 = AbilityType.PhysicalDefense,
            AbilityValue1 = 119,
            TypeAttack = TypeAttack.MagicAttack,
        };

        protected DbSkill Dispel = new DbSkill()
        {
            SkillId = 787,
            SkillLevel = 1,
            SkillName = "Dispel",
            TypeDetail = TypeDetail.Dispel,
            TypeAttack = TypeAttack.MagicAttack,
        };

        protected DbSkill Skill_HealthRemedy_Level1 = new DbSkill()
        {
            SkillId = 256,
            SkillLevel = 1,
            SkillName = "Health Remedy Lv1",
            TypeDetail = TypeDetail.Buff,
            TargetType = TargetType.Caster,
            AbilityType1 = AbilityType.HP,
            AbilityValue1 = 500,
            FixRange = ClearAfterDeath.KeepInMins
        };

        protected DbSkill Leadership = new DbSkill()
        {
            SkillId = 112,
            SkillLevel = 1,
            SkillName = "Leadership Lv1",
            TypeDetail = TypeDetail.Buff,
            TargetType = TargetType.AlliesNearCaster,
            SuccessType = SuccessType.SuccessBasedOnValue,
            SuccessValue = 100,
            ApplyRange = 50,
            AbilityType1 = AbilityType.PhysicalAttackPower,
            AbilityValue1 = 13,
            FixRange = ClearAfterDeath.Clear
        };

        protected DbSkill EXP = new DbSkill()
        {
            SkillId = 222,
            SkillLevel = 1,
            SkillName = "Increase EXP",
            TypeDetail = TypeDetail.Buff,
            TargetType = TargetType.Caster,
            SuccessType = SuccessType.SuccessBasedOnValue,
            SuccessValue = 100,
            ApplyRange = 50,
            AbilityType1 = AbilityType.ExpGainRate,
            AbilityValue1 = 150,
            FixRange = ClearAfterDeath.KeepInHours
        };

        protected DbSkill skill1_level1 = new DbSkill()
        {
            SkillId = 0,
            SkillLevel = 1,
            TypeDetail = TypeDetail.Buff,
            KeepTime = 3000 // 3 sec
        };

        protected DbSkill skill1_level2 = new DbSkill()
        {
            SkillId = 0,
            SkillLevel = 2,
            TypeDetail = TypeDetail.Buff,
            KeepTime = 5000 // 5 sec
        };

        protected DbSkill BlastAbsorbRedemySkill = new DbSkill()
        {
            SkillId = 418,
            SkillLevel = 11,
            TypeDetail = TypeDetail.Buff,
            SuccessType = SuccessType.SuccessBasedOnValue,
            SuccessValue = 100,
            TargetType = TargetType.Caster,
            AbilityType1 = AbilityType.AbsorptionAura,
            AbilityValue1 = 20
        };

        protected DbSkill Untouchable = new DbSkill()
        {
            SkillId = 655,
            SkillLevel = 1,
            SkillName = "Untouchable Lv1",
            TypeDetail = TypeDetail.Untouchable,
            SuccessType = SuccessType.SuccessBasedOnValue,
            SuccessValue = 100,
            TargetType = TargetType.Caster
        };

        protected DbSkill BullsEye = new DbSkill()
        {
            SkillId = 724,
            SkillLevel = 1,
            SkillName = "Bull's Eye",
            SuccessType = SuccessType.SuccessBasedOnValue,
            SuccessValue = 100,
            TargetType = TargetType.SelectedEnemy
        };

        #endregion

        #region Items

        protected DbItem WaterArmor = new DbItem()
        {
            Type = 17,
            TypeId = 2,
            ItemName = "Water armor",
            Element = Element.Water1,
            Count = 1,
            Quality = 1200
        };

        protected DbItem FireSword = new DbItem()
        {
            Type = 2,
            TypeId = 92,
            ItemName = "Thane Breaker of Fire",
            Element = Element.Fire1,
            Count = 1,
            Quality = 1200
        };

        protected DbItem PerfectLinkingHammer = new DbItem()
        {
            Type = 100,
            TypeId = 192,
            ItemName = "Perfect Linking Hammer",
            Special = SpecialEffect.PerfectLinkingHammer,
            Count = 255,
            Quality = 0
        };

        protected DbItem PerfectExtractingHammer = new DbItem()
        {
            Type = 44,
            TypeId = 237,
            ItemName = "GM Extraction Hammer",
            Special = SpecialEffect.PerfectExtractionHammer,
            Count = 10,
            Quality = 0
        };

        protected DbItem LuckyCharm = new DbItem()
        {
            Type = 100,
            TypeId = 139,
            ItemName = "Lucky Charm",
            Special = SpecialEffect.LuckyCharm,
            Count = 255,
            Quality = 0
        };

        protected DbItem JustiaArmor = new DbItem()
        {
            Type = 17,
            TypeId = 59,
            ItemName = "Justia Armor",
            ConstStr = 30,
            ConstDex = 30,
            ConstRec = 30,
            ConstHP = 1800,
            ConstSP = 600,
            Slot = 6,
            Quality = 1200,
            Attackfighter = 1,
            Defensefighter = 1,
            ReqWis = 20,
            Count = 1
        };

        protected DbItem Gem_Str_Level_1 = new DbItem()
        {
            Type = 30,
            TypeId = 1,
            ConstStr = 3,
            ReqIg = 0, // always fail linking or extracting, unless hammer is used
            Count = 255,
            Quality = 0
        };

        protected DbItem Gem_Str_Level_2 = new DbItem()
        {
            Type = 30,
            TypeId = 2,
            ConstStr = 5,
            ReqIg = 255, // always success linking or extracting.
            Count = 255,
            Quality = 0
        };

        protected DbItem Gem_Str_Level_3 = new DbItem()
        {
            Type = 30,
            TypeId = 3,
            ConstStr = 7,
            ReqIg = 255, // always success linking or extracting.
            Count = 255,
            Quality = 0
        };

        protected DbItem Gem_Str_Level_7 = new DbItem()
        {
            Type = 30,
            TypeId = 7,
            ConstStr = 50,
            ReqVg = 1, // Will break item if linking/extracting fails
            ReqIg = 0, // always fail linking or extracting, unless hammer is used
            Count = 255,
            Quality = 0
        };

        protected DbItem Gem_Absorption_Level_4 = new DbItem()
        {
            Type = 30,
            TypeId = 240,
            Exp = 20
        };

        protected DbItem Gem_Absorption_Level_5 = new DbItem()
        {
            Type = 30,
            TypeId = 241,
            Exp = 50
        };

        protected DbItem EtainPotion = new DbItem()
        {
            Type = 100,
            TypeId = 1,
            ConstHP = 75,
            ConstMP = 75,
            ConstSP = 75,
            Special = SpecialEffect.PercentHealingPotion
        };

        protected DbItem RedApple = new DbItem()
        {
            Type = 25,
            TypeId = 1,
            Special = SpecialEffect.None,
            ConstHP = 50
        };

        protected DbItem HorseSummonStone = new DbItem()
        {
            Type = 42,
            TypeId = 1
        };

        protected DbItem Nimbus1d = new DbItem()
        {
            Type = 42,
            TypeId = 136,
            Duration = 86400
        };

        protected DbItem Item_HealthRemedy_Level_1 = new DbItem()
        {
            Type = 100,
            TypeId = 95,
            Range = 256,
            AttackTime = 1
        };

        protected DbItem Item_AbsorbRemedy = new DbItem()
        {
            Type = 101,
            TypeId = 71,
            Range = 418,
            AttackTime = 11
        };

        protected DbItem Etin_100 = new DbItem()
        {
            Type = 43,
            TypeId = 3,
            Special = SpecialEffect.Etin_100
        };

        #endregion

        #region Levels

        protected DbLevel Level1_Mode1 = new DbLevel()
        {
            Level = 1,
            Mode = Mode.Beginner,
            Exp = 70
        };

        protected DbLevel Level1_Mode2 = new DbLevel()
        {
            Level = 1,
            Mode = Mode.Normal,
            Exp = 200
        };

        protected DbLevel Level1_Mode3 = new DbLevel()
        {
            Level = 1,
            Mode = Mode.Hard,
            Exp = 200
        };

        protected DbLevel Level1_Mode4 = new DbLevel()
        {
            Level = 1,
            Mode = Mode.Ultimate,
            Exp = 200
        };

        protected DbLevel Level2_Mode1 = new DbLevel()
        {
            Level = 2,
            Mode = Mode.Beginner,
            Exp = 130
        };

        protected DbLevel Level2_Mode2 = new DbLevel()
        {
            Level = 2,
            Mode = Mode.Normal,
            Exp = 400
        };

        protected DbLevel Level2_Mode3 = new DbLevel()
        {
            Level = 2,
            Mode = Mode.Hard,
            Exp = 400
        };

        protected DbLevel Level2_Mode4 = new DbLevel()
        {
            Level = 2,
            Mode = Mode.Ultimate,
            Exp = 400
        };

        protected DbLevel Level37_Mode1 = new DbLevel()
        {
            Level = 37,
            Mode = Mode.Beginner,
            Exp = 171200
        };

        protected DbLevel Level37_Mode2 = new DbLevel()
        {
            Level = 37,
            Mode = Mode.Normal,
            Exp = 2418240
        };

        protected DbLevel Level37_Mode3 = new DbLevel()
        {
            Level = 37,
            Mode = Mode.Hard,
            Exp = 2418240
        };

        protected DbLevel Level37_Mode4 = new DbLevel()
        {
            Level = 37,
            Mode = Mode.Ultimate,
            Exp = 3022800
        };

        protected DbLevel Level38_Mode1 = new DbLevel()
        {
            Level = 38,
            Mode = Mode.Beginner,
            Exp = 171200
        };

        protected DbLevel Level38_Mode2 = new DbLevel()
        {
            Level = 38,
            Mode = Mode.Normal,
            Exp = 2714880
        };

        protected DbLevel Level38_Mode3 = new DbLevel()
        {
            Level = 38,
            Mode = Mode.Hard,
            Exp = 2714880
        };

        protected DbLevel Level38_Mode4 = new DbLevel()
        {
            Level = 38,
            Mode = Mode.Ultimate,
            Exp = 3396800
        };

        protected DbLevel Level79_Mode1 = new DbLevel()
        {
            Level = 79,
            Mode = Mode.Beginner,
            Exp = 171200
        };

        protected DbLevel Level79_Mode2 = new DbLevel()
        {
            Level = 79,
            Mode = Mode.Normal,
            Exp = 214847083
        };

        protected DbLevel Level79_Mode3 = new DbLevel()
        {
            Level = 69,
            Mode = Mode.Hard,
            Exp = 214847083
        };

        protected DbLevel Level79_Mode4 = new DbLevel()
        {
            Level = 79,
            Mode = Mode.Ultimate,
            Exp = 330854048
        };

        protected DbLevel Level80_Mode1 = new DbLevel()
        {
            Level = 50,
            Mode = Mode.Beginner,
            Exp = 171200
        };

        protected DbLevel Level80_Mode2 = new DbLevel()
        {
            Level = 60,
            Mode = Mode.Normal,
            Exp = 214847083
        };

        protected DbLevel Level80_Mode3 = new DbLevel()
        {
            Level = 70,
            Mode = Mode.Hard,
            Exp = 214847083
        };

        protected DbLevel Level80_Mode4 = new DbLevel()
        {
            Level = 80,
            Mode = Mode.Ultimate,
            Exp = 330854048
        };

        #endregion

        #region NPC

        protected DbNpc WeaponMerchant = new DbNpc()
        {
            Type = 1,
            TypeId = 1,
            Name = "Erina Probicio",
            MerchantType = MerchantType.WeaponSeller
        };

        #endregion
    }
}
