using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Trade
{
    private Family offerFamily = null;
    private List<Character> offeredChar = new List<Character>();
    private int offeredMoney = 0;

    private Family proposedFamily = null;
    private List<Character> proposedChar = new List<Character>();
    private int proposedMoney = 0;

    private bool accepted = false;

    public Trade()
    {
    }

	public void SetOffer(Family offer_family, List<Child> offer_children, List<Parent> offer_parent, int offer_money, 
						 Family proposed_family, List<Child> proposed_children, List<Parent> proposed_parent, int proposed_money)
    {
		offerFamily = offer_family;
		offeredChar.AddRange(offer_children.Cast<Character>());
		offeredChar.AddRange(offer_parent.Cast<Character>());
		offeredMoney = offer_money;
       
		proposedFamily = proposed_family;
		proposedChar.AddRange(proposed_children.Cast<Character>());
		proposedChar.AddRange(proposed_parent.Cast<Character>());
		proposedMoney = proposed_money;     
    }

    public void ConfirmOffer()
    {
        this.DetermineAcceptReject();
        this.RegisterEventResponse();
    }

    private void DetermineAcceptReject()
    {
        double proposedValue = 0;
        double offeredValue = 0;

        foreach (Character cur in this.offeredChar)
        {
            if (typeof(Parent) == cur.GetType())
                offeredValue += ((Parent)cur).Value;
            else if (typeof(Child) == cur.GetType())
                offeredValue += ((Child)cur).Value;
        }
        offeredValue *= 5;
        offeredValue += this.offeredMoney * 0.70;

        foreach (Character cur in this.proposedChar)
        {
            if (typeof(Parent) == cur.GetType())
                proposedValue += ((Parent)cur).Value;
            else if (typeof(Child) == cur.GetType())
                proposedValue += ((Child)cur).Value;
        }
        proposedValue *= 7;                           //give edge to AI
        proposedValue += this.proposedMoney * 0.75;

        if (Main.GetDataManager().PlayerInfo.FIRST_TRADE & offeredChar.Count <= 2 && proposedChar.Count <= 2 && Math.Abs(offeredMoney - proposedMoney) <= 1000)
        {
            this.accepted = true;
            Main.GetDataManager().PlayerInfo.FIRST_TRADE = false;
        }
        else if (offeredValue > proposedValue)
        {
            this.accepted = true;
        }
    }

    public Outcome PerformTradeAction(DataManager dataManager)
    {
        Outcome tradeOutcome = new Outcome();
        if(!this.accepted)
        {
            tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
            tradeOutcome.OutcomeDescription = String.Format("{0}\n\nI would never make that trade to you for such a worthless offer!", proposedFamily.Grandpa.Name);
            tradeOutcome.Mail = new Mail();
            tradeOutcome.Mail.Subject = "RE: Trade offer";
            tradeOutcome.Mail.Sender = proposedFamily.Grandpa.Name;
            tradeOutcome.Mail.Date = dataManager.Calendar.GetCurrentDay();
            tradeOutcome.Mail.Message = String.Format("Dear {0},\n\n \tI hope you'll refrain from sending me such an unpleasant offer to me again. \n\nSincerely,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
        }
        else
        {
            if(this.OfferStillValid(out tradeOutcome, dataManager))
            {
                string tradedList = "";
                foreach(Character cur in offeredChar)
                {
                    if(cur.GetType() == typeof(Parent))
                    {
                        this.offerFamily.Parents.Remove((Parent)cur);
                        this.proposedFamily.Parents.Add((Parent)cur);
                    }
                    else if(cur.GetType() == typeof(Child))
                    {
                        this.offerFamily.Children.Remove((Child)cur);
                        this.proposedFamily.Children.Add((Child)cur);
                    }
                    tradedList += cur.Name + ", ";
                }
                this.proposedFamily.Grandpa.Money += this.offeredMoney;
                this.offerFamily.Grandpa.Money -= this.offeredMoney;
                
                string recievedList = "";
                foreach (Character cur in proposedChar)
                {
                    if (cur.GetType() == typeof(Parent))
                    {
                        this.offerFamily.Parents.Add((Parent)cur);
                        this.proposedFamily.Parents.Remove((Parent)cur);
                    }
                    else if (cur.GetType() == typeof(Child))
                    {
                        this.offerFamily.Children.Add((Child)cur);
                        this.proposedFamily.Children.Remove((Child)cur);
                    }
                    recievedList += cur.Name + ", ";
                }
                this.proposedFamily.Grandpa.Money -= this.proposedMoney;
                this.offerFamily.Grandpa.Money += this.proposedMoney;
                
                tradeOutcome.Status = (int)Enums.EventOutcome.SUCCESS;
                tradeOutcome.OutcomeDescription = String.Format("{0} has accepted your trade offer and the trade has been successfully registered with the county magistrate!", this.proposedFamily.Grandpa.Name);
                tradeOutcome.Mail = new Mail();
                tradeOutcome.Mail.Subject = "RE: Trade offer successfully processed";
                tradeOutcome.Mail.Sender = "Office of the County Magistrate";
                tradeOutcome.Mail.Date = dataManager.Calendar.GetCurrentDay();
                tradeOutcome.Mail.Message = String.Format("Details of the completed offer are below:\n\nTraded: ${0}, {1}goodbye!\n\nRecieved: ${2}, {3}enjoy your new home!", this.offeredMoney.ToString(), tradedList, this.proposedMoney.ToString(), recievedList);
            }
        }
        return tradeOutcome;
    }

    private bool OfferStillValid(out Outcome tradeOutcome, DataManager dataManager)
    {
        //first check that the offer can actually still be done (5 days later at this point)
        tradeOutcome = new Outcome();
        if (offerFamily.Grandpa.Money < this.offeredMoney)
        {
            tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
            tradeOutcome.OutcomeDescription = String.Format("You don't have ${0} anymore, what are you trying to rip me off?", this.offeredMoney.ToString());
            tradeOutcome.Mail = new Mail();
            tradeOutcome.Mail.Subject = "RE: Trade offer invalidated";
            tradeOutcome.Mail.Sender = proposedFamily.Grandpa.Name;
            tradeOutcome.Mail.Date = dataManager.Calendar.GetCurrentDay();
            tradeOutcome.Mail.Message = String.Format("Dear {0},\n\n \tOffering the money when you can't follow up? Now that I know you're a slimy bastard I'll be on the lookout next time. \n\nRegards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
            return false;
        }
        else if (proposedFamily.Grandpa.Money < this.proposedMoney)
        {
            proposedFamily.Grandpa.Money += this.proposedMoney - proposedFamily.Grandpa.Money;      //give proposed AI family the money if they dont have it
        }

        foreach (Character cur in this.offeredChar)
        {
            if ((cur.GetType() == typeof(Child) && !offerFamily.Children.Contains((Child)cur)) || (cur.GetType() == typeof(Parent) && !offerFamily.Parents.Contains((Parent)cur)))
            {
                tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
                tradeOutcome.OutcomeDescription = String.Format("Hey, you don't have {0} anymore, what are you trying to rip me off?", cur.Name);
                tradeOutcome.Mail = new Mail();
                tradeOutcome.Mail.Subject = "RE: Trade offer invalidated";
                tradeOutcome.Mail.Sender = proposedFamily.Grandpa.Name;
                tradeOutcome.Mail.Date = dataManager.Calendar.GetCurrentDay();
                tradeOutcome.Mail.Message = String.Format("Dear {0},\n\n \tI didn't want your dirty kids anyways. I sincerely hope we never do buisness again. \n\nRegards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
                return false;
            }
        }
        foreach (Character cur in this.proposedChar)
        {
            if ((cur.GetType() == typeof(Child) && !proposedFamily.Children.Contains((Child)cur)) || (cur.GetType() == typeof(Parent) && !proposedFamily.Parents.Contains((Parent)cur)))
            {
                tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
                tradeOutcome.OutcomeDescription = String.Format("Looks like {0} has already been traded away, drats!", cur.Name);
                tradeOutcome.Mail = new Mail();
                tradeOutcome.Mail.Subject = "RE: Trade offer invalidated";
                tradeOutcome.Mail.Sender = proposedFamily.Grandpa.Name;
                tradeOutcome.Mail.Date = dataManager.Calendar.GetCurrentDay();
                tradeOutcome.Mail.Message = String.Format("Dear {0},\n\n \tYou snooze you loose, sucker! I got a pretty penny while you were snoozing! \n\nWarm Regards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
                return false;
            }
        }
        tradeOutcome.Status = (int)Enums.EventOutcome.SUCCESS;
        return true;
    }

    private void RegisterEventResponse()
    {
        DataManager datamanager = Main.GetDataManager();
        SimulationEvent newTrade = EventManager.GetSystemEventById((int)Enums.SystemEvents.TRADE_ACCEPT_REJECT);
        newTrade.Requirements.Trade.Add(this);
        datamanager.Calendar.ScheduleEventInXDays(newTrade, 5);
    }
}

