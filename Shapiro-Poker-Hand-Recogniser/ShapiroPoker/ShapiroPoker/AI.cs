using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poker_AI_Game;


namespace ShapiroPoker
{
    class AI
    {
        //Since there will be only one AI, and this AI will be "player 1", we will create the AI first.
        //retrieve the player list, this will allow the AI to access all the info that it would have as a player, 
        //it is always player 1 so we will always retrieve the first position in the list.
        static Player AIPlayerClass = MainProgram.players.First();
        static void updateplayer()
        {
            AIPlayerClass = MainProgram.players.First();
        }
        static int lastbet;
        public static void GetLastBet(int bet)
        {
            lastbet = bet;
        }
        public static char MakeChoice()
        {
            updateplayer();
            //must check if it must call at least.
            if (lastbet <= AIPlayerClass.currentBet)
            {
                //must either be a call, raise, or fold
                if (AIPlayerClass.currentChips > lastbet || AIPlayerClass.currentChips == 100)
                {
                    return 'r';
                }
                else
                {
                    return 'f';
                }
            }
            else
            {
                //may call, raise, fold, or check
                return 'c';
            }
        }

        public static string ChooseRaiseAmount()
        {
            int AmountToRaise = 1;
            int Raise = lastbet + AmountToRaise;
            return Raise.ToString();
        }
        
    }
}
