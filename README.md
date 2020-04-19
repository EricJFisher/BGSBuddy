# BGSBuddy

BGS buddy is a simple application for generating an Elite Dangerous Back Ground Situation Report. Currently it is hard coded and only works for the minor player faction "Alliance Rapid-reaction Corps", however; I'll expand it to allow for other player factions later.

## Critical Reports
 - Systems where you are at risk of losing control of the system due to conflict

## Warning Reports
 - Systems where you're in danger of conflict that won't gain you any assets
 - Systems where data is stale (over 24 hours out of date)
 
## Opportunity Reports
 - Systems where you are near a conflict that can gain you a new asset
 - Systems that aren't on your "off limits" list that you aren't the controlling faction
 
## Controlled Systems
 - All systems currently controlled by your minor player faction
 
## FAQ
 - How do I update the off limits systems? It's hard coded so you can't yet, I'll add that later.
 - How do I use this? Open it, and it'll do the rest, press Update if you want to manually trigger an update. (it updates automatically when you open the application)
 - Do you accept pull requests? Yes, submit it and as long as it's desirable functionality (from my perspective, sorry) I'll check it out and bring it in!
 - Why is the code so bad? Check the Code Situation section, I just hacked this together with no intention of it being a thing, and well... it's a thing now I guess... so yeah... my bad... I'll work on it.

## Code situation

I've decided to open source it to share with others, however; it was a proof of concept hack job so the code is not up to the quality it should be, I will correct this over time, but just advising it's a bit of an untestable mess atm, you've been warning.
