using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Bridge.Html5;

namespace DanTup.DaChip8JS
{
	public static class Main
	{
		static Chip8 chip8;
		const string ROM = "../../../ROMs/Chip-8 Pack/Chip-8 Games/Breakout (Brix hack) [David Winter, 1997].ch8";

		// For timing..
		static readonly Stopwatch stopWatch = Stopwatch.StartNew();
		static readonly TimeSpan targetElapsedTime60Hz = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
		static readonly TimeSpan targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 1000);
		static TimeSpan lastTime;

		[Ready]
		public static void OnReady()
		{
			chip8 = new Chip8(Draw, Beep);
			//chip8.LoadProgram(File.ReadAllBytes(ROM));

			Document.OnKeyUp += SetKeyDown;
			Document.OnKeyDown += SetKeyUp;

			StartGameLoop();
		}

		public static void Draw(bool[,] buffer)
		{
		}

		public static void Beep(int milliseconds)
		{
		}

		static Dictionary<Keys, byte> keyMapping = new Dictionary<Keys, byte>
		{
			{ Keys.D1, 0x1 },
			{ Keys.D2, 0x2 },
			{ Keys.D3, 0x3 },
			{ Keys.D4, 0xC },
			{ Keys.Q, 0x4 },
			{ Keys.W, 0x5 },
			{ Keys.E, 0x6 },
			{ Keys.R, 0xD },
			{ Keys.A, 0x7 },
			{ Keys.S, 0x8 },
			{ Keys.D, 0x9 },
			{ Keys.F, 0xE },
			{ Keys.Z, 0xA },
			{ Keys.X, 0x0 },
			{ Keys.C, 0xB },
			{ Keys.V, 0xF },
		};

		static void SetKeyDown(object sender, KeyEventArgs e)
		{
			if (keyMapping.ContainsKey(e.KeyCode))
				chip8.KeyDown(keyMapping[e.KeyCode]);
		}

		static void SetKeyUp(object sender, KeyEventArgs e)
		{
			if (keyMapping.ContainsKey(e.KeyCode))
				chip8.KeyUp(keyMapping[e.KeyCode]);
		}

		static void StartGameLoop()
		{
			Task.Run(GameLoop);
		}

		static Task GameLoop()
		{
			while (true)
			{
				var currentTime = stopWatch.Elapsed;
				var elapsedTime = currentTime - lastTime;

				while (elapsedTime >= targetElapsedTime60Hz)
				{
					this.Invoke((Action)Tick60Hz);
					elapsedTime -= targetElapsedTime60Hz;
					lastTime += targetElapsedTime60Hz;
				}

				this.Invoke((Action)Tick);

				Thread.Sleep(targetElapsedTime);
			}
		}

		static void Tick() => chip8.Tick();
		static void Tick60Hz()
		{
			chip8.Tick60Hz();
			pbScreen.Refresh();
		}
	}
}
