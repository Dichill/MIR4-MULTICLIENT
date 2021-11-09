using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;


namespace MIR4_MULTICLIENT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		// File Path to Mir4.exe
		public string file_path = "D:/Steam/steamapps/common/MIR4/MirMobile/Binaries/Win64/Mir4.exe";
		internal static byte[] key_byte = MemStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MIR4_MULTICLIENT.Mir4Launcher"));

		int iRunIndex = 0;

		public MainWindow()
        {
			InitializeComponent();
		}

		internal static byte[] MemStream(object obj)
		{
			Stream stream = obj as Stream;
			Stream stream2 = stream;
			MemoryStream memoryStream = new MemoryStream();
			
			for (int i = 1; i < 4; i++)
			{
				stream.ReadByte();
			}

			ushort num = (ushort)stream.ReadByte();
			num = (ushort)~num;
			if ((num & 2) != 0)
			{
				MemoryStream memoryStream1 = new MemoryStream();
				DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();

				byte[] array = new byte[8];
				stream.Read(array, 0, 8);
				descryptoServiceProvider.IV = array;
				byte[] array2 = new byte[8];
				stream.Read(array2, 0, 8);
				byte[] array3 = array2;
                for (int j = 0; j < array3.Length; j++)
				{
					if (array3[j] != 0)
					{
                        bool flag = false;
                        if (flag)
						{
							array2 = GetToken();
						}
						descryptoServiceProvider.Key = array2;				

						memoryStream1.Position = 0L;
						ICryptoTransform cryptoTransform = descryptoServiceProvider.CreateDecryptor();
						int inputBlockSize = cryptoTransform.InputBlockSize;
						int outputBlockSize = cryptoTransform.OutputBlockSize;
						byte[] array4 = new byte[outputBlockSize];
						byte[] array5 = new byte[inputBlockSize];
						int num2 = (int)stream.Position;
						while ((num2 + inputBlockSize) < stream.Length)
						{
							stream.Read(array5, 0, inputBlockSize);
							int count = cryptoTransform.TransformBlock(array5, 0, inputBlockSize, array4, 0);
							memoryStream1.Write(array4, 0, count);
							num2 += inputBlockSize;
						}
						stream.Read(array5, 0, (int)(stream.Length - (long)num2));
						byte[] array6 = cryptoTransform.TransformFinalBlock(array5, 0, (int)(stream.Length - (long)num2));
						memoryStream1.Write(array6, 0, array6.Length);
						stream2 = memoryStream1;
						stream2.Position = 0L;
                        memoryStream = memoryStream1;
						goto IL_1D5;
					}
				}
			}
		IL_1D5:
			if ((num & 8) != 0)
			{
				MemoryStream memoryStream2 = new MemoryStream();	
				memoryStream2.Position = 0L;
				DeflateStream deflateStream = new DeflateStream(stream2, CompressionMode.Decompress);
				int num3 = 1000;
				byte[] buffer = new byte[1000];
				int num4;
				do
				{
					num4 = deflateStream.Read(buffer, 0, num3);
					if (num4 > 0)
					{
						memoryStream2.Write(buffer, 0, num4);
					}
				}
				while (num4 >= num3);
				memoryStream = memoryStream2;
			}
			if (memoryStream != null)
			{
				return memoryStream.ToArray();
			}
			byte[] array7 = new byte[stream.Length - stream.Position];
			stream.Read(array7, 0, array7.Length);
			return array7;
		}

		internal static string smethod_0(int int_1)
		{
			int num;
			if ((key_byte[int_1] & 128) == 0)
			{
				num = (int)key_byte[int_1];
				int_1++;
			}
			else if ((key_byte[int_1] & 64) == 0)
			{
				num = ((int)key_byte[int_1] & -129) << 8;
				num |= (int)key_byte[int_1 + 1];
				int_1 += 2;
			}
			else
			{
				num = ((int)key_byte[int_1] & -193) << 24;
				num |= (int)key_byte[int_1 + 1] << 16;
				num |= (int)key_byte[int_1 + 2] << 8;
				num |= (int)key_byte[int_1 + 3];
				int_1 += 4;
			}
			if (num < 1)
			{
				return string.Empty;
			}
			string @string = Encoding.Unicode.GetString(key_byte, int_1, num);
			return string.Intern(@string);
		}

		private static byte[] GetToken()
		{
			try
			{
				string fullName = "PublicKeyToken=b03f5f7f11d50a3a";
				int num = fullName.IndexOf("PublicKeyToken=");
				if (num < 0)
				{
					num = fullName.IndexOf("publickeytoken=");
				}
				if (num < 0)
				{
					return key_byte;
				}
				num += 15;
				if (fullName[num] != 'n')
				{
					if (fullName[num] != 'N')
					{
						string s = fullName.Substring(num, 16);
						long value = long.Parse(s, NumberStyles.HexNumber);
						byte[] bytes = BitConverter.GetBytes(value);
						Array.Reverse(bytes);
						return bytes;
					}
				}
				return key_byte;
			}
			catch
			{
			}

			return null;
		}

		private string AsciiConvert(string p0)
		{
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			return asciiencoding.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(asciiencoding.CodePage), Encoding.UTF8.GetBytes(p0)));
		}

		public byte[] m00003c(byte[] p0, byte[] p1)
		{
			Aes aes = aesDecrypt;
			byte[] result;
			try
			{
				aes = Aes.Create();
				aes.Key = p1;
				aes.Padding = (PaddingMode)GetData(2012);
				aes.Mode = (CipherMode)GetData(2016);
				ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				result = cryptoTransform.TransformFinalBlock(p0, GetData(2020), (int)(UIntPtr)p0.Length);
			}
			finally
			{
				bool flag = aes != aesDecrypt;
				if (flag)
				{
					for (; ; )
					{
						switch (5)
						{
							case 0:
						}
						break;
					}
					aes.Clear();
				}
			}
			return result;
		}

		private void CreateInstance_Click(object sender, RoutedEventArgs e)
        {
            string machineName = Environment.MachineName;
            string text = Guid.NewGuid().ToString();
            text = text.Replace("-", "");

            string p = machineName + "K3SJU9F2SSKJ96FSLO4XFWKD9WJFTGVS";
            string p2 = text + "OM3iBr94SD";

			string text2 = AsciiConvert(p);
			text2 = text2.Substring(BitConverter.ToInt32(key_byte, 1372), BitConverter.ToInt32(key_byte, 1376));

			byte[] bytes = Encoding.UTF8.GetBytes(AsciiConvert(p2));
            byte[] bytes2 = Encoding.UTF8.GetBytes(text2);
            byte[] inArray = m00003c(bytes, bytes2);
            string arg = Convert.ToBase64String(inArray);

            string param = string.Format("-FeatureLevelES31 -Windowed -ResX=1280 -ResY=720 -LauncherToken={0} -LaunchGameInstanceID={1} -ClientIndex={2} -opengl -Lang=EN", arg, text, iRunIndex.ToString());

			iRunIndex += 1;

			RunClient(file_path, param);
		}
		internal static int GetData(int value)
		{
			return BitConverter.ToInt32(key_byte, value);
		}

		public static void RunClient(string path, string param)
        {
			try
            {
				ProcessStartInfo startInfo = new ProcessStartInfo(path, param)
                {
                    CreateNoWindow = GetData(192) != 0,
                    RedirectStandardOutput = GetData(196) != 0,
                    UseShellExecute = GetData(200) != 0
                };
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
				MessageBox.Show(e.ToString());
            }
        }

		internal static Aes aesDecrypt;

		private static Dictionary<int, Process> processlist = new Dictionary<int, Process>();
	}
}
