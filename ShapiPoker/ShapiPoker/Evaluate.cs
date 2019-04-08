///Summary
///So i have referenced the players list and table list which basically works as pointers in c# 
///This means anything we change here is changed over in the main file. This also gives us access 
///to the players hand and the cards on the table. The framework is here but the checks just need to 
///be hard coded and is a lot harder to concieve in code then you think...

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class Evaluate
    {
        public int[] DecideWinner(ref List<Player> players, ref Table table)
        {
            //Loop through all players -> check for royal flush -> if one they win, if two go into deeper check
            //If none loop through all players and check for straight flush -> if one they win, if two go into deeper check -> etc

            Card[] onTable = table.GetTable().ToArray();

            Console.WriteLine(" FI am HERE.....................");
            Console.ReadKey();
            for (int i = 0; i < players.Count; i++)
            {
                Card[] playerHand = players[i].getPlayerHand();
                //for (int j = 0; j < playerHand.Length; j++)
                //{
                //    Console.WriteLine(playerHand[j].rank + " " +playerHand[j].suit);
                //}
                //Console.WriteLine();
                //for (int j = 0; j < onTable.Length; j++)
                //{
                //    Console.WriteLine(onTable[j].rank + " " + onTable[j].suit);
                //}
                //Console.WriteLine();
                Card[] allCards = new Card[onTable.Length + playerHand.Length];
                onTable.CopyTo(allCards, 0);
                playerHand.CopyTo(allCards, onTable.Length);

                for(int j = 0;j < allCards.Length;j++)
                {
                    Console.WriteLine(allCards[j].rank + " " + allCards[j].suit);
                }


                if (CalculateGrade(allCards, 0)== true)
                    players[i].grade = Grades.RoyalFlush;
                else if (CalculateGrade(allCards, Grades.StraightFlush) == true)
                    players[i].grade = Grades.StraightFlush;
                else if (CalculateGrade(allCards, Grades.FourOfAKind) == true)
                    players[i].grade = Grades.FourOfAKind;
                else if (CalculateGrade(allCards, Grades.FullHouse) == true)
                    players[i].grade = Grades.FullHouse;
                else if (CalculateGrade(allCards, Grades.Flush) == true)
                    players[i].grade = Grades.Flush;
                else if (CalculateGrade(allCards, Grades.Straight) == true)
                    players[i].grade = Grades.Straight;
                else if (CalculateGrade(allCards, Grades.ThreeOfAKind) == true)
                    players[i].grade = Grades.ThreeOfAKind;
                else if (CalculateGrade(allCards, Grades.TwoPairs) == true)
                    players[i].grade = Grades.TwoPairs;
                else if (CalculateGrade(allCards, Grades.Pair) == true)
                    players[i].grade = Grades.Pair;
                else
                    players[i].grade = Grades.HighCard;
            }

            List<int> winners = new List<int>();
            int lowestGrade = 10;

            //Loop through all players -> Check if their grade is lower i.e. better hand -> replace the lowest value
            //If grade is the same, they have the same value hand -> decide which has highest card

            for (int i = 0; i < players.Count; i++)
            {
                if ((int)players[i].grade < lowestGrade)
                {
                    winners.Clear();
                    winners.Add(i);
                    lowestGrade = (int)players[i].grade;
                }
                else if ((int)players[i].grade == lowestGrade)
                {
                    winners.Add(i);
                }
            }

            //Check for highest card
            if (winners.Count == 1) //One Winner
            {
                return winners.ToArray();
            }
            else if (winners.Count > 1) //Multiple Potential Winners
            {
                int highestCard = -1;
                List<int> positionOfPlayer = new List<int>();

                for (int i = 0; i < winners.Count; i++)
                {
                    int high = GetHighestCard(players[winners[i]].getPlayerHand());

                    if (high > highestCard)
                    {
                        highestCard = high;
                        positionOfPlayer.Clear();
                        positionOfPlayer.Add(i);
                    }
                    else if (high == highestCard)
                    {
                        positionOfPlayer.Add(i);
                    }
                }

                if (positionOfPlayer.Count == 1) //One Person has highest card
                {
                    int[] data = new int[] { winners[positionOfPlayer[0]] };
                    return data;
                }
                else // Multiple people have same highest card
                {
                    List<int> data = new List<int>();

                    for (int i = 0; i < positionOfPlayer.Count; i++)
                    {
                        data.Add(winners[positionOfPlayer[i]]);
                    }
                    return data.ToArray();
                }
            }
            else
            {
                int[] data = new int[] { -1 };
                return data;
            }
        }


        int GetHighestCard(Card[] cards)
        {
            //Handles that ACE = 0
            if ((int)cards[0].rank == 0 || (int)cards[1].rank == 0)
            {
                return 13;
            }
            //Handles the Rest
            else if ((int)cards[0].rank > (int)cards[1].rank)
            {
                return (int)cards[1].rank;
            }
            else if ((int)cards[0].rank < (int)cards[1].rank)
            {
                return (int)cards[0].rank;
            }
            //Both card is the same so return any
            else
            {
                return (int)cards[0].rank;
            }
        }

        public enum Grades
        {
            RoyalFlush, StraightFlush, FourOfAKind, FullHouse, Flush, Straight, ThreeOfAKind, TwoPairs, Pair, HighCard
        }

        public bool CalculateGrade(Card[] allCards, Grades grade) {

            int grades = (int)grade;
            Console.WriteLine(grades);
            Console.ReadKey();
            switch (grades){
                case 0: //Royal flush
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCardsLessThan10 = 0;
                        for (int i = 0; i < allCards.Length; i++)
                        {
                            if ((int)allCards[i].rank < 9 && allCards[i].rank > 0)
                            {
                                countCardsLessThan10++;
                            }
                            else
                            {
                                if (countCardsLessThan10 < 3)
                                {
                                    if (differentCards.Count == 0)
                                    {
                                        differentCards.Add(allCards[i]);
                                    }
                                    else
                                    {
                                        if (differentCards.Count < 5)
                                        {
                                            bool goodCard = false;
                                            foreach (Card c in differentCards)
                                            {
                                                if (allCards[i].rank == c.rank || allCards[i].suit != c.suit)
                                                {
                                                    goodCard = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    goodCard = true;
                                                }
                                            }
                                            if (goodCard == true)
                                            {
                                                differentCards.Add(allCards[i]);
                                            }
                                        }
                                        else
                                            break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                
                            }
                            
                        }
                        if (differentCards.Count == 5)
                        {
                            Console.ReadKey();
                            return true;
                        }
                    }
                break;
                
                case 1: //Straight flush
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        List<Card> descendedAllCards = allCards.OrderBy(Card => Card.rank).ToList();                     
                        for (int i = 0; i < descendedAllCards.Count - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i+1];
                            if (indexBreak < 3)
                            {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);
                                    if (currentCard.rank == nextCard.rank - 1 && currentCard.suit == nextCard.suit)
                                    {
                                        differentCards.Add(descendedAllCards[i]);
                                    }
                                    else
                                    {
                                        indexBreak++;
                                        differentCards.Clear();
                                    }

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {
                                        Console.WriteLine(currentCard.rank +" "+ nextCard.rank + " " + currentCard.suit + " " + nextCard.suit);
                                        if (currentCard.rank == nextCard.rank - 1 && currentCard.suit == nextCard.suit)
                                        {
                                            differentCards.Add(descendedAllCards[i]);
                                        }
                                        else
                                        {
                                            indexBreak++;
                                            differentCards.Clear();
                                        }
                                    }
                                    else
                                        break;
                                }
                                    
                            }
                            else
                            {
                                 break;
                            }

                            

                        }
                        if (differentCards.Count >= 5)
                        {
                            Console.WriteLine(" Straight flush.....................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 2:  //Four of a kind
                    if (allCards.Length > 4)
                    {
                        int countCard = 0;
                        bool fourOfAKind = false;
                        for (int i = 0; i < 5; i++)
                        {
                            for(int j = i + 1; j < allCards.Length; j++)
                            {
                                if(allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                }
                            }
                            if (countCard < 3)
                            {
                                countCard = 0; 
                            }
                            else
                            {
                                fourOfAKind = true;
                            }

                        }
                        if(fourOfAKind == true)
                        {
                            Console.WriteLine(" Four of a kind......................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 3: // Full House
                    if (allCards.Length > 4)
                    {
                        List<Card> allCardsList = allCards.ToList();
                        int countCard = 0;
                        int position = 0;//position of one of the cards from the threeOfAKind 
                        bool pair = false;
                        bool threeOfAKind = false;
                        for (int i = 0; i < allCardsList.Count - 1; i++)
                        {
                            for (int j = i + 1; j < allCardsList.Count; j++)
                            {
                                if (allCardsList[i].rank == allCardsList[j].rank)
                                {
                                    countCard++;
                                    position = j;
                                }
                            }
                            if (countCard < 2)
                            {
                                if (countCard < 1)
                                {
                                    countCard = 0;
                                }
                                else
                                {
                                    pair = true;
                                    countCard = 0;
                                }
                            }
                            else
                            {
                                threeOfAKind = true;
                                allCardsList.RemoveAt(position);
                                countCard = 0;
                            }
                            

                        }
                        if (pair == true && threeOfAKind == true)
                        {
                            Console.WriteLine(" Full House......................");
                            Console.ReadKey();

                            return true;
                        }
                    }

                    break;
                case 4: //Flush
                    if (allCards.Length > 4)
                    {
                        int countCard = 0;
                        bool flush = false;
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                             //   Console.WriteLine(allCards[i].suit +" "+ allCards[j].suit);
                                if (allCards[i].suit == allCards[j].suit)
                                {
                                    countCard++;
                                }
                            }
                           // Console.WriteLine(allCards[i].suit + " " + countCard);
                            if (countCard >= 4)
                            {
                                flush = true;
                            }
                            else
                            {
                                countCard=0;
                            }

                        }
                        if (flush == true)
                        {
                            Console.WriteLine(" Flush......................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 5: // Straight
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        List<Card> descendedAllCards = allCards.OrderBy(Card => Card.rank).ToList();
                        for (int i = 0; i < descendedAllCards.Count - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i + 1];
                            if (indexBreak < 5)
                            {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);
                                    if (currentCard.rank == nextCard.rank - 1)
                                    {
                                        differentCards.Add(descendedAllCards[i]);
                                    }
                                    else
                                    {
                                        indexBreak++;
                                        differentCards.Clear();
                                    }

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {
                                        if (currentCard.rank == nextCard.rank - 1)
                                        {
                                            differentCards.Add(descendedAllCards[i]);
                                        }
                                        else
                                        {
                                            indexBreak++;
                                            differentCards.Clear();
                                        }
                                    }
                                    else
                                        break;
                                }

                            }
                            else
                            {
                                break;
                            }



                        }
                        if (differentCards.Count >= 5)
                        {
                            Console.WriteLine(" Straight .....................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 6: //Three of a kind
                    if (allCards.Length > 4)
                    {
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        bool threeOfAKind = false;
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                }
                            }
                            if(countCard < 2)
                            {
                                countCard = 0;
                            }
                            else
                            {
                                threeOfAKind = true;
                            }

                        }
                        if (threeOfAKind == true)
                        {
                            Console.WriteLine(" Three of Kind......................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 7: // Two pair
                    if (allCards.Length > 4)
                    {
                        List<Card> allCardsList = allCards.ToList();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        int countPairs = 0;
                        for (int i = 0; i < allCardsList.Count-1; i++)
                        {
                            for (int j = i + 1; j < allCardsList.Count; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countPairs++;
                                }
                                
                            }
                            if(countPairs == 2)
                            {
                                Console.WriteLine(" Two Pair......................");
                                Console.ReadKey();
                                return true;
                            }

                        }
                        if (countPairs == 2)
                        {
                            Console.WriteLine(" Two Pair......................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
                case 8: // Pair
                    Console.WriteLine(allCards.Length + " Pair......................");
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                }
                            }

                        }
                        Console.WriteLine(countCard + " countCard");
                        if (countCard == 1)
                        {
                            //Console.WriteLine(countCard);
                            Console.WriteLine("Pair......................");
                            Console.ReadKey();
                            return true;
                        }
                    }

                    break;
            }
            return false;
        }
    }
  
}