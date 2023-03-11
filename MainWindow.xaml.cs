using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace 盾构机机器人操作界面V0
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

	public partial class MainWindow : Window
	{
		public float[] joint_theta_to_servo = new float[8];
		public Int32 Command = 0;
		public bool Command_flag = false;

		MyData_UPDATE user = new MyData_UPDATE();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = user;
			user.Var_state = "未连接!";
			user.Var0 = "23.4";
		}

		private void 连接_Click(object sender, RoutedEventArgs e)
		{
			if (user.Var_state == "已连接!")
			{
				Mysocket.SocketEnd();
				user.Var_state = "未连接！";
				Led1.Background = Brushes.DarkRed;
				Led2.Background = Brushes.DarkRed;
			}
			else
			{
				try
				{
					Mysocket.SocketBuild();
					user.Var_state = "已连接!";
					Led1.Background = Brushes.Green;
					var th_status = new Thread(this.Th_status_get);
					th_status.Start();
					var th_order_send = new Thread(this.Th_Order_send);
					th_order_send.Start();
				}
				catch
				{
					Led1.Background = Brushes.DarkRed;
					MessageBoxResult result = MessageBox.Show("请检查通讯！", "警告！", MessageBoxButton.OK, MessageBoxImage.Error);
				};

			}
		}
		private void 使能_click(object sender, RoutedEventArgs e)
		{
			if (user.Var_state == "已连接!")
			{
				if (Command_flag)
				{
					Command = 0;
					Led2.Background = Brushes.Yellow;
					Command_flag = false;
				}
				else
				{
					Command |= 0b10;
					Command_flag = true;
					Led2.Background = Brushes.Green;
				}
			}
			else
			{
				MessageBoxResult result = MessageBox.Show("请先打开通讯！", "警告！", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		public void Send_Click(object sender, RoutedEventArgs e)
		{
			if (user.Var_state == "已连接!")
			{
				if ((Command & 0b10)==0b10)
				{
					Single[] data = new Single[1024];
					var string1 = Joint_read1.Text;
					var string2 = Joint_read2.Text;
					var string3 = Joint_read3.Text;
					var string4 = Joint_read4.Text;
					var string5 = Joint_read5.Text;
					var string6 = Joint_read6.Text;
					var string7 = Joint_read7.Text;
					var string8 = Joint_read8.Text;
					joint_theta_to_servo[0] = Convert.ToSingle(string1);
					joint_theta_to_servo[1] = Convert.ToSingle(string2);
					joint_theta_to_servo[2] = Convert.ToSingle(string3);
					joint_theta_to_servo[3] = Convert.ToSingle(string4);
					joint_theta_to_servo[4] = Convert.ToSingle(string5);
					joint_theta_to_servo[5] = Convert.ToSingle(string6);
					joint_theta_to_servo[6] = Convert.ToSingle(string7);
					joint_theta_to_servo[7] = Convert.ToSingle(string8);
				}
				else
				{
					MessageBoxResult result = MessageBox.Show("请先打开使能！", "警告！", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
		
			}
			else
			{
				MessageBoxResult result = MessageBox.Show("请先打开通讯！", "警告！", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		public void Th_status_get()
		{
			while (true)
			{
				try
				{
					float[] getdata = Mysocket.SocketReceivedAsync().Result;
					user.Var0 = Convert.ToString(getdata[0]);
					user.Var1 = Convert.ToString(getdata[1]);
					user.Var2 = Convert.ToString(getdata[2]);
					user.Var3 = Convert.ToString(getdata[3]);
					user.Var4 = Convert.ToString(getdata[4]);
					user.Var5 = Convert.ToString(getdata[5]);
					user.Var6 = Convert.ToString(getdata[6]);
					user.Var7 = Convert.ToString(getdata[7]);
				}
				catch { };

			}

		}
		public void Th_Order_send()
		{
			while (true)
			{
				byte[] Data_to_Server = new byte[1024];
				var Head_Check_Byte = BitConverter.GetBytes(22);
				var Command_Byte = BitConverter.GetBytes(Command);
				byte[][] temp_joint = new byte[8][];
				for (int i = 0; i < 8; i++)
				{
					temp_joint[i] = BitConverter.GetBytes(joint_theta_to_servo[i]);
				}
				Head_Check_Byte.CopyTo(Data_to_Server, 0);
				Command_Byte.CopyTo(Data_to_Server, 4);
				for (int j = 0; j < 8; j++)
				{
					temp_joint[j].CopyTo(Data_to_Server, j * 4 + 8);
				}
				Mysocket.mysocket_send2(Data_to_Server);
				Thread.Sleep(300);
			}
		}

	}
}
/// <summary>
/// 创建User类
/// </summary>
public class MyData_UPDATE : INotifyPropertyChanged
{
	private string? _var_state; //通讯标志
	private string? _var_get_joint_theta0; //关节0的角度值（status)
	private string? _var_get_joint_theta1;//关节1的角度值（status)
	private string? _var_get_joint_theta2;//关节2的角度值（status)
	private string? _var_get_joint_theta3;//关节3的角度值（status)
	private string? _var_get_joint_theta4;//关节4的角度值（status)
	private string? _var_get_joint_theta5;//关节5的角度值（status)
	private string? _var_get_joint_theta6;//关节6的角度值（status)
	private string? _var_get_joint_theta7;//关节7的角度值（status)
	public string? Var_state
	{
		get { return _var_state; }
		set
		{
			_var_state = value;
			OnPropertyChanged(nameof(Var_state));
		}
	}
	public string? Var0
	{
		get { return _var_get_joint_theta0; }
		set
		{
			_var_get_joint_theta0 = value;
			OnPropertyChanged(nameof(Var0));
		}
	}
	public string? Var1
	{
		get { return _var_get_joint_theta1; }
		set
		{
			_var_get_joint_theta1 = value;
			OnPropertyChanged(nameof(Var1));
		}
	}
	public string? Var2
	{
		get { return _var_get_joint_theta2; }
		set
		{
			_var_get_joint_theta2 = value;
			OnPropertyChanged(nameof(Var2));
		}
	}
	public string? Var3
	{
		get { return _var_get_joint_theta3; }
		set
		{
			_var_get_joint_theta3 = value;
			OnPropertyChanged(nameof(Var3));
		}
	}
	public string? Var4
	{
		get { return _var_get_joint_theta4; }
		set
		{
			_var_get_joint_theta4 = value;
			OnPropertyChanged(nameof(Var4));
		}
	}
	public string? Var5
	{
		get { return _var_get_joint_theta5; }
		set
		{
			_var_get_joint_theta5 = value;
			OnPropertyChanged(nameof(Var5));
		}
	}
	public string? Var6
	{
		get { return _var_get_joint_theta6; }
		set
		{
			_var_get_joint_theta6 = value;
			OnPropertyChanged(nameof(Var6));
		}
	}
	public string? Var7
	{
		get { return _var_get_joint_theta7; }
		set
		{
			_var_get_joint_theta7 = value;
			OnPropertyChanged(nameof(Var7));
		}
	}
	public event PropertyChangedEventHandler? PropertyChanged;
	/// <summary>
	/// 当指定的某个属性发生变化时，对界面UI的线程推送，更新消息
	/// </summary>
	/// <param name="propertyName"></param>
	public void OnPropertyChanged(string propertyName)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}


