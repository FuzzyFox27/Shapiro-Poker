using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ShapiroPoker;

namespace Poker_AI_Game
{
    class MainProgram
    {
        //Class References
        static Deck deck = new Deck();
        public static List<Player> players = new List<Player>();
        static Table table = new Table();

        //Game Vars
        static bool gameActive = true;
        static int gamePhase = 0;

        static int winnerID = -1;

        //Main code chain
        static void Main(string[] args)
        {
            PopulatePlayers();

            while (gameActive)
            {
                DecipherPhase();
            }
        }

        //This is where all players and AI need to be instantiated
        static void PopulatePlayers()
        {
            //Add custom amount of players
            int NoPlayers;
            bool validNoPlayers = false;
            do
            {
                Console.Write("How many players: ");
                string input = Console.ReadLine();
                validNoPlayers = int.TryParse(input, out NoPlayers);
                if (NoPlayers <= 1) validNoPlayers = false;
            } while (!validNoPlayers);
            //Will create AI first, as player 1
            players.Add(new Player(1, 100));
            //Temp Adding player 2+
            for (int i = 2; i <= NoPlayers; i++)
            {
                players.Add(new Player(i, 100));
            }
        }
        
        //Actions are taken depending on phase of game
        static void DecipherPhase()
        {
            switch (gamePhase)
            {
                //Start of the game, players bet then flip 3
                case 0:
                    DealCardsToAllPlayers();
                    UserAction();
                    DealToTable(3);
                    ResetHighestBets();
                    break;
                //3 currently on table, players bet then flip 1
                case 1:
                    UserAction();
                    DealToTable(1);
                    ResetHighestBets();
                    break;
                //4 currently on table, players bet then flip 1
                case 2:
                    UserAction();
                    DealToTable(1);
                    ResetHighestBets();
                    break;
                //5 currently on table, players bet then the winner is decided
                case 3:
                    UserAction();
                    ResetHighestBets();
                    CalculateWinner();
                    break;
                //Winner decided for round, full reset cards
                case 4:
                    EndRound();
                    break;
            }

            //If gamePhase >= 4 then reset to 0, else add 1 to gamePhase
            gamePhase = (gamePhase >= 4) ? 0 : gamePhase + 1;
        }

        //Deals cards to all players, regardless of how many
        static void DealCardsToAllPlayers()
        {
            foreach (Player player in players)
            {
                player.ReceiveHand(deck.GetCards(2));
                
            }
        }

        //Deals cards to the table
        static void DealToTable(int amountToPlace)
        {
            Card[] tempCards = deck.GetCards(amountToPlace);
            table.AddCardsToTable(tempCards);
        }

        //Reset the table and players highest bets for next phase
        static void ResetHighestBets()
        {
            table.highestBet = 0;
            foreach(Player player in players)
            {
                player.currentBet = 0;
            }
        }

        //this int is to ensure that the first time through the code runs
        public int firsttime = 0;
        //Take bets from all players
        static void UserAction()
        {
            //this int is to ensure that the first time through the code runs
            int firsttime = 0;
            while (table.highestBet != players.ElementAt(0).currentBet && table.highestBet != players.ElementAt(1).currentBet || firsttime == 0)
            {
                if (firsttime == 0) { firsttime++; }
                for (int i = 0; i < players.Count; i++) // <-- Change to a while, have a global roundOver, change to true when all players bet the same
                {
                    if (!OnlyPlayer()) //Check the player isnt the only one left playing
                    {
                        if (players[i].inRound && !players[i].allIn) //Check the player is still in the round and isnt all in
                        {
                            CalculateOptions(players[i]);
                        }
                    }
                }

            }
        }

