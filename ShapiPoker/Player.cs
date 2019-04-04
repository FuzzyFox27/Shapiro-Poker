using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class Player
    {
        public int playerID;
        public int currentChips = 0;

        //Per Turn Vars
        public int currentBet = 0;
        public bool inRound = true;
        public bool checking = false;
        public bool allIn = false;
        public Grades grade = Grades.HighCard;
        public Ranks highCard = Ranks.Two;

        //0 = Fold, 1 = Check, 2 = Call, 3 = Raise, 4 = Return to Menu
        public bool[] possibleActions = new bool[5];

        public Card[] hand = new Card[2];

        //Constructor, used for actual players
        public Player(int ID, int startingChips)
        {
            playerID = ID;
            currentChips = startingChips;
        }

        //Constructor, empty for temps
        public Player()
        {

        }

        //Reset Rounds Vars
        public void RefreshPlayer()
        {
            currentBet = 0;
            inRound = true;
            allIn = false;
            checking = false;
        }

        //Sets players hand to given cards
        public void ReceiveHand(Card[] givenHand)
        {
            hand = givenHand;
        }

        //Returns player card in given location
        public Card GetCardInHand(int position)
        {
            return hand[position];
        }

        public void SetPossibilities(bool[] actions)
        {
            for (int i = 0; i < possibleActions.Length; i++)
            {
                possibleActions[i] = actions[i];
            }
        }

        public Card[] getPlayerHand()
        {
            return hand;
        }

        public void Bet(int amount)
        {
            currentChips -= amount;
            currentBet += amount;
        }

        public void PrintHand()
        {
            Console.WriteLine("Player " + playerID + " has the " + hand[0].rank + " of " + hand[0].suit + " and the " + hand[1].rank + " of " + hand[1].suit);
        }
    }
}
