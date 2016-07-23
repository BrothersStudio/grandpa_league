using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public void ConfirmOffer(bool offered, List<Child> children, List<Parent> parent, int money, Family family)
    {
        if (offered)
        {
            offerFamily = family;
            offeredMoney = money;
            offeredChar.AddRange(children.Cast<Character>());
            offeredChar.AddRange(parent.Cast<Character>());
        }
        else
        {
            proposedFamily = family;
            proposedMoney = money;
            proposedChar.AddRange(children.Cast<Character>());
            proposedChar.AddRange(parent.Cast<Character>());
        }
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
        offeredValue += this.offeredMoney;

        foreach (Character cur in this.proposedChar)
        {
            if (typeof(Parent) == cur.GetType())
                proposedValue += ((Parent)cur).Value;
            else if (typeof(Child) == cur.GetType())
                proposedValue += ((Child)cur).Value;
        }
        proposedValue *= 5.5;                           //give edge to AI
        proposedValue += this.offeredMoney;

        if (offeredValue > proposedValue)
            this.accepted = true;
    }

    public Outcome PerformTradeAction(DataManager dataManager)
    {
        Outcome tradeOutcome = new Outcome();
        if(!this.accepted)
        {
            tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
            tradeOutcome.OutcomeDescription = String.Format("I would never make that trade to you for such a worthless offer!");
            tradeOutcome.OutcomeDescription = String.Format("Dear {0}\n, \tI hope you'll never send such an unpleasant offer to me again. \n\nSincerely,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
        }
        else
        {
            if(this.OfferStillValid(out tradeOutcome))
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
                    this.proposedFamily.Grandpa.Money += this.offeredMoney;
                    this.offerFamily.Grandpa.Money -= this.offeredMoney;
                    tradedList += cur.Name + ", ";
                }

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
                    this.proposedFamily.Grandpa.Money -= this.proposedMoney;
                    this.offerFamily.Grandpa.Money += this.proposedMoney;
                    recievedList += cur.Name + ", ";
                }

                tradeOutcome.Status = (int)Enums.EventOutcome.SUCCESS;
                tradeOutcome.OutcomeDescription = String.Format("{0} has accepted your trade offer and the trade has been successfully registered with the county magistrate!", this.proposedFamily.Grandpa.Name);
                tradeOutcome.Mail = String.Format("Details of the completed offer are below:\n\nTraded: ${0}, {1} goodye!\n\nRecieved: ${2}, {3} enjoy your new home!", this.offeredMoney.ToString(), tradedList, this.proposedMoney.ToString(), recievedList);
            }
        }
        return tradeOutcome;
    }

    private bool OfferStillValid(out Outcome tradeOutcome)
    {
        //first check that the offer can actually still be done (5 days later at this point)
        tradeOutcome = new Outcome();
        if (offerFamily.Grandpa.Money < this.offeredMoney)
        {
            tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
            tradeOutcome.OutcomeDescription = String.Format("You don't have ${0} anymore, what are you trying to rip me off?", this.offeredMoney.ToString());
            tradeOutcome.OutcomeDescription = String.Format("Dear {0}\n, \tNow that I know you're a slimy bastard I'll be on the lookout next time. \n\nRegards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
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
                tradeOutcome.OutcomeDescription = String.Format("Dear {0}\n, \tI sincerely hope we never do buisness again. \n\nRegards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
                return false;
            }
        }
        foreach (Character cur in this.proposedChar)
        {
            if ((cur.GetType() == typeof(Child) && !proposedFamily.Children.Contains((Child)cur)) || (cur.GetType() == typeof(Parent) && !proposedFamily.Parents.Contains((Parent)cur)))
            {
                tradeOutcome.Status = (int)Enums.EventOutcome.FAILURE;
                tradeOutcome.OutcomeDescription = String.Format("Looks like {0} has already been traded away, drats!", cur.Name);
                tradeOutcome.OutcomeDescription = String.Format("Dear {0}\n, \tYou snooze you loose, sucker! \n\nWarm Regards,\n{1}", offerFamily.Grandpa.Name, proposedFamily.Grandpa.Name);
                return false;
            }
        }
        tradeOutcome.Status = (int)Enums.EventOutcome.SUCCESS;
        return false;
    }

    private void RegisterEventResponse()
    {
        DataManager datamanager = Main.GetDataManager();
        datamanager.Calendar.ScheduleEventInXDays(EventManager.GetSystemEventById((int)Enums.SystemEvents.TRADE_ACCEPT_REJECT), 5);
    }
}