        static void CalculateOptions(Player player)
        {
            bool fold = false, check = false, call = false, raise = false;

            //Check if player is all in
            if (player.currentChips > 0)
            {
                fold = true;

                //Check if player can check
                if (player.currentBet == table.highestBet)
                {
                    check = true;
                }
                else
                {
                    call = true;
                }

                if (player.currentChips > (table.highestBet - player.currentBet) + 1) //Check if player can afford to raise current bet
                {
                    raise = true;
                }
            }

            player.SetPossibilities(new bool[4] { fold, check, call, raise });

            ShowOptions(player);
        }

        //Show Potential Options of -- Check, Call, Raise, Fold
        static void ShowOptions(Player player)
        {
            WipeWithInfo(player);
            Console.WriteLine("The pot has {0} chips.", table.currentPot);
            //
            HandCalculate(player.GetPlayerID()-1);
            //

            string options = "Player can:"; //Create string of options available

            if (player.possibleActions[0])
                options += " Fold (F) ";
            if (player.possibleActions[1])
                options += " Check (C) ";
            if (player.possibleActions[2])
                options += " Call (C)(" + (table.highestBet - player.currentBet).ToString() + ")"; //Show how many chips it is to call
            if (player.possibleActions[3])
                options += " Raise (R) ";

            Console.WriteLine(options);
                TakeActionInput(player);
        }

        //Take the users choice
        static void TakeActionInput(Player player)
        {
            bool result = true;
            char choice;
            if (player.GetPlayerID() != 1)
            {
                string input = Console.ReadLine().ToLower();
                result = Char.TryParse(input, out choice);
            }
            else
            //AI Makes it's choice if it is it's turn
            {
                AI.GetLastBet(table.highestBet);
                choice = AI.MakeChoice();
            }
            if (!result || choice != 'f' && choice != 'c' && choice != 'r') //Check if cannot parse, or if not equal to a choice
            {
                Console.WriteLine("Unrecognised input, please try again...");
                TakeActionInput(player);
            }
            else
            {
                //Fold
                if (choice == 'f')
                {
                    player.inRound = false;
                }

                //Check or Call
                if (choice == 'c')
                {
                    //Look if Check is false
                    if (player.possibleActions[1] == false)
                    {
                        //Call
                        int amountToCall = table.highestBet - player.currentBet;

                        if (amountToCall > player.currentChips)
                        {
                            player.allIn = true;
                        }
                        player.Bet(amountToCall);
                        table.currentPot += amountToCall;
                    }
                    else
                    {
                        //Check
                    }
                }

                //Raise
                if (choice == 'r')
                {
                    if (player.possibleActions[3])
                    {
                        TakeRaiseAmount(player);
                    }
                }
            }
        }

        //Player raises
        static void TakeRaiseAmount(Player player)
        {
            string input = "0";
            Console.Write("Raise By: ");
            if (player.GetPlayerID() != 1)
            {
                input = Console.ReadLine();
            }
            else
            {
                input = AI.ChooseRaiseAmount();
            }

            if (int.TryParse(input, out int amountToRaise)) //Check input number
            {
                if (amountToRaise > table.highestBet) //Check if input is more than previous bet
                {
                    if (amountToRaise > 0 && amountToRaise <= player.currentChips) //Check if player is raising more than 0 and has enough to raise
                    {
                        player.Bet(amountToRaise);
                        table.currentPot += amountToRaise;
                        table.highestBet = player.currentBet;
                    }
                    else
                    {
                        Console.WriteLine("Not enough chips");
                        TakeRaiseAmount(player);
                    }
                }
                else
                {
                    Console.WriteLine("Not higher than previous bet");
                    TakeRaiseAmount(player);
                }
            }
            else
            {
                Console.WriteLine("Input Not Recognised...");
                TakeRaiseAmount(player);
            }
        }

        //Reset the round for the next
        static void EndRound()
        {
            table.RefreshTable();
            deck.RefreshDeck();
            foreach (Player player in players)
                player.RefreshPlayer();
            winnerID = -1;

            RemoveLosers();
        }

