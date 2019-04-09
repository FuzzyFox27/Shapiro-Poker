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
        private float score = 0;
        private float percievedScore = 0;

        private float potOdds;
        double[] value = new double[10];

        public bool reraise = false;
        float[] weights = new float[4];
        int raiseAmount = 5;

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
            //Console.WriteLine(score);

        }

        /*
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
        */
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

        public void WorkOutOddsForPot(Table table)
        {

            //EG Pot has 100, costs 10 to play. Pot Odds are 1:10 - must win once in every 10 to break even. This is where hand odds come in

            int ValueOfPot;
            int CostToPlay; 

            ValueOfPot = table.currentPot;
            CostToPlay = table.highestBet - currentBet;
            if (CostToPlay == 0) CostToPlay = 1;

            potOdds = ValueOfPot / CostToPlay;
        }

        /*public void WorkOutHandOuts(Table table)
        {
            int outs = 0;
            bool[] hands = new bool[13];
            if (table.presentOnTable.Count == 3)
            {
                if (this.hand[0].rank == hand[1].rank) //Pair to Set
                {
                    outs += 2;
                }
                if (this.hand.Max(i => i.rank) > table.presentOnTable.Max(i => i.rank)) //If player has a card higher than the table
                {
                    outs += 3;
                }
            }
            //Pocket Pair to Set
            //One overcard (one card in pocket higher than card on table)
            //Inside Straight Draw
            //Two Pair to Full House
            //One Pair to Two Pairs or Trips
            //No Pair to Pair
            //Two Overcards to Overpair
            //Set to Full House/4OAK
            //Open-Ended Straight Draw
            //Flush Draw
            //Inside Straight Draw and Two Overcards
            //Inside Straight and Flush Draw
            //Open-Ended Straight and Flush Draw
        }*/

        public void SimulateHands(Table table)
        {
            WorkOutOddsForPot(table);

            Deck simDeck = new Deck();
            List<Card> simHand = new List<Card>();
            Evaluate eval = new Evaluate();
            int[] Probs = new int[10];

            int iterate = 100000;

            for (int i = 0; i < iterate; i++)
            {

                for (int x = 0; x < hand.Count(); x++) //Removes pocket cards from simDeck
                {
                 simDeck.RemoveSpecificCard(hand[x]);
                 simHand.Add(hand[x]);
                }

                for (int x = 0; x < table.presentOnTable.Count(); x++) //Removes cards already on table from simDeck
                {
                 simDeck.RemoveSpecificCard(table.presentOnTable[x]);
                 simHand.Add(table.presentOnTable[x]);
                }
            
                while(simHand.Count < 7)
                {
                    simHand.Add(simDeck.SimulateWithdrawCard());
                }
                if (eval.CalculateGrade(simHand.ToArray(), 0) == true)
                {
                    Probs[0] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.StraightFlush) == true)
                {
                    Probs[1] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.FourOfAKind) == true)
                {
                    Probs[2] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.FullHouse) == true)
                {
                    Probs[3] += 1;
                    //Probs[6] += 1;
                    //Probs[8] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.Flush) == true)
                {
                    Probs[4] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.Straight) == true)
                {
                    Probs[5] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.ThreeOfAKind) == true)
                {
                    Probs[6] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.TwoPairs) == true)
                {
                    Probs[7] += 1;
                    //Probs[8] += 1;
                }
                if (eval.CalculateGrade(simHand.ToArray(), Evaluate.Grades.Pair) == true)
                {
                    Probs[8] += 1;
                }

                Probs[9] += 1;
                

                simHand.Clear();
            }

            for (int i = 0; i < 10; i++)
            {
                value[i] = (float)Probs[i] / (float)iterate;
                value[i] *= 100;
            }

            //DEBUG//            
            Console.WriteLine();
            Console.WriteLine("{0}% chance of getting a high card", value[9]);
            Console.WriteLine("{0}% chance of getting a pair", value[8]);
            Console.WriteLine("{0}% chance of getting two pairs", value[7]);
            Console.WriteLine("{0}% chance of getting three of a kind", value[6]);
            Console.WriteLine("{0}% chance of getting a straight", value[5]);
            Console.WriteLine("{0}% chance of getting a flush", value[4]);
            Console.WriteLine("{0}% chance of getting a full house", value[3]);
            Console.WriteLine("{0}% chance of getting four of a kind", value[2]);
            Console.WriteLine("{0}% chance of getting a straight flush", value[1]);
            Console.WriteLine("{0}% chance of getting a royal flush", value[0]);
            Console.ReadLine();
            for (int i = 0; i < 10; i++)
            {
                handConfidence += (10-i) * (float) value[i];
            }
            handConfidence /= 100;
            
        }

        public char Play()
        {
            char ans;

            int checking = 0;
            float tScore = score;
            bool stop = false;
            do
            {
                if (value[checking] > 60)
                {
                    stop = true;
                }
                else checking++;
            } while (!stop);
            checking = 9 - checking;
            tScore += checking * score;
            if (tScore >= 50)
            {
                ans = 'r';
            }
            else if (tScore > 0)
            {
                ans = 'c';
            }
            else ans = 'f';

            //If PotOdds > OddsToWin, Raise

            return ans;
        }

        public int GetRaiseAmount(int amountToCall)
        {
            float amountToRaise = amountToCall;
            amountToRaise += (float)handConfidence * (float) raiseAmount;
            
            return (int) amountToRaise;
        }

    }
}

