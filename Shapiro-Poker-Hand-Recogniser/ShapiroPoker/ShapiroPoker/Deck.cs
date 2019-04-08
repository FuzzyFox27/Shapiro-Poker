using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_AI_Game
{
    class Deck
    {
        //Deck Holders
        Card[] templateDeck = new Card[52];
        List<Card> publicDeck = new List<Card>();

        //Randomness
        Random rand = new Random();

        //Constructor
        public Deck()
        {
            PopulateDeck();
            RefreshDeck();
        }

        //Populate template deck once only
        void PopulateDeck()
        {
            int i = 0;

            foreach (Suits suit in Enum.GetValues(typeof(Suits)))
            {
                foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
                {
                    templateDeck[i].suit = suit;
                    templateDeck[i].rank = rank;

                    i++;
                }
            }
        }

        //Clear current playing deck and mirror template
        public void RefreshDeck()
        {
            publicDeck.Clear();

            foreach (Card card in templateDeck)
            {
                publicDeck.Add(card);
            }
        }

        //Randomly pick 2 cards and then remove them
        public Card[] GetCards(int amount)
        {
            Card[] tempHand = new Card[amount];

            for (int i = 0; i < amount; i++)
            {
                tempHand[i] = WithdrawCard();
            }

            return tempHand;
        }

        //Picks a single card randomly from deck, remembers it, removes it and then returns it
        Card WithdrawCard()
        {
            int positionOfCard = rand.Next(0, publicDeck.Count);
            Card tempCard = publicDeck[positionOfCard];
            publicDeck.RemoveAt(positionOfCard);
            return tempCard;
        }

        public void PrintDeck()
        {
            foreach (Card card in publicDeck)
            {
                Console.WriteLine("Suit: " + card.suit + " Rank: " + card.rank);
            }
        }
    }

    //Hold information for one card -- Make 52
    struct Card
    {
        public Suits suit;
        public Ranks rank;
    }

    //Make life easier by being able to use suits instead of number alternatives
    public enum Suits
    {
        Hearts, Diamonds, Spades, Clubs
    }

    public enum Ranks
    {
        Ace = 14, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13
    }
}