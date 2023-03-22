using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace 盾构机机器人操作界面V0
{
	internal class Mysocket
	{
		/// <summary>
		/// 创建myClient套接字实例
		/// </summary>
		static private Socket myClient = client();

		/// <summary>
		///设定套接字信息 
		/// </summary>
		/// <returns></returns>
		private static Socket client()
		{
			return new(
		AddressFamily.InterNetwork,
		SocketType.Stream,
		ProtocolType.Tcp);
		}
		/// <summary>
		/// 
		/// 建立套接字，并且等待连接
		/// </summary>
		/// <returns></returns>
		public static int SocketBuild()
		{
			myClient = client();
			IPAddress remoteHost = IPAddress.Parse("127.0.0.1");
			IPEndPoint iep = new IPEndPoint(remoteHost, 1115);
			myClient.Connect(iep);
			return 0;
		}
		/// <summary>
		/// 套接字发送数据
		/// </summary>
		public static async void mysocket_send(Single[] data)
		{
			//Socket myclient = s;
			var messagebytes0 = BitConverter.GetBytes(data[0]);
			var messagebytes1 = BitConverter.GetBytes(data[1]);
			byte[] messagebytes3 = messagebytes0.Concat(messagebytes1).ToArray();
			try
			{
				_ = await myClient.SendAsync(messagebytes3, SocketFlags.None);
			}
			catch { };
		}
		public static async void mysocket_send2(Byte[] data)
		{
			try
			{
				_ = await myClient.SendAsync(data, SocketFlags.None);
			}
			catch { };
		}
		/// <summary>
		/// 关闭套接字
		/// </summary>
		public static void SocketEnd()
		{
			myClient.Shutdown(SocketShutdown.Both);
			myClient.Close();
		}

		public static async Task<float[]> SocketReceivedAsync()
		{
			var received = await myClient.ReceiveAsync(buffer, SocketFlags.None);
			//_ = myClient.ReceiveAsync(buffer, SocketFlags.None);
			for (int i = 0; i < 9; i++)
			{
				Getdata[i] = BitConverter.ToSingle(buffer, i * 4);
			}
			return Getdata;
		}
		private static byte[] buffer = new byte[1024];
		public static float[] Getdata = new float[9];
	}
}
