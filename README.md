# README

## Mod Information
Nexus Mods Link: https://www.nexusmods.com/mountandblade2bannerlord/mods/3066?tab=description&BH=0

## Implemented Quests
### Merchant Quests:
    - {Settlement} Needs Food
        - Town needs food to combat starvation, give food to merchant to complete
          Accepts Grain/Fish/Meat, pays triple the current average price for each
          Mission is completed after 300 food given
          Adds 300 food to town upon quest completion (stops starvation), plus relationship bonus with all notables in town

### Village Quests:
    - Bandit Army Raiding {Settlement}
        - Bandit Army is spawned at hideout based on player Clan tier, heading to nearby Village, defeat bandit army
          Bandit army consists of 50% bandits based on hideout culture (eg sea raiders, forest bandits etc), and 50% looters
          If bandits reach village they will begin raiding, stop them from finishing the raid
          Rewards 3000 gold as "bounty" from nobles plus relationship bonus with all notables in village

    - {Settlement} Needs Militia Weapons
        - Simple quest, return 20 one handed polearms to village to complete
          Increases number of militia in village by 20
          Quest only occurs in villages with less then 15 militia

### Artisan Quests:
    - Rebellion in {ISSUE_SETTLEMENT}!
        - In a low loyalty town (<50 ), the people are going to rise up!
          Fight a small battle against the garrison alongside the militia of the town, on success, force a rebellion in that town
          Currently a 10vs10 due to alley fight limitation, might up it later if I figure out how to increase the size of the battle


## Non-Implemented Quests

### Noble Quests:
    - {NobleName} wants training battle
        - Noble wants to train his troops (has to many tier 1-2 troops), offers a sparring battle with player.
          Small battle, ~50 troops per side (change based on difficulty?)
          No fatalities during combat, only Knock-outs
          Bonus party experience and gold reward for player if they win the fight
          upon quest completion, levels up all tier 1-2 troops for player and noble



## Modding notes

The code is included in the folder AdditionalQuestsCode (including solution for visual studio)
You will need to redo the references based on where you installed Bannerlord



# Saving Issue + Quest data
When creating Issues, had to choose an unused save ID that didnt clash with existing ones:
Eg
```
public class HeadmanNeedsHardWoodIssueTypeDefiner : SaveableTypeDefiner
    {
        // Ensure the number in the base() is unique
        public HeadmanNeedsHardWoodIssueTypeDefiner() : base(1000500)
        {
        }
        
        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(HeadmanNeedsHardWoodIssueBehavior.HeadmanNeedsHardWoodIssue), 1);
            base.AddClassDefinition(typeof(HeadmanNeedsHardWoodIssueBehavior.HeadmanNeedsHardWoodIssueQuest), 2);
        }
    }
```
