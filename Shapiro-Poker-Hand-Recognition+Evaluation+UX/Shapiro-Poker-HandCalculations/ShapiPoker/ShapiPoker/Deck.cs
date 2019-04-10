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

        public void RemoveSpecificCard(Card card)
        {
            publicDeck.Remove(card);
        }

        public Card SimulateWithdrawCard()
        {
            int positionOfCard = rand.Next(0, publicDeck.Count);
            Card tempCard = publicDeck[positionOfCard];
            return tempCard;
        }

    }

    //Hold information for one card -- Make 52
    struct Card
    {
        public Suits suit;
        public Ranks rank;

        public Card(Suits s, Ranks r)
        {
            suit = s;
            rank = r;
        }
    }

    //Make life easier by being able to use suits instead of number alternatives
    public enum Suits
    {
        Hearts, Diamonds, Spades, Clubs
    }

    public enum Ranks
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }
}