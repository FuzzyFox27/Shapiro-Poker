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
        public int gamesWon = 0;

        public int blindPaid = 0;

        //Per Turn Vars
        public int currentBet = 0;
        public bool inRound = true;
        public bool checking = false;
        public bool allIn = false;
        public Evaluate.Grades grade = Evaluate.Grades.HighCard;
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

        public void setBlind(int amount)
        {
            blindPaid = amount;
        }

        public void PrintHand()
        {
            //Console.WriteLine("Player " + playerID + " has the " + hand[0].rank + " of " + hand[0].suit + " and the " + hand[1].rank + " of " + hand[1].suit);
            printCards(hand);
        }

        //print the cards on the console
        public void printCards(Card[] stackOfCardsPrinted)
        {
            string[] faceOfTheCard = { ".------.|      || ('v')||  " + (char)92 + " / ||   v  |'------'",
                                      ".------.|   ^  ||  / " + (char)92 + " ||  " + (char)92 + " / ||   v  |'------'",
                                      ".------.|   .  ||  / " + (char)92 + " || (_,_)||   T  |'------'",
                                      ".------.|   _  ||  ( ) || (_,_)||   T  |'------'"};

            for (int repeat = 0; repeat < stackOfCardsPrinted.Length; repeat++)
            {
                string rankCard = ((int)stackOfCardsPrinted[repeat].rank).ToString();
                if ((int)stackOfCardsPrinted[repeat].rank > 9 || stackOfCardsPrinted[repeat].rank == 0)
                {

                    if ((int)stackOfCardsPrinted[repeat].rank + 1 == 11)
                        rankCard = "J";
                    if ((int)stackOfCardsPrinted[repeat].rank + 1 == 12)
                        rankCard = "Q";
                    if ((int)stackOfCardsPrinted[repeat].rank + 1 == 13)
                        rankCard = "K";
                    if (stackOfCardsPrinted[repeat].rank == 0)
                        rankCard = "A";
                }
                else
                {
                    rankCard = ((int)stackOfCardsPrinted[repeat].rank + 1).ToString(); ;
                }
                string faceCard = faceOfTheCard[(int)stackOfCardsPrinted[repeat].suit];

                for (int i = 0; i < 48; i++)
                {
                    if (i == 9)
                    {
                        Console.Write(rankCard);
                        if (rankCard == "10")
                        {
                            i++;
                        }
                    }
                    else
                    {
                        Console.Write(faceCard[i]);
                    }
                    if ((i + 1) % 8 == 0 && i + 1 != 48)
                    {
                        Console.WriteLine();
                        Console.SetCursorPosition(Console.CursorLeft + (repeat * 10), Console.CursorTop);
                    }

                }
                Console.SetCursorPosition(Console.CursorLeft + 2, Console.CursorTop - 5);
            }
            Console.SetCursorPosition(0, Console.CursorTop + 6);
        }
    }
}
