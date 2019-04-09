using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class AI : Player
    {
        private float handConfidence = 1;

        float score = 0;
        float percievedScore = 0;

        public bool reraise = false;
        float[] weights = new float[4];

        int VPIP = 0;
        List<int> PFRs = new List<int>();
        List<Tuple<int, int>> Tendencies = new List<Tuple<int, int>>();

        public AI(int ID, int startingChips)
        {
            playerID = ID;
            currentChips = startingChips;
        }

        public void ScorePreFlopHand()
        {
            int highestCard = 0;
            //Both Aces
            if (hand[0].rank == Ranks.Ace && hand[1].rank == Ranks.Ace)
            {
                score = 10;

            }
            //Only one ace
            else if (hand[0].rank == Ranks.Ace ^ hand[1].rank == Ranks.Ace)
            {
                score = 10;
                if (hand[1].rank == Ranks.Ace) highestCard = 1;
            }
            else
            {
                if (hand[1].rank >= hand[0].rank)
                {
                    highestCard = 1;
                }
                switch ((int)hand[highestCard].rank)
                {
                    case 0:
                        score = 10;
                        break;
                    case 13:
                        score = 8;
                        break;
                    case 12:
                        score = 7;
                        break;
                    case 11:
                        score = 6;
                        break;
                    default:
                        score = (float)hand[highestCard].rank / 2f;
                        break;
                }
            }
            
            
            if (hand[0].rank == hand[1].rank) score *= 2; //Double if pair
            if (hand[0].suit == hand[1].suit) score += 2; //Add 2 if suited
            int difference;
            int otherHand = -1;
            switch (highestCard)
            {
                case 0:
                    otherHand = 1;
                    break;
                case 1:
                    otherHand = 0;
                    break;
            }

            if (hand[highestCard].rank != Ranks.Ace) //Ace is high
            {
                difference = (int)hand[highestCard].rank - (int)hand[otherHand].rank; //If highest isn't an ace
            }
            else if (hand[otherHand].rank == Ranks.Ace) //If Highest is an ace, and second card is an ace.
            {
                difference = 0;
            } else difference = 13 - (int)hand[otherHand].rank; //Else, card subtract 14 - value of ace.
            switch (difference)
            {
                case 0:
                    break;
                case 1:
                    score -= 1;
                    break;
                case 2:
                    score -= 2;
                    break;
                case 3:
                    score -= 4;
                    break;
                default:
                    score -= 5;
                    break;
            }

            if (difference == 1)
            {
                if (hand[0].rank < Ranks.Queen && hand[1].rank < Ranks.Queen)
                {
                    score += 1;
                }
            }

            score = (int) Math.Ceiling(score); //Got final valuation of pre-flop hand.
            Console.WriteLine(score);
            Play(score);

        }

        public void Play(float score)
        {
            if (!reraise)
            {
                if (score >= 9)
                {
                    Console.WriteLine("I need to Raise");//Raise
                }
                else
                {
                    Console.WriteLine("I need to Fold");//Fold
                }
            }
            else
            {
                if (score >= 12)
                {
                    Console.WriteLine("I need to Raise");//Raise
                }
                else if (score >= 10)
                {
                    Console.WriteLine("I need to Check");//Check
                }
                else
                {
                    Console.WriteLine("I need to Fold");//Fold
                }
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void AddTendency(int aVPIP, int PFR)
        {
            PFRs.Add(PFR);
            if (aVPIP < VPIP) VPIP += aVPIP;
            else VPIP = aVPIP;
        }
        
        public void CompileTendencies()
        {
            if (VPIP > 0) Tendencies.Add(Tuple.Create(VPIP, PFRs.Sum()));
        }

        public void ExaminePlayerType(List<Player> players, int gamesPlayed)
        {
            if (Tendencies.Count > 0)
            {
                float winRate = players[1].gamesWon / gamesPlayed;
                float passive = 0f;
                float aggressive = 0f;

                //Passive 
                int SumOfVPIP = 0;
                int SumOfPFR = 0;
                foreach (var t in Tendencies)
                {
                    SumOfVPIP += t.Item1;
                    SumOfPFR += t.Item2;
                }
                float VPIPAverage = SumOfVPIP / Tendencies.Count;
                float PFRAverage = SumOfPFR / Tendencies.Count;
                passive = 1f - ((VPIPAverage - PFRAverage) / 100f);

                //Aggressive
                aggressive = PFRAverage / 100f;

                //Tight
                if (VPIPAverage < 50 && PFRAverage < (VPIPAverage / 2f))
                {
                    weights[0] += 0.03f;
                }

                //Bluffer
                if (aggressive > 0.5f && winRate < 0.5f)
                {
                    weights[0] -= 0.03f;
                }

                //Normal
                if (passive < 0.5f)
                {
                    weights[0] += 0.03f;
                }
            }
            
        }

        public void ExaminePlayerMove(int playerRaisedAmount, List<Player> players)
        {
            float weightedPlayerRaisedAmount = playerRaisedAmount * weights[0];
            float confidence = weightedPlayerRaisedAmount / players[1].currentChips;

            if (confidence > weights[1])
            {
                handConfidence += 0.03f;
            }
            else handConfidence -= 0.03f;
            percievedScore = score * handConfidence;
        }
    }
}

