

namespace InternetBanking.Application.Helpers
{
    public class GeneradosNumeros
    {
        public static string GenerarTarjeta()
        {
            Random rand = new Random();
            string numeroTarjeta = "";

            // Generar 16 dígitos aleatorios
            for (int i = 0; i < 16; i++)
            {
                numeroTarjeta += rand.Next(0, 10).ToString(); 
            }

            return numeroTarjeta;
        }
        public static string GenerateNineDigite()
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
