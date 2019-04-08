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

            for (int i = 0; i < players.Count; i++)
            {
                Card[] playerHand = players[i].getPlayerHand();
                Card[] allCards = new Card[onTable.Length + playerHand.Length];
                Grade g;
                foreach(var g1 in g)
                {
                    if (CalculateGrade(allCards, g1))
                    {
                        players[i].grade = g1;
                        break;
                    }
                }

                //if (CheckRoyalFlush(allCards))
                //    players[i].grade = Grades.RoyalFlush;
                //else if (CheckStraightFlush(allCards))
                //    players[i].grade = Grades.StraightFlush;
                //else if (CheckFourOfAKind(allCards))
                //    players[i].grade = Grades.FourOfAKind;
                //else if (CheckFullHouse(allCards))
                //    players[i].grade = Grades.FullHouse;
                //else if (CheckFlush(allCards))
                //    players[i].grade = Grades.Flush;
                //else if (CheckStraight(allCards))
                //    players[i].grade = Grades.Straight;
                //else if (CheckThreeOfAKind(allCards))
                //    players[i].grade = Grades.ThreeOfAKind;
                //else if (CheckTwoPair(allCards))
                //    players[i].grade = Grades.TwoPairs;
                //else if (CheckPair(allCards))
                //    players[i].grade = Grades.Pair;
                //else
                //    players[i].grade = Grades.HighCard;
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

        //bool CheckRoyalFlush(Card[] allCards)
        //{

        //    return false;
        //}

        //bool CheckStraightFlush(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckFourOfAKind(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckFullHouse(Card[] allCards)
        //{

        //    return false;
        //}

        //bool CheckFlush(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckStraight(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckThreeOfAKind(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckTwoPair(Card[] allCards)
        //{
        //    return false;
        //}

        //bool CheckPair(Card[] allCards)
        //{
        //    return false;
        //}

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
            switch (grades){
                case 0: //Royal flush
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCardsLessThan10 = 0;
                        for (int i = 0; i < allCards.Length; i++)
                        {
                            if((int)allCards[i].rank < 10)
                            {
                                countCardsLessThan10++;
                                break;
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
                                            foreach (Card c in differentCards)
                                            {
                                                if (allCards[i].rank == c.rank || allCards[i].suit != c.suit)
                                                    break;
                                                else
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
                        if(differentCards.Count == 5)
                        {
                            return true;
                        }
                    }
                break;
                
                case 1: //Straight flush
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        List<Card> descendedAllCards = allCards.OrderBy(Card => Card.suit).ToList();
                        for (int i = 0; i < allCards.Length - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i+1];

                            if (indexBreak < 2)
                                {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {

                                        if (currentCard.rank - 1 == nextCard.rank && currentCard.suit == nextCard.suit)
                                        {
                                            differentCards.Add(allCards[i]);
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
                        if (differentCards.Count == 5)
                        {
                            return true;
                        }
                    }

                    break;
                case 2:  //Four of a kind
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        for (int i = 0; i < 5; i++)
                        {
                           for(int j = i + 1; j < allCards.Length; j++)
                           {
                                if(allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                }
                           }

                        }
                        if(countCard == 3)
                        {
                            return true;
                        }
                    }

                    break;
                case 3: // Full House
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                }
                            }
                            if (countCard < 2)
                            {
                                countCard = 0;
                            }

                        }
                        if (countCard == 2)
                        {
                            return true;
                        }
                    }

                    break;
                case 4: //Flush
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].suit == allCards[j].suit)
                                {
                                    countCard++;
                                }
                            }

                        }
                        if (countCard == 4)
                        {
                            return true;
                        }
                    }

                    break;
                case 5: // Straight
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        List<Card> descendedAllCards = allCards.OrderBy(Card => Card.suit).ToList();
                        for (int i = 0; i < allCards.Length - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i + 1];

                            if (indexBreak < 2)
                            {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {

                                        if (currentCard.rank - 1 == nextCard.rank)
                                        {
                                            differentCards.Add(allCards[i]);
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
                        if (differentCards.Count == 5)
                        {
                            return true;
                        }

                    }

                    break;
                case 6: //Three of a kind
                    if (allCards.Length < 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        for (int i = 0; i < 5; i++)
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

                        }
                        if (countCard == 2)
                        {
                            return true;
                        }
                    }

                    break;
                case 8: // Two pair
                    if (allCards.Length < 4)
                    {
                        List<Card> allCardsList = allCards.ToList();
                        int countCard = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        int countPairs = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = i + 1; j < allCardsList.Count; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countPairs++;
                                    allCardsList.RemoveAt(i);
                                    allCardsList.RemoveAt(j);
                                    i = 0;
                                }
                                
                            }
                            if(countPairs == 2)
                            {
                                return true;
                            }

                        }
                        if (countCard == 4)
                        {
                            return true;
                        }
                    }

                    break;
                case 9: // Pair
                    if (allCards.Length < 4)
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
                        if (countCard == 1)
                        {
                            return true;
                        }
                    }

                    break;
            }
            return false;
        }
    }
  
}