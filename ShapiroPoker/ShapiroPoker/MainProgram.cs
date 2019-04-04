using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class MainProgram
    {
        //Class References
        static Deck deck = new Deck();
        static List<Player> players = new List<Player>();
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

            //Temp Adding player 1 and 2
            for (int i = 1; i <= NoPlayers; i++)
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

        //Take bets from all players
        static void UserAction()
        {
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
            Console.WriteLine(" The pot has {0} chips.", table.currentPot);
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
            char choice;
            string input = Console.ReadLine().ToLower();
            bool result = Char.TryParse(input, out choice);

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
            Console.Write("Raise By: ");
            string input = Console.ReadLine();

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
                Console.WriteLine("Player " + winnerID + " wins.");
                players[(winnerID-1)].currentChips += table.currentPot;
                Console.ReadLine();
            }
            else
            {
                //Figure out winner depending on cards
                
            }
        }

        static void HandCalculate()
        {
            //Royal Flush
            /*if ()
            {

            }*/
            //Straight Flush
            //Four of a Kind
            //Full House
            //Flush
            //Straight
            //Three of a kind
            //Two Pair
            //Pair
            //High Card
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