        //Calculate the winning hand -- potentially pot split etc
        static void CalculateWinner()
        {
            if (winnerID != -1)
            {
                //All others have folded, one winner remains
                DeclareWinner(winnerID);
                /*
                Console.WriteLine("Player " + winnerID + " wins.");
                players[(winnerID-1)].currentChips += table.currentPot;
                Console.ReadLine();
                */
            }
            else
            {
                //Figure out winner depending on cards
                //Smallest type number wins, if two same, then highest hand total wins. Aces high, suits dont matter
                //EG 1 (RF) wins 2-10. If two 10s, take sum of card ranks. If tie, split pot n-ways.
                List<Tuple<int, int, int[]>> playerData = new List<Tuple<int, int, int[]>>();
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].inRound)
                    {
                        Tuple<int,int[]> cardData = HandCalculate(i);
                        playerData.Add(Tuple.Create(cardData.Item1, i, cardData.Item2)); //HandType, Player ID, AND HandLocation
                    }
                }

                playerData = playerData.OrderBy(i => i.Item1).ToList();
                if (playerData[0].Item1 < playerData[1].Item1) //There is a winner by higher value hand
                {
                    winnerID = playerData[0].Item2 +1;
                    DeclareWinner(winnerID);
                }
                else
                {
                    List<Tuple<int,int>> playerScores = new List<Tuple<int, int>>(); //Translating data
                    foreach (var p in playerData)
                    {
                        playerScores.Add(Tuple.Create(p.Item1,p.Item2));
                    }
                    playerScores = playerScores.OrderBy(i => i.Item1).ToList();

                    List<int> TyingPlayers = FindTyingPlayers(playerScores);
                    playerScores.Clear();
                    foreach (var p in TyingPlayers)
                    {
                        playerScores.Add(Tuple.Create(p, playerData[p].Item1));
                    }
                    //GETS TYING PLAYERS
                    //COMPARES TYING PLAYERS BASED ON HIGH CARD

                    List<Tuple<int, int>> tPlayerScores = new List<Tuple<int, int>>();
                    foreach (var p in playerScores)
                    {
                        tPlayerScores.Add(p);
                    }
                    playerScores.Clear();

                    foreach (var p in tPlayerScores)
                    {
                        int score = HandScoreReturner(p.Item1,playerData[p.Item1].Item1,playerData[p.Item1].Item3);
                        playerScores.Add(Tuple.Create(score,p.Item1));
                    }
                    /*
                    for (int i = 0; i <= c; i++)
                    {
                        int score = HandScoreReturner(i, playerData[i].Item1, playerData[i].Item3);
                        playerScores.Add(Tuple.Create(score,i));
                    }
                    */
                    playerScores = playerScores.OrderByDescending(i => i.Item1).ToList();
                    if (playerScores[0].Item1 > playerScores[1].Item1) //Is there a winner?
                    {
                        winnerID = playerScores[0].Item2+1;
                        DeclareWinner(winnerID);
                    }
                    else //TIE
                    {

                        //FIND WINNER BY HIGH CARD
                        Console.WriteLine("Tie.");

                        TyingPlayers = FindTyingPlayers(playerScores);
                        playerScores.Clear();
                        foreach (var p in TyingPlayers)
                        {
                            int score = HandScoreReturner(p, 10, playerData[p].Item3); //Tests the current tied players for high card winner
                            playerScores.Add(Tuple.Create(score, p));
                        }

                        playerScores = playerScores.OrderByDescending(i => i.Item1).ToList();
                        if (playerScores[0].Item1 > playerScores[1].Item1) //If there's a winner given hand and high card
                        {
                            winnerID = playerScores[0].Item2 + 1;
                            DeclareWinner(winnerID);
                        }
                        else //Failed to find a winner by high card. Splitting pot between winners
                        {
                            TyingPlayers = FindTyingPlayers(playerScores);
                            SplitPot(TyingPlayers.ToArray());
                        }


                        
                    }
                }

                
            }
        }

        static List<int> FindTyingPlayers(List<Tuple<int, int>> playerScores)
        {
            List<int> tyingPlayers = new List<int>();
            bool tie = true;
            int i = 0;
            tyingPlayers.Add(playerScores[0].Item2);
            do
            {
                if (playerScores[i].Item1 == playerScores[i + 1].Item1)
                {
                    tyingPlayers.Add(playerScores[i+1].Item2);
                    i++;
                    if (i == playerScores.Count - 1) tie = false;
                }
                else tie = false;
            } while (tie);
            return tyingPlayers;
        }

        static void DeclareWinner(int winnerID)
        {
            Console.WriteLine("Player " + winnerID + " wins.");
            players[(winnerID-1)].currentChips += table.currentPot;
            Console.ReadLine();
        }

        static void SplitPot(int[] winnerIDs)
        {
            if (table.currentPot % winnerIDs.Length == 0) //If pot can be evenly split
            {
                Console.WriteLine("Splitting {0} chips {1} ways",table.currentPot, winnerIDs.Length);
                foreach (var p in winnerIDs)
                {
                    players[winnerID].currentChips += table.currentPot / winnerIDs.Length;
                }
            }
            else
            {
                int remainder = table.currentPot % winnerIDs.Length;
                Random r = new Random();
                int extra = r.Next(winnerIDs.Length);
                Console.WriteLine("{0} extra chips randomly assigned to player {1}",remainder, extra+1);
                table.currentPot -= remainder;
                foreach (var p in winnerIDs)
                {
                    players[p].currentChips += table.currentPot / winnerIDs.Length;
                }

                players[extra].currentChips += remainder;
            }
        }

        static Tuple<int,int[]> HandCalculate(int player)
        {
          //HighCard
            //Check player hand
            Card highest = new Card();
            highest = players[player].GetCardInHand(0);
            if (highest.rank < players[player].GetCardInHand(1).rank) highest = players[player].GetCardInHand(1);
            //Checks both cards in hand to find the highest.
            //Check table
            for (int i = 0; i < table.GetNoCardsOnTable(); i++)
            {
                if (highest.rank < table.GetCardInPosition(i).rank) highest = table.GetCardInPosition(i);
            }

          //Pair checker
            bool HasPair = false;
            bool HasTwoPair = false;
            bool HasThreeOfAKind = false;
            bool HasStraight = false;
            bool HasFlush = false;
            bool HasFullHouse = false;
            bool HasFourOfAKind = false;
            bool HasStraightFlush = false;
            bool HasRoyalFlush = false;
            
            List<Card> ComparisonDeck = new List<Card>();
            for (int i = 0; i < 2; i++) ComparisonDeck.Add(players[player].GetCardInHand(i));
            for (int i = 0; i < table.GetNoCardsOnTable(); i++)
            {
                ComparisonDeck.Add(table.GetCardInPosition(i));
            }
            //SORTING
            //ComparisonDeck.;
            //
            ComparisonDeck = ComparisonDeck.OrderBy(c => (int) c.rank).ToList();

            bool Count = false;
            int runningCount = 1;
            Card runningCard = new Card();
            runningCard = ComparisonDeck[0];

            int pairAt = -1, twoPairAt = -1, threeAt = -1, fourAt = -1;


            //Checking for Pair combinations
            for (int i = 1; i < ComparisonDeck.Count; i++)
            {
                if (ComparisonDeck[i].rank == runningCard.rank)
                {
                    runningCount++;
                    Count = true;
                }
                if (!Count)
                {
                    runningCount = 1;
                    runningCard = ComparisonDeck[i];
                }
                else Count = false;

                if (runningCount == 2 && HasTwoPair == true)
                {
                    HasTwoPair = true;
                    pairAt = twoPairAt;
                    twoPairAt = i; //Used to calculate final hand. To include, include hand at i and i-1
                }
                else if (runningCount == 2 && HasPair == true)
                {
                    HasTwoPair = true;
                    twoPairAt = i; //Used to calculate final hand. To include, include hand at i and i-1
                }
                else if (runningCount == 2 && HasPair == false)
                {
                    HasPair = true;
                    pairAt = i; //i and i-1
                }
                else if (runningCount == 3)
                {
                    HasThreeOfAKind = true;
                    HasPair = false;
                    threeAt = i; //i and i-1 and i-2
                }
                else if (runningCount == 4)
                {
                    HasFourOfAKind = true;
                    HasThreeOfAKind = false;
                    fourAt = i; //i and i-1 and i-2 and i-3
                }
            }
            //Checking for Straight
            
            int straightCount = 1;
            int flushAt = -1, straightAt = -1;
            if (ComparisonDeck.Count >= 5)
            {
                runningCard = ComparisonDeck[0];
                for (int i = 1; i < ComparisonDeck.Count; i++)
                {
                
                    if (ComparisonDeck[i].rank == runningCard.rank + 1) straightCount++;
                    else straightCount = 1;

                    runningCard = ComparisonDeck[i];


                    if (straightCount > 5)
                    {
                        HasStraight = true;
                        straightAt = i;
                    }
                    
                    //USE THE INT TO WORK OUT THE 5 CARDS IN THE FINAL HAND//
                }

                int Clubs = 0, Diamonds = 0, Hearts = 0, Spades = 0;
                for (int i = 0; i < ComparisonDeck.Count; i++)
                {
                    if (ComparisonDeck[i].suit == Suits.Clubs) Clubs++;
                    else if (ComparisonDeck[i].suit == Suits.Diamonds) Diamonds++;
                    else if (ComparisonDeck[i].suit == Suits.Hearts) Hearts++;
                    else if (ComparisonDeck[i].suit == Suits.Spades) Spades++;

                    if (Clubs >= 5 || Diamonds >= 5 || Hearts >= 5 || Spades >= 5)
                    {
                        HasFlush = true;
                        flushAt = i;
                    }
                }
                

            }

            //Logical Equivalence - Hand by definition
            if (HasPair && HasThreeOfAKind) HasFullHouse = true;
            if (HasFlush && HasStraight) HasStraightFlush = true;
            if (HasStraightFlush)
            {
                if (ComparisonDeck[flushAt].rank == Ranks.Ace) HasRoyalFlush = true;
            }

            int[] handLocation = new int[2];
            int handType = 0;
            if (HasRoyalFlush) {
                Console.WriteLine("Royal Flush"); //Added
                handType = 1;
                handLocation[0] = flushAt;
            }
            else if (HasStraightFlush) {
                Console.WriteLine("Straight Flush"); //Added by logic - Given both Staight and Flush work, this should also work.
                handType = 2;
                handLocation[0] = straightAt;
            }
            else if (HasFourOfAKind) {
                Console.WriteLine("Four of a Kind: {0}", ComparisonDeck[fourAt].rank); //Added
                handType = 3;
                handLocation[0] = fourAt;
            }
            else if (HasFullHouse) {
                Console.WriteLine("Full House"); //Added by Logic +TESTED
                handType = 4;
                handLocation[0] = pairAt;
                handLocation[1] = threeAt;
            }
            else if (HasFlush) {
                Console.WriteLine("Flush"); //Added + TESTED
                handType = 5;
                handLocation[0] = flushAt;
            }
            else if (HasStraight) {
                Console.WriteLine("Straight"); //Added + TESTED
                handType = 6;
                handLocation[0] = straightAt;
            }
            else if (HasThreeOfAKind) {
                Console.WriteLine("Three of a Kind: {0}", ComparisonDeck[threeAt].rank); // Added + TESTED
                handType = 7;
                handLocation[0] = threeAt;
            }
            else if (HasTwoPair) {
                Console.WriteLine("Two Pairs of {0}s and {1}s", ComparisonDeck[pairAt].rank, ComparisonDeck[twoPairAt].rank); //Added + TESTED
                handType = 8;
                handLocation[0] = pairAt;
                handLocation[1] = twoPairAt;
            }
            else if (HasPair) {
                Console.WriteLine("Pair of {0}s",ComparisonDeck[pairAt].rank); //Added + TESTED
                handType = 9;
                handLocation[0] = pairAt;
            }
            else{
                Console.WriteLine("High card: {0} of {1}",highest.rank, highest.suit); //Added + TESTES
                handType = 10;
                handLocation[0] = 7;
            }

            return Tuple.Create(handType,handLocation);
        }

        static int HandScoreReturner(int player, int handType, int[] handLocation)
        {
            //HAND LOCATION IS ASCENDING 


            List<Card> ComparisonDeck = new 
                List<Card>();
            //List<Card> FinalDeck = new List<Card>();
            for (int i = 0; i < 2; i++) ComparisonDeck.Add(players[player].GetCardInHand(i));
            for (int i = 0; i < table.GetNoCardsOnTable(); i++)
            {
                ComparisonDeck.Add(table.GetCardInPosition(i));
            }
            ComparisonDeck = ComparisonDeck.OrderBy(c => (int)c.rank).ToList();

            int score = 0;
            switch (handType)
            {
                case 1: //Royal Flush
                    //A tied royal flush is always split


                    break;
                case 2: //Straight Flush
                    score += (int)ComparisonDeck[handLocation[0]].rank;
                    break;
                case 3: //Four of a Kind
                    score += (int)ComparisonDeck[handLocation[0]].rank;
                    break;
                case 4: //Full House
                    score += (int)ComparisonDeck[handLocation[0]].rank;
                    score += (int)ComparisonDeck[handLocation[1]].rank;
                    break;
                case 5: //Flush

                    break;
                case 6: //Straight
                    score += (int)ComparisonDeck[handLocation[0]].rank;
                    break;
                case 7: //Three of a Kind
                    score += (int)ComparisonDeck[handLocation[0]].rank;
                    break;
                case 8: //Two Pair
                    score += (int)ComparisonDeck[handLocation[0]-1].rank;
                    score += (int)ComparisonDeck[handLocation[1]].rank;
                    break;
                case 9: //Pair
                    score += (int)ComparisonDeck[handLocation[0]].rank;

                    break;
                case 10: //High Card
                    ComparisonDeck = ComparisonDeck.OrderByDescending(c => (int)c.rank).ToList();
                    score += (int)ComparisonDeck[0].rank;


                    break;
                default: break;
            }


            return score;
        }


        //Check the player isn't the only one with a hand left
        static bool OnlyPlayer()
        {
            uint amount = 0;
            Player tempPlayer = new Player();

            foreach(Player player in players) //Check all players for if they havent folded
            {
                if (player.inRound)
                {
                    tempPlayer = player;
                    amount++;
                }
            }

            if (amount > 1)
            {
                return false;
            }
            else
            {
                winnerID = (int)tempPlayer.playerID; //All but one have folded, set as winner
                return true;
            }
        }

        //Remove players that finished the round with 0 chips
        static void RemoveLosers()
        {
            List<int> positionOfLosers = new List<int>();

            for(int i = 0; i < players.Count; i++)
            {
                if (players[i].currentChips <= 0) //Check if player has more than 0 chips
                {
                    positionOfLosers.Add(i);
                }
            }

            foreach (int position in positionOfLosers)
            {
                players.RemoveAt(position); //Remove all players with 0 chips
            }
        }

        //Clear console and display current player hand and table info
        static void WipeWithInfo(Player player)
        {
            Console.Clear();
            Console.WriteLine("Player: " + player.playerID + " to play...");
            table.PrintTable();
            player.PrintHand();
            Console.WriteLine("Player has " + player.currentChips + " chips.");
        }
    }
}
