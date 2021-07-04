﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace AdditionalQuestsCode.Quests
{
    public class NobleWantsTrainingBattleIssueBehavior : CampaignBehaviorBase
    {
        // Needs to be a noble commander with at least 20% of their army being tier one/tier two units
        private bool ConditionsHold(Hero issueGiver)
        {
            if (issueGiver.IsNoble && issueGiver.IsPartyLeader)
            {
                double lowTierTroops = 0;
                double totalTroops = 0;

                foreach (var troop in issueGiver.PartyBelongedTo.MemberRoster.GetTroopRoster())
                {
                    if(troop.Character.Tier == 1 || troop.Character.Tier == 2)
                    {
                        lowTierTroops += troop.Number;
                    }
                    totalTroops += troop.Number;
                }

                if (lowTierTroops / totalTroops >= 0.2)
                {
                    return true;
                }
            }

            return false;
        }

        // If the conditions hold, start this quest, otherwise just add it as a possible quest
        public void OnCheckForIssue(Hero hero)
        {
            if (this.ConditionsHold(hero))
            {
                Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(VillageBanditArmyRaidIssueBehavior.VillageBanditArmyRaidIssue), IssueBase.IssueFrequency.Common));
                return;
            }
            Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(VillageBanditArmyRaidIssueBehavior.VillageBanditArmyRaidIssue), IssueBase.IssueFrequency.Common));
        }

        private IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
        {
            return new NobleWantsTrainingBattleIssueBehavior.NobleWantsTrainingBattleIssue(issueOwner);
        }

        // Now the Issue
        internal class NobleWantsTrainingBattleIssue : IssueBase
        {
            public NobleWantsTrainingBattleIssue(Hero issueOwner) : base(issueOwner, CampaignTime.DaysFromNow(20f))
            {
            }

            public override TextObject Title
            {
                get
                {
                    TextObject textObject = new TextObject("{NOBLE_NAME} Wants a Training Battle", null);
                    textObject.SetTextVariable("NOBLE_NAME", base.IssueOwner.Name);
                    return textObject;
                }
            }

            public override TextObject Description
            {
                get
                {
                    TextObject textObject = new TextObject("{NOBLE_NAME} wants to have {?QUEST_GIVER.GENDER}her{?}his{\\?} army trained in combat, via a miltary exercise.", null);
                    textObject.SetTextVariable("NOBLE_NAME", base.IssueSettlement.Name);
                    return textObject;
                }
            }

            public override TextObject IssueBriefByIssueGiver
            {
                get
                {
                    return new TextObject("I have far to many fresh faced recruits in my army, not ready for combat against anything more trained then looters. I believe a training exercise with some veteran soliders will be a good experience for them..", null);
                }
            }

            public override TextObject IssueAcceptByPlayer
            {
                get
                {
                    return new TextObject("I have some experienced troops, maybe I can help?", null);
                }
            }

            public override TextObject IssueQuestSolutionExplanationByIssueGiver
            {
                get
                {
                    TextObject textObject = new TextObject("That would be great, If you can wait a couple of days, I can prepare a training battle. Bring 50 of your men against 50 of mine. I will also provide suitably blunted weapons and medics. If you beat my troops, I'll even throw in a extra reward.", null);
                    return textObject;
                }
            }

            public override TextObject IssueQuestSolutionAcceptByPlayer
            {
                get
                {
                    return new TextObject("Sure, I will prepare my men and wait for your word.", null);
                }
            }

            public override TextObject IssueAsRumorInSettlement
            {
                get
                {
                    TextObject textObject = new TextObject("I heard {QUEST_GIVER.NAME} shouting at {?QUEST_GIVER.GENDER}her{?}his{\\?} troops today, {?QUEST_GIVER.GENDER}she{?}he{\\?} was shouting at a poor recruit who had tried to hold a sword by the wrong end. Yes, the pointy one. I hear they are looking for people with combat experience to help them train.", null);
                    StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject);
                    return textObject;
                }
            }

            public override bool IsThereAlternativeSolution
            {
                get
                {
                    return false;
                }
            }

            public override bool IsThereLordSolution
            {
                get
                {
                    return false;
                }
            }

            public override IssueFrequency GetFrequency()
            {
                return IssueBase.IssueFrequency.VeryCommon;
            }

            public override bool IssueStayAliveConditions()
            {
                if (IssueOwner.IsNoble && IssueOwner.IsPartyLeader)
                {
                    double lowTierTroops = 0;
                    double totalTroops = 0;

                    foreach (var troop in IssueOwner.PartyBelongedTo.MemberRoster.GetTroopRoster())
                    {
                        if (troop.Character.Tier == 1 || troop.Character.Tier == 2)
                        {
                            lowTierTroops += troop.Number;
                        }
                        totalTroops += troop.Number;
                    }

                    if (lowTierTroops / totalTroops >= 0.2)
                    {
                        return true;
                    }
                }

                return false;
            }

            protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
            {
                relationHero = null;
                flag = IssueBase.PreconditionFlags.None;
                if (issueGiver.GetRelationWithPlayer() < -10f)
                {
                    flag |= IssueBase.PreconditionFlags.Relation;
                    relationHero = issueGiver;
                }
                if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
                {
                    flag |= IssueBase.PreconditionFlags.AtWar;
                }
                if (Clan.PlayerClan.Tier < 1)
                {
                    flag |= IssueBase.PreconditionFlags.ClanTier;
                }
                if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 50)
                {
                    flag |= IssueBase.PreconditionFlags.NotEnoughTroops;
                }
                skill = null;
                return flag == IssueBase.PreconditionFlags.None;
            }

            protected override void CompleteIssueWithTimedOutConsequences()
            {
            }

            protected override QuestBase GenerateIssueQuest(string questId)
            {
                return new NobleWantsTrainingBattleQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(10f), 1000);
            }

            protected override void OnGameLoad()
            {
            }
        }

        internal class NobleWantsTrainingBattleQuest : QuestBase
        {
            public NobleWantsTrainingBattleQuest(string questId, Hero questGiver, CampaignTime duration, int rewardGold) : base(questId, questGiver, duration, rewardGold)
            {
            }

            public override TextObject Title => throw new NotImplementedException();

            public override bool IsRemainingTimeHidden => throw new NotImplementedException();

            protected override void InitializeQuestOnGameLoad()
            {
                throw new NotImplementedException();
            }

            protected override void SetDialogs()
            {
                throw new NotImplementedException();
            }
        }

        // Save data goes into this class
        public class NobleWantsTrainingBattleIssueTypeDefiner : SaveableTypeDefiner
        {
            public NobleWantsTrainingBattleIssueTypeDefiner() : base(585840)
            {
            }

            protected override void DefineClassTypes()
            {
                base.AddClassDefinition(typeof(NobleWantsTrainingBattleIssueBehavior.NobleWantsTrainingBattleIssue), 1);
                base.AddClassDefinition(typeof(NobleWantsTrainingBattleIssueBehavior.NobleWantsTrainingBattleQuest), 2);
            }
        }
        
        // Register this event to check for issue event
        public override void RegisterEvents()
        {
            CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
        }

        // Unused Sync Data method?
        public override void SyncData(IDataStore dataStore)
        {
        }

    }
}
