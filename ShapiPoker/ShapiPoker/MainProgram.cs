//Alec Taylor, 04/04//2019, Last Edit
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
        static Evaluate evaluate = new Evaluate();
        static AI Ai;

        //Game Vars
        static bool gameActive = true;
        static int gamePhase = 0;
        static int button = 0;
        static int gamesPlayed = 0;
        static bool NoRaises = false;

        //Main code chain
        static void Main(string[] args)
        {
            players.Clear();
            MainMenu();
            try
            {
                int ans = Convert.ToInt32(Console.ReadLine());
                if (ans == 1)
                {
                    PopulatePlayers();

                    while (gameActive)
                    {
                        DecipherPhase();
                    }
                }
                else if (ans == 2)
                {
                    PopulateAIGame();
                    while (gameActive)
                    {
                        DecipherPhase();
                    }
                    //Call AI Functions, Intergrate AI here, And normal game functions 
                    //Unsire how to incorporate our neural network here, help appreciated 
                }
                else if (ans == 3)
                {
                    Environment.Exit(1);
                }
            }
            catch(FormatException)
            {
                Main(null);
                Console.WriteLine("Please Enter 1, 2 or 3....");
            }
        }

        //This is where all players and AI need to be instantiated
        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("\tWelcome To Shapiro's Poker\n\t_____________________");
            Console.WriteLine("\t1: P V P\n\t2: P V AI\n\t3: QUIT");
        }

        static void PopulateAIGame()
        {
            Ai = new AI(1, 100);
            players.Add(Ai);
            players.Add(new Player(2, 100));
        }

        static void PopulatePlayers()
        {
            //Temp Adding player 1 and 2
            players.Add(new Player(1, 100));
            players.Add(new Player(2, 100));
            players.Add(new Player(3, 100));
            players.Add(new Player(4, 100));
        }
        
        //Actions are taken depending on phase of game
        static void DecipherPhase()
        {
            switch (gamePhase)
            {
                //Start of the game, players bet then flip 3
                case 0:
                    BetBlinds();
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
                    if (players[0] is AI)
                    {
                        gamesPlayed++;
                        Ai.CompileTendencies();
                        Ai.ExaminePlayerType(players, gamesPlayed);
                        players[0] = Ai;
                    }
                    break;
                //Winner decided for round, full reset cards
                case 4:

                    //Cheat for Testing Start
                    /*
                    List<Card> tempTable = new List<Card>();
                    tempTable.Add(new Card(Suits.Hearts, Ranks.Ten));
                    tempTable.Add(new Card(Suits.Hearts, Ranks.Jack));
                    tempTable.Add(new Card(Suits.Spades, Ranks.Nine));
                    tempTable.Add(new Card(Suits.Hearts, Ranks.King));
                    tempTable.Add(new Card(Suits.Spades, Ranks.Seven));
                    table.presentOnTable = tempTable;

                    List<Card> tempHand = new List<Card>();
                    tempHand.Add(new Card(Suits.Spades, Ranks.Nine));
                    tempHand.Add(new Card(Suits.Hearts, Ranks.Seven));
                    players[0].hand = tempHand.ToArray();
                    //Cheat for Testing End
                    */

                    CalculateWinner();
                    Console.ReadLine();
                    EndRound();
                    break;
            }

            //If gamePhase >= 4 then reset to 0, else add 1 to gamePhase
            gamePhase = (gamePhase >= 4) ? 0 : gamePhase + 1;
        }

        static void CycleButton()
        {
            // Button acts cyclic
            if (button >= players.Count - 1)
            {
                button = 0;
            }
        }

        static void BetBlinds()
        {
            CycleButton();

            int blind = 1;

            // SmallBlind has enough money to bet blind
            if (players[button].currentChips > blind)
            {
                players[button].Bet(blind);
                players[button].setBlind(blind);
                Console.WriteLine("Player {0} has bet the small blind of {1}", button + 1, blind);
                table.currentPot += blind;
                table.highestBet = blind;
            }
            else // If not, player goes all in.
            {
                players[button].Bet(players[button].currentChips);
                players[button].setBlind(players[button].currentChips);
                players[button].allIn = true;
                Console.WriteLine("Player {0} has bet the small blind of {1}. They are All In", button + 1, players[button].currentBet);
                table.currentPot += players[button].currentBet;
                table.highestBet = players[button].currentBet;
            }
            //Big Blind has enough money to bet blind
            if (players[button + 1].currentChips > 2 * blind)
            {
                players[button + 1].Bet(2 * blind);
                players[button + 1].setBlind(2*blind);
                Console.WriteLine("Player {0} has bet the big blind of {1}", button + 2, 2*blind);
                table.currentPot += 2 * blind;
                table.highestBet = 2 * blind;
            }
            else // If not, player goes all in
            {
                players[button + 1].Bet(players[button + 1].currentChips);
                players[button + 1].setBlind(players[button + 1].currentChips);
                players[button + 1].allIn = true;
                Console.WriteLine("Player {0} has bet the small blind of {1}. They are All In", button + 2, players[button + 1].currentBet);
                table.currentPot += players[button + 1].currentBet;
                table.highestBet = players[button + 1].currentBet;
            }
            button++;
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
            
            int tButton = button+1;
            do
            {
                NoRaises = false;
                for (int i = 0; i < players.Count; i++)
                {
                    //Play from button
                    if (tButton == players.Count) tButton = 0;
                    if (!OnlyPlayer()) //Check the player isnt the only one left playing

                    {
                        if (players[tButton].inRound && !players[tButton].allIn) //Check the player is still in the round and isnt all in
                        {
                            if (players[tButton] is AI)
                            {
                                Console.WriteLine("AI");
                                Ai.ScorePreFlopHand();
                                players[0] = Ai;
                            }
                            CalculateOptions(players[tButton]);
                            tButton++;
                        }
                    }
                }
                
            } while (NoRaises);

            //Play from Button
            

        }

        static void CalculateOptions(Player player)
        {
            bool fold = false, check = false, call = false, raise = false, quit = false;

            //Check if player is all in
            if (player.currentChips > 0)
            {
                fold = true;
                call = true;

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
                if(gameActive)
                {
                    quit = true;
                }
            }

            player.SetPossibilities(new bool[5] { fold, check, call, raise, quit});

            ShowOptions(player);
        }

        //Show Potential Options of -- Check, Call, Raise, Fold
        static void ShowOptions(Player player)
        {
            WipeWithInfo(player);

            string options = "Player can:"; //Create string of options available

            if (player.possibleActions[0])
                options += " Fold (F) ";
            if (player.possibleActions[1])
                options += " Check (C) ";
            if (player.possibleActions[2])
                options += " Call (C)(" + (table.highestBet - player.currentBet).ToString() + ")"; //Show how many chips it is to call
            if (player.possibleActions[3])
                options += " Raise (R) ";
            if (player.possibleActions[4])
                options += "Quit (Q)";

            Console.WriteLine(options);

            TakeActionInput(player);
        }

        //Take the users choice
        static void TakeActionInput(Player player)
        {
            char choice;
            string input = Console.ReadLine().ToLower();
            bool result = Char.TryParse(input, out choice);

            if (!result || choice != 'f' && choice != 'c' && choice != 'r' && choice != 'q') //Check if cannot parse, or if not equal to a choice
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

                        if (player.playerID == 2 && players[0] is AI) // Is player turn
                        {
                                int PIP = player.currentBet;
                                int VPIP = PIP - player.blindPaid; // PIP - blinds.
                                Ai.AddTendency(VPIP+amountToCall,0);
                        }

                        

                        if (amountToCall > player.currentChips)
                        {
                            player.allIn = true;
                        }
                       // player.checking = true;
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
                    NoRaises = true;
                    if (player.possibleActions[3])
                    {
                        TakeRaiseAmount(player);
                    }
                }

                //Quit
                if(choice == 'q')
                {
                    if (player.possibleActions[4])
                    {
                        Main(null);
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
                int amountToCall = table.highestBet - player.currentBet;
                //if (amountToRaise > table.highestBet - amountToCall) //Check if input is more than previous bet
                //{
                    if (amountToRaise + amountToCall > 0 && amountToRaise <= player.currentChips) //Check if player is raising more than 0 and has enough to raise
                    {
                        player.Bet(amountToRaise + amountToCall);
                        if (player.playerID == 2 && players[0] is AI)
                        {
                            int PIP = player.currentBet;
                            int costToCheck = table.highestBet;
                            int VPIP = PIP - player.blindPaid; // PIP - blinds.
                            Ai.AddTendency(VPIP, amountToRaise-costToCheck);
                        }
                        table.currentPot += amountToRaise+amountToCall;
                        table.highestBet = player.currentBet;
                        
                    }
                    else
                    {
                        Console.WriteLine("Not enough chips");
                        TakeRaiseAmount(player);
                    }
                /*}
                else
                {
                    Console.WriteLine("Not higher than previous bet");
                    TakeRaiseAmount(player);
                }*/
            }
            else
            {
                Console.WriteLine("Input Not Recognised...");
                TakeRaiseAmount(player);
            }
        }

        //Calculate the winner/winners
        static void CalculateWinner()
        {
            //Get all winners, potential for multiple winners and pot split
            int[] winningPlayers = evaluate.DecideWinner(ref players, ref table);

            if (winningPlayers.Length == 1)
            {
                players[winningPlayers[0]].currentChips += table.currentPot;
            }
            else
            {
                float potSplit = 1 / winningPlayers.Length;

                for (int i = 0; i < winningPlayers.Length; i++)
                {
                    players[winningPlayers[i]].currentChips += (int)(table.currentPot * potSplit);
                }
            }
            //Evaluate.Grades highestGrade = players[0].grade;
            //List<Player> winners = new List<Player>();

            //for(int i = 1; i < players.Count - 1;i++)
            //{
            //    if(players[i].grade > highestGrade)
            //    {
            //        highestGrade = players[i].grade;
            //        winners.Clear();
            //        winners.Add(players[i]);
            //    }
            //    else
            //    {
            //        if (players[i].grade == highestGrade)
            //        {
            //            winners.Add(players[i]);
            //        }
            //    }
            //}
            //if(winners[0].highCard > winners[1].highCard)
            //{

            //}

        }

        //Reset the round for the next
        static void EndRound()
        {
            table.RefreshTable();
            deck.RefreshDeck();
            foreach (Player player in players)
                player.RefreshPlayer();

            RemoveLosers();
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
            Console.WriteLine("\n--------TABLE--------\n");
            table.PrintTable();
            Console.WriteLine("\n--------CARDS--------\n");
            player.PrintHand();
            Console.WriteLine("\n--------CHIPS--------\n");
            Console.WriteLine("Pot: {0}",table.currentPot);
            Console.WriteLine("Player has " + player.currentChips + " chips.");
            Console.WriteLine("Players:");
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].inRound == true)
                {
                    Console.Write("Player" + players[i].playerID + "\t");
                    Console.Write(players[i].currentChips + "\n");
                    if(players[i].checking == true)
                    {
                        //Console.Write("\tChecking");
                    }else if(players[i].allIn == true)
                    {
                        Console.WriteLine("All In !");
                    }
                }
            }
            Console.WriteLine("\n--------OPTIONS--------\n");
        }
    }
}
