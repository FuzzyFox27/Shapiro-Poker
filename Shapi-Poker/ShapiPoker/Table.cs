using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class Table
    {
        public List<Card> presentOnTable = new List<Card>();
        public int currentPot = 0;
        public int highestBet = 0;

        //Add cards to the table -- the dealer
        public void AddCardsToTable(Card[] toPlace)
        {
            foreach (Card card in toPlace)
            {
                presentOnTable.Add(card);
            }
        }

        //Reset Table for next round
        public void RefreshTable()
        {
            presentOnTable.Clear();
            currentPot = 0;
            highestBet = 0;
        }

        public void PrintTable()
        {
            //for (int i = 0; i < presentOnTable.Count; i++)
            //{
            //    Console.WriteLine("In Position " + i + " is the " + presentOnTable[i].rank + " of " + presentOnTable[i].suit);
            //}
            if (presentOnTable.Count > 0)
            {
                printCards(presentOnTable);
            }

        }

        public List<Card> GetTable()
        {
            return presentOnTable;
        }

        public void printCards(List<Card> stackOfCardsPrinted)
        {
            string[] faceOfTheCard = { ".------.|      || ('v')||  " + (char)92 + " / ||   v  |'------'",
                                      ".------.|   ^  ||  / " + (char)92 + " ||  " + (char)92 + " / ||   v  |'------'",
                                      ".------.|   .  ||  / " + (char)92 + " || (_,_)||   T  |'------'",
                                      ".------.|   _  ||  ( ) || (_,_)||   T  |'------'"};


            for (int repeat = 0; repeat < stackOfCardsPrinted.Count; repeat++)
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
