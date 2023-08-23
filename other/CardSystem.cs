using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Cache_Money_Questions.other
{
    public class CardSystem
    {
        private int[] cardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private string[] cardSuits = { "Clubs", "Spades", "Diamonds", "Hearts" };

        public int SelectedNum { get; set; }
        public string SelectedCard { get; set; } 
        public bool IsFaceCard { get; set; }
        public string FaceCard { get; set; }    

        public CardSystem()
        {
            var random = new Random();
            int numberIndex = random.Next(0, cardNumbers.Length);
            int suiteIndex = random.Next(0, cardSuits.Length);

            SelectedNum = cardNumbers[numberIndex];

            if (SelectedNum == 1)
                FaceCard = "Ace";
            else if (SelectedNum == 11)
                FaceCard = "Jack";
            else if (SelectedNum == 12)
                FaceCard = "Queen";
            else if (SelectedNum == 13)
                FaceCard = "King";
            else
                FaceCard = null;

            IsFaceCard = FaceCard != null;

            if (IsFaceCard)
                SelectedCard = $"{FaceCard} of {cardSuits[suiteIndex]}";
            else
                SelectedCard = $"{SelectedNum} of {cardSuits[suiteIndex]}";

        }
    }
}
