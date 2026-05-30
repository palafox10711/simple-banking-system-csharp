using System.Diagnostics.CodeAnalysis;

namespace BankingSystem;

static class LuhnGenerator
{
    public static string GenerateCardNumber()
    {

        int[] numeros = new int[15];
        for (int i = 0; i < 15; i++)
        {
            numeros[i] = Random.Shared.Next(0,10);
        }
        return string.Join("",numeros) + CalculateCheckDigit(numeros);

    }
    private static int CalculateCheckDigit(int[] digits)
    {
        int suma = 0;
        for (int i = 0; i < 15; i++)
        {
            
            int d = digits[i];
            if(i % 2 == 0)
            {
                d *= 2;

                   if(d > 9)
                   {
                    d -= 9;
                }
            }
            suma += d;
        }
        return (10 - (suma % 10)) % 10;
    }


    public static bool IsValid(string cardNumber)
    {
        int[] digits = cardNumber.Select(c => c - '0').ToArray();
        int suma = 0;
        for(int i = 0; i < 16; i++)
        {
            int n = digits[i];
           if( i % 2 == 0)
            {
                n *= 2;
                if(n > 9)
                {
                    n -= 9;
                }
            }
            suma += n;
        }
        
        return suma % 10 == 0;
    }
   
}