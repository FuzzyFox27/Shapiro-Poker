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
            Console.WriteLine();
            for (int i = 0; i < players.Count; i++)
            {
                Card[] playerHand = players[i].getPlayerHand();                
                Card[] allCards = new Card[onTable.Length + playerHand.Length]; // array that store the cards from the board and the player i hand
                onTable.CopyTo(allCards, 0);
                playerHand.CopyTo(allCards, onTable.Length);
                //Ranks[] HighCards = new Ranks[3];

                Console.Write(i+":");
                if (CalculateGrade(allCards, 0, players[i].highCards, false) == true)
                    players[i].grade = Grades.RoyalFlush;
                else if (CalculateGrade(allCards, Grades.StraightFlush, players[i].highCards, false) == true)
                    players[i].grade = Grades.StraightFlush;
                else if (CalculateGrade(allCards, Grades.FourOfAKind, players[i].highCards, false) == true)
                    players[i].grade = Grades.FourOfAKind;
                else if (CalculateGrade(allCards, Grades.FullHouse, players[i].highCards, false) == true)
                    players[i].grade = Grades.FullHouse;
                else if (CalculateGrade(allCards, Grades.Flush, players[i].highCards, false) == true)
                    players[i].grade = Grades.Flush;
                else if (CalculateGrade(allCards, Grades.Straight, players[i].highCards, false) == true)
                    players[i].grade = Grades.Straight;
                else if (CalculateGrade(allCards, Grades.ThreeOfAKind, players[i].highCards, false) == true)
                    players[i].grade = Grades.ThreeOfAKind;
                else if (CalculateGrade(allCards, Grades.TwoPairs, players[i].highCards, false) == true)
                    players[i].grade = Grades.TwoPairs;
                else if (CalculateGrade(allCards, Grades.Pair, players[i].highCards, false) == true)
                {
                    players[i].grade = Grades.Pair;
                    if(players[i].hand[0].rank < players[i].hand[1].rank)
                    {
                        players[i].highCards[1] = players[i].hand[0].rank;
                    }
                    else
                    {
                        players[i].highCards[1] = players[i].hand[1].rank;
                    }
                }
                else
                {
                    Console.WriteLine("High Card");
                    players[i].grade = Grades.HighCard;
                }
                //for(int j = 0; j < HighCards.Length; j++)
                //{
                //    players[i].highCards[j] = HighCards[j];
                //}
            }
            Console.ReadKey();
            List<int> winners = new List<int>();
            int lowestGrade = 10;

            //Loop through all players -> Check if their grade is lower i.e. better hand -> replace the lowest value
            //If grade is the same, they have the same value hand -> decide which has highest card

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].inRound)
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
                
            }

            //Check for highest card
            if (winners.Count == 1) //One Winner
            {
                return winners.ToArray();
            }
            else if (winners.Count > 1) //Multiple Potential Winners
            {
                int repeat;
                List<int> positionOfPlayer = new List<int>();
                if (lowestGrade != 1 && lowestGrade != 5)
                {
                    repeat = players[winners[0]].highCards.Count();
                }
                else
                {
                    repeat = 1;
                }
                bool removeNextPlayer = false;
                for (int i = 0; i < winners.Count - 1; i++)
                {
                    for (int j = 0; j < repeat; j++)
                    {
                        if(winners.Count > 1)
                        { 
                            if (removeNextPlayer == true)
                            {
                                if(i != 0)
                                    i--;
                                removeNextPlayer = false;
                            }
                            Ranks high2 = players[winners[i + 1]].highCards[j];
                            Ranks high1 = players[winners[i]].highCards[j];

                            if (high1 > high2)
                            {
                                //Console.WriteLine(winners.Count);
                                for (int g = 0; g < winners.Count; g++)
                                {
                                    //Console.WriteLine(winners[g]);
                                }
                                Console.WriteLine();
                                winners.RemoveAt(i + 1);
                                //Console.WriteLine(winners.Count);
                                for (int g = 0; g < winners.Count; g++)
                                {
                                    //Console.WriteLine(winners[g]);
                                }
                                if (positionOfPlayer.Count != 0)
                                {
                                    positionOfPlayer.Clear();
                                }
                                positionOfPlayer.Add(i);
                                removeNextPlayer = true;
                            }
                            else
                            {
                                if (high1 < high2)
                                {

                                    if (positionOfPlayer.Count != 0)
                                    {
                                        positionOfPlayer.Clear();
                                    }
                                    positionOfPlayer.Add(i + 1);
                                }
                                else
                                {
                                    if (high1 == high2 && (j + 1) == repeat)
                                    {
                                        if (positionOfPlayer.Count == 0)
                                        {
                                            positionOfPlayer.Add(i);
                                            positionOfPlayer.Add(i + 1);
                                        }
                                        else
                                        {
                                            positionOfPlayer.Add(i + 1);
                                        }
                                    }
                                }
                            }
                        }
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

        public bool CalculateGrade(Card[] allCards, Grades grade, Ranks[] HighCards, bool Sim) {

            int grades = (int)grade;
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
                            if(!Sim) Console.WriteLine("Royal Flush...............");
                            return true;
                        }
                    }
                break;
                
                case 1: //Straight flush
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh
                        List<Card> descendedAllCards = allCards.OrderByDescending(Card => Card.rank).ToList();                     
                        for (int i = 0; i < descendedAllCards.Count - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i+1];
                            if (indexBreak < 3)
                            {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);
                                    if (currentCard.rank == nextCard.rank + 1 && currentCard.suit == nextCard.suit)
                                    {
                                        differentCards.Add(nextCard);
                                    }
                                    else
                                    {
                                        if (currentCard.rank != nextCard.rank || currentCard.suit != nextCard.suit)
                                        {
                                            indexBreak++;
                                            differentCards.Clear();
                                        }
                                    }

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {
                                        if (currentCard.rank == nextCard.rank + 1 && currentCard.suit == nextCard.suit)
                                        {
                                            differentCards.Add(nextCard);
                                        }
                                        else
                                        {
                                            if (currentCard.rank != nextCard.rank || currentCard.suit != nextCard.suit)
                                            {
                                                indexBreak++;
                                                differentCards.Clear();
                                            }
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
                            HighCards[0] = differentCards[0].rank;
                            if (!Sim) Console.WriteLine("Straight flush...............");
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
                            if (!Sim) Console.WriteLine("Four Of A Kind...............");
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
                                    HighCards[1] = allCardsList[i].rank;
                                    pair = true;
                                    countCard = 0;
                                }
                            }
                            else
                            {
                                HighCards[0] = allCardsList[i].rank;
                                threeOfAKind = true;
                                allCardsList.RemoveAt(position);
                                countCard = 0;
                            }
                            

                        }
                        if (pair == true && threeOfAKind == true)
                        {
                            if (!Sim) Console.WriteLine("Full House...............");
                            return true;
                        }
                    }

                    break;
                case 4: //Flush
                    if (allCards.Length > 4)
                    {
                        int countCard = 0;
                        bool flush = false;
                        bool foundHighCards = false;
                        HighCards[0] = allCards[0].rank;
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].suit == allCards[j].suit)
                                {
                                    countCard++;
                                }
                                if (foundHighCards == false)
                                {
                                    if (HighCards[0] < allCards[j].rank)
                                    {
                                        HighCards[1] = HighCards[0];
                                        HighCards[0] = allCards[j].rank;
                                    }
                                    else
                                    {
                                        if (HighCards[1] < allCards[j].rank)
                                        {
                                            HighCards[2] = HighCards[1];
                                            HighCards[1] = allCards[j].rank;
                                        }
                                        else
                                        {
                                            if (HighCards[2] < allCards[j].rank)
                                            {
                                                HighCards[2] = allCards[j].rank;
                                            }
                                        }
                                    }
                                }
                            }
                            foundHighCards = true;
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

                            if (!Sim) Console.WriteLine("Flush...............");
                            return true;
                        }
                    }

                    break;
                case 5: // Straight
                    if (allCards.Length > 4)
                    {
                        List<Card> differentCards = new List<Card>();
                        int indexBreak = 0;                 //if we have less than 5 cards needed for a straight flsuh check if 
                        List<Card> descendedAllCards = allCards.OrderByDescending(Card => Card.rank).ToList();
                        for (int i = 0; i < descendedAllCards.Count - 1; i++)
                        {
                            Card currentCard = descendedAllCards[i];
                            Card nextCard = descendedAllCards[i + 1];
                            if (indexBreak < 4)
                            {
                                if (differentCards.Count == 0)
                                {
                                    differentCards.Add(currentCard);
                                    if (currentCard.rank == nextCard.rank + 1 || (currentCard.rank == Ranks.Ten && descendedAllCards[descendedAllCards.Count - 1].rank == Ranks.Ace && differentCards.Count == 4))
                                    {
                                        if (currentCard.rank == Ranks.Ten)
                                        {
                                            differentCards.Add(descendedAllCards[descendedAllCards.Count - 1]);
                                        }
                                        else
                                        {
                                            differentCards.Add(nextCard);
                                        }
                                    }
                                    else
                                    {
                                        if (currentCard.rank != nextCard.rank)
                                        {
                                            indexBreak++;
                                            differentCards.Clear();
                                        }
                                    }

                                }
                                else
                                {
                                    if (differentCards.Count < 5)
                                    {
                                        if (currentCard.rank == nextCard.rank + 1 || (currentCard.rank == Ranks.Ten && descendedAllCards[descendedAllCards.Count - 1].rank == Ranks.Ace && differentCards.Count == 4))
                                        {
                                            if (currentCard.rank == Ranks.Ten)
                                            {
                                                differentCards.Add(descendedAllCards[descendedAllCards.Count - 1]);
                                            }
                                            else
                                            {
                                                differentCards.Add(nextCard);
                                            }
                                        }
                                        else
                                        {
                                            if (currentCard.rank != nextCard.rank)
                                            {
                                                indexBreak++;
                                                differentCards.Clear();
                                            }
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
                            HighCards[0] = differentCards[0].rank;
                            if (!Sim) Console.WriteLine("Straight...............");
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
                                try
                                {
                                    HighCards[0] = allCards[i].rank;
                                }
                                catch (Exception e)
                                {
                                }
                                
                                threeOfAKind = true;
                            }
                            /*
                            if(HighCards[1] < allCards[i].rank && allCards[i].rank!= HighCards[0])
                            {
                                HighCards[1] = allCards[i].rank;
                            }*/
                        }
                        if (threeOfAKind == true)
                        {

                            if (!Sim) Console.WriteLine("Three Of A Kind...............");
                            return true;
                        }
                    }

                    break;
                case 7: // Two pair
                    if (allCards.Length >= 4)
                    {
                        List<Card> allCardsList = allCards.ToList();
                        int countPairs = 0;
                        for (int i = 0; i < allCardsList.Count-1; i++)
                        {
                            for (int j = i + 1; j < allCardsList.Count; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countPairs++;
                                    if(HighCards[0] == HighCards[1])
                                    {
                                        HighCards[0] = allCards[i].rank;
                                    }
                                    else
                                    {
                                        if (HighCards[0] < allCards[i].rank)
                                        {
                                            HighCards[1] = HighCards[0];
                                            HighCards[0] = allCards[j].rank;
                                        }
                                        else
                                        {
                                            if (HighCards[1] < allCards[j].rank)
                                            {
                                                HighCards[1] = allCards[j].rank;
                                            }
                                        }
                                    }
                                }
                                if (allCards[i].rank > HighCards[2] && HighCards[0] != allCards[i].rank && HighCards[1] != allCards[i].rank)
                                {
                                    HighCards[2] = allCards[j].rank;
                                }
                                
                            }
                            if(countPairs >= 2)
                            {
                                if (!Sim) Console.WriteLine("Two pair...............");
                                return true;
                            }

                        }
                        if (countPairs >= 2)
                        {
                            if (!Sim) Console.WriteLine("Two pair...............");
                            return true;
                        }
                    }

                    break;
                case 8: // Pair
                    if (allCards.Length >= 3)
                    {
                        List<Card> differentCards = new List<Card>();
                        int countCard = 0;               
                        for (int i = 0; i < 6; i++)
                        {
                            for (int j = i + 1; j < allCards.Length; j++)
                            {
                                if (allCards[i].rank == allCards[j].rank)
                                {
                                    countCard++;
                                    HighCards[0] = allCards[i].rank;
                                }
                            }

                        }
                        if (countCard >= 1)
                        {
                            if (!Sim) Console.WriteLine("Pair...............");
                            return true;
                        }
                    }

                    break;
            }
            return false;
        }
    }
  
}