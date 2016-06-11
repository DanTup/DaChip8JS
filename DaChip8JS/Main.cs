using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;

namespace DanTup.DaChip8JS
{
	public static class Main
	{
		static Chip8 chip8;
		const string ROM = "ROMs/Chip-8 Pack/Chip-8 Games/Breakout (Brix hack) [David Winter, 1997].ch8";

		static readonly int targetElapsedTime60Hz = (int)(1000f / 60); // 60 tickets per second
		static readonly int targetElapsedTime = (int)(1000f / 500); // 500 ticks per second

		[Ready]
		public static void OnReady()
		{
			chip8 = new Chip8(Draw, Beep);

			BeginLoadRom(ROM);

			Document.OnKeyUp += SetKeyDown;
			Document.OnKeyDown += SetKeyUp;
		}

		static void BeginLoadRom(string rom)
		{
			var req = new XMLHttpRequest();
			req.Open("GET", rom);
			req.OnLoad = e => EndLoadRom(ConvertToByteArray(req.ResponseText));
			req.Send();
		}

		static byte[] ConvertToByteArray(string data)
		{
			return data.ToCharArray().Select(c => (byte)c).ToArray();
		}

		static void EndLoadRom(byte[] data)
		{
			chip8.LoadProgram(data);

			StartGameLoop();
		}

		static void Draw(bool[,] buffer)
		{
		}

		static void Beep(int milliseconds)
		{
		}

		static Dictionary<KeyCode, byte> keyMapping = new Dictionary<KeyCode, byte>
		{
			{ KeyCode.D1, 0x1 },
			{ KeyCode.D2, 0x2 },
			{ KeyCode.D3, 0x3 },
			{ KeyCode.D4, 0xC },
			{ KeyCode.Q, 0x4 },
			{ KeyCode.W, 0x5 },
			{ KeyCode.E, 0x6 },
			{ KeyCode.R, 0xD },
			{ KeyCode.A, 0x7 },
			{ KeyCode.S, 0x8 },
			{ KeyCode.D, 0x9 },
			{ KeyCode.F, 0xE },
			{ KeyCode.Z, 0xA },
			{ KeyCode.X, 0x0 },
			{ KeyCode.C, 0xB },
			{ KeyCode.V, 0xF },
		};

		static void SetKeyDown(KeyboardEvent e)
		{
			if (keyMapping.ContainsKey((KeyCode)e.KeyCode))
				chip8.KeyDown(keyMapping[(KeyCode)e.KeyCode]);
		}

		static void SetKeyUp(KeyboardEvent e)
		{
			if (keyMapping.ContainsKey((KeyCode)e.KeyCode))
				chip8.KeyUp(keyMapping[(KeyCode)e.KeyCode]);
		}

		static void StartGameLoop()
		{
			Window.SetInterval(chip8.Tick, targetElapsedTime);
			Window.SetInterval(chip8.Tick60Hz, targetElapsedTime60Hz);
		}
	}
}
