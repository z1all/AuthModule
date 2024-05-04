using CryptoModule.Interfaces;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace CryptoModule.Services
{
    public class ECCCryptoService : IAsymmetricCryptoService
    {
        private readonly Random _random = new Random();
        private readonly BigInteger _alphabetLength = 128;

        public byte[] Decrypt(string privateKey, byte[] data)
        {
            // private 
            int a = int.Parse(privateKey.Split(' ')[0]);
            BigInteger p = BigInteger.Parse(privateKey.Split(' ')[1]);

            List<byte> decryptedMessage = new List<byte>();
            string[] encryptedMessage = Encoding.UTF8.GetString(data).Split(',');
            foreach(var coordinate in encryptedMessage)
            {
                string[] coordinates = coordinate.Split(' ');

                if (coordinates.Length != 2) continue;

                BigInteger y1 = BigInteger.Parse(coordinates[0]);
                BigInteger y2 = BigInteger.Parse(coordinates[1]);

                // уравнение  y1^(-a) * y2 (mod p)
                //BigInteger modInverse = ModInverse(BigInteger.Pow(y1, a), p);
                //BigInteger y1ModK = modInverse % p;

                BigInteger y1ModK = ModInverse(BigInteger.ModPow(y1, a, p), p);
                BigInteger y2ModK = y2 % p;
                BigInteger mA = (y1ModK * y2ModK) % p;

                //BigInteger modInverse = ModInverse(BigInteger.Pow(y1, a), p);
                //BigInteger mA = (modInverse * y2) % p;
                decryptedMessage.AddRange(DecodingBlock(mA));
            }

            int messageBytesCount = int.Parse(encryptedMessage.Last());
            byte[] decryptedMessageBytes = new byte[messageBytesCount];
            Array.Copy(decryptedMessage.ToArray(), decryptedMessageBytes, decryptedMessageBytes.Length);
            return decryptedMessageBytes;
        }

        public byte[] Encrypt(string publicKey, byte[] data)
        {
            // public
            string[] curve = publicKey.Split(' ');
            BigInteger p = BigInteger.Parse(curve[0]);
            BigInteger g = BigInteger.Parse(curve[1]);
            BigInteger K = BigInteger.Parse(curve[2]);

            List<BigInteger> codingBlocks = CodingBlocks(data, 18);

            int b = 1000 + _random.Next() % 1000;
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < codingBlocks.Count; ++i)
            {
                BigInteger codingBlock = codingBlocks[i];

                BigInteger y1 = BigInteger.Pow(g, b) % p;
                BigInteger y2 = (BigInteger.Pow(K, b) * codingBlock) % p;

                stringBuilder.Append($"{y1} {y2},");
            }
            stringBuilder.Append(data.Length);

            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        private List<byte> DecodingBlock(BigInteger m)
        {
            List<byte> block = new();

            BigInteger mCopy = m;
            while (mCopy != 0)
            {
                block.Add((byte)(mCopy % _alphabetLength));
                mCopy = mCopy / _alphabetLength;
            }

            block.Reverse();

            return block;
        }

        public Keys MakeKeysPair()
        {
            // private 
            int a = 1_000_000 + _random.Next() % 100_000;

            // public
            BigInteger p = BigInteger.Parse("00FFFFF0001000000000000000FFFFFFFF", NumberStyles.HexNumber);
            BigInteger g = 3;
            BigInteger K = BigInteger.Pow(g, a) % p;

            return new()
            {
                PrivateKey = $"{a} {p}",
                PublicKey = $"{p} {g} {K}",
            };
        }

        private List<BigInteger> CodingBlocks(byte[] data, int blockLength)
        {
            List<BigInteger> coding = new();

            for (int i = 0; i < data.Length / blockLength + (data.Length % blockLength != 0 ? 1 : 0); ++i)
            {
                coding.Add(CodingBlocks(data, i * blockLength, blockLength));
            }

            return coding;
        }

        private BigInteger CodingBlocks(byte[] data, int offset, int length)
        {
            BigInteger ans = 0;

            for(int i = offset + length - 1; i >= offset; --i)
            {
                int pos = length - (i - offset) - 1;

                if (i >= data.Length) continue;

                ans += data[i] * BigInteger.Pow(_alphabetLength, pos);
            }

            return ans;
        }

        BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }
    }
}

/*
  private Keys MakeKeysPair2()
        {
            #region Создание пары ключей

            BigInteger p = 15485863; // Выбираем: Простое число p в данном случае достаточно большой
            BigInteger g = 7; // Генератор g,мы возьмем координату и нашего генератора ECC:7.
            int a = 21702; // Случайный закрытый ключ  a : 21702

            // Теперь Алисия рассчитывает K=g^a(mod p) чтобы найти открытый ключ (g,p,K) , тогда
            // K = 721702(mod 15485862) = 8890431 открытый ключ будет (g, p, K) = (7, 15485862, 8890431)
            BigInteger K = BigInteger.Pow(g, a) % p;

            #endregion

            #region Шифрование сообшение с помощью public key (g, p, K)
            // Теперь, имея таблицу с алфавитом без n, то есть таблицу размером 27,
            // мы выражаем сообщение  m  который Боб хочет отправить,
            // какой из них является "HIJO", следующим образом, числовое
            // значение буквы  ⋅26^position ,  м = 7*26^3 + 8*26^2 + 9*26 + 14 = 128688 .
            BigInteger mB = 7 * BigInteger.Pow(26, 3) + 8 * BigInteger.Pow(26, 2) + 9 * BigInteger.Pow(26, 1) + 14 * BigInteger.Pow(26, 0);

            // Теперь Боб выбирает случайное число  b
            // Входите  2yp - 1  в этом случае  b = 480  а затем вычислите:
            // y1 = g^b (mod p) и y2 = (K^b)m (mod p)
            int b = 23423;
            BigInteger y1 = BigInteger.Pow(g, b) % p;
            BigInteger y2 = (BigInteger.Pow(K, b) * mB) % p;

            #endregion

            #region Расшифровка с помощью privat key (a)
            // Теперь Алиса расшифровывает сообщения с помощью закрытого ключа а = 21702
            // и уравнение  y1^(-a) * y2 (mod K) = 14823281 * 8263449 (mod 15485863) = 128688,
            // если мы посмотрим, мы найдем то же значение, что и m таким образом, Алиса получает
            // сообщение от Боба с помощью алгоритма Эльгамала
            BigInteger modInverse = ModInverse(BigInteger.Pow(y1, a), p);
            BigInteger mA = (modInverse * y2) % p;

            #endregion


            throw new NotImplementedException();
        }
 
 */
