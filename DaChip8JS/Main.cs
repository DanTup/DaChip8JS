using System;
using System.Collections.Generic;
using Bridge.Html5;

namespace DanTup.DaChip8JS
{
	public static class Main
	{
		static Chip8 chip8;
		const string ROM = "ROMs/Chip-8 Pack/Chip-8 Games/Breakout (Brix hack) [David Winter, 1997].ch8";

		static readonly int targetElapsedTime60Hz = (int)(1000f / 60); // 60 tickets per second
		static readonly int targetElapsedTime = (int)(1000f / 500); // 500 ticks per second

		static HTMLCanvasElement screen;
		static CanvasRenderingContext2D screenContext;
		static ImageData darkPixel, lightPixel;

		[Ready]
		public static void OnReady()
		{
			// Set up canvas rendering.
			screen = Document.GetElementById<HTMLCanvasElement>("screen");
			screenContext = screen.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
			darkPixel = screenContext.CreateImageData(1, 1);
			lightPixel = screenContext.CreateImageData(1, 1);
			lightPixel.Data[1] = 0x64; // Set green part (#006400)
			darkPixel.Data[3] = lightPixel.Data[3] = 255; // Alpha

			// Create the interpreter.
			chip8 = new Chip8(Draw, Beep);

			// Pass keypresses over to the interpreter.
			Document.OnKeyUp += SetKeyDown;
			Document.OnKeyDown += SetKeyUp;

			// Kick off async loading of ROM.
			BeginLoadRom(ROM);
		}

		static void BeginLoadRom(string rom)
		{
			var req = new XMLHttpRequest();
			req.ResponseType = XMLHttpRequestResponseType.ArrayBuffer;
			req.Open("GET", rom);
			req.OnLoad = e => EndLoadRom(GetResponseAsByteArray(req));
			req.Send();
		}

		static byte[] GetResponseAsByteArray(XMLHttpRequest req)
		{
			return new Uint8Array(req.Response as ArrayBuffer).As<byte[]>();
		}

		static void EndLoadRom(byte[] data)
		{
			chip8.LoadProgram(data);

			StartGameLoop();
		}

		static void Draw(bool[,] buffer)
		{
			for (var x = 0; x < buffer.GetLength(0); x++)
				for (var y = 0; y < buffer.GetLength(1); y++)
					screenContext.PutImageData(buffer[x, y] ? lightPixel : darkPixel, x, y);
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
