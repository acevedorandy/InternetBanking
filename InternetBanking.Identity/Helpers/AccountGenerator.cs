

namespace InternetBanking.Identity.Helpers
{
    public class AccountGenerator
    {
        public static string GenerateAccount()
        {
            Random rand = new Random();
            string numeroTarjeta = "";

            // Generar 16 dígitos aleatorios
            for (int i = 0; i < 9; i++)
            {
                numeroTarjeta += rand.Next(0, 10).ToString();
            }

            return numeroTarjeta;
        }
    }
}
