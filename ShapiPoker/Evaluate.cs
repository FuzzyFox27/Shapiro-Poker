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

                if (CheckRoyalFlush(playerHand, onTable))
                    players[i].grade = Grades.RoyalFlush;
                else if (CheckStraightFlush(playerHand, onTable))
                    players[i].grade = Grades.StraightFlush;
                else if (CheckFourOfAKind(playerHand, onTable))
                    players[i].grade = Grades.FourOfAKind;
                else if (CheckFullHouse(playerHand, onTable))
                    players[i].grade = Grades.FullHouse;
                else if (CheckFlush(playerHand, onTable))
                    players[i].grade = Grades.Flush;
                else if (CheckStraight(playerHand, onTable))
                    players[i].grade = Grades.Straight;
                else if (CheckThreeOfAKind(playerHand, onTable))
                    players[i].grade = Grades.ThreeOfAKind;
                else if (CheckTwoPair(playerHand, onTable))
                    players[i].grade = Grades.TwoPairs;
                else if (CheckPair(playerHand, onTable))
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

        bool CheckRoyalFlush(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckStraightFlush(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckFourOfAKind(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckFullHouse(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckFlush(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckStraight(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckThreeOfAKind(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckTwoPair(Card[] playerHand, Card[] tableCards)
        {
            return false;
        }

        bool CheckPair(Card[] playerHand, Card[] tableCards)
        {
            return false;
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
    }

    public enum Grades
    {
        RoyalFlush, StraightFlush, FourOfAKind, FullHouse, Flush, Straight, ThreeOfAKind, TwoPairs, Pair, HighCard
    }
}
