﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace DanTup.DaChip8
{
	class Chip8
	{
		Bitmap screen; // 64x32

		// Registers
		byte V0, V1, V2, V3, V4, V5, V6, V7, V8, V9, VA, VB, VC, VD, VE, VF;
		// Timers
		byte Delay, Sound;
		// Address/Program Counters
		ushort I, PC;
		// Stack
		byte SP;
		ushort[] Stack = new ushort[16];

		// Memory & ROM
		byte[] RAM = new byte[0x1000];
		byte[] Program;

		// OpCodes
		Dictionary<byte, Action<OpCodeData>> opCodes;

		public Chip8(Bitmap screen)
		{
			this.screen = screen;

			opCodes = new Dictionary<byte, Action<OpCodeData>>
			{
				{ 0x0, ClearOrReturn },
				{ 0x1, Jump },
				{ 0x2, CallSubroutine },
				{ 0x3, SkipIfXEqual },
				{ 0x4, SqipIfXNotEqual },
				{ 0x5, SkipIfXEqualY },
				{ 0x6, SetX },
				{ 0x7, AddX },
				{ 0x8, SetXFromY },
				{ 0x9, SkipIfXNotEqualY },
				{ 0xA, SetI },
				{ 0xB, JumpWithOffset },
				{ 0xC, Rnd },
				{ 0xD, DrawSprite },
				{ 0xE, SkipOnKey },
				{ 0xF, Misc },
			};
		}

		public void LoadProgram(byte[] data)
		{
			Program = data;
		}

		public void Tick()
		{
			// Read the two bytes of OpCode (big endian).
			var opCode = (ushort)(Program[PC++] << 8 | Program[PC++]);

			// Split data into the possible formats the instruction might need.
			// https://en.wikipedia.org/wiki/CHIP-8#Opcode_table
			var op = new OpCodeData()
			{
				OpCode = opCode,
				NNN = (ushort)(opCode & 0x0FFFF),
				X = (byte)(opCode & 0x0F00 >> 8),
				Y = (byte)(opCode & 0x00F0 >> 4),
				N = (byte)(opCode & 0x000F)
			};

			// Loop up the OpCode using the first nibble and execute.
			opCodes[(byte)(opCode >> 12)](op);
		}

		void ClearOrReturn(OpCodeData data) { }
		void Jump(OpCodeData data) { }
		void CallSubroutine(OpCodeData data) { }
		void SkipIfXEqual(OpCodeData data) { }
		void SqipIfXNotEqual(OpCodeData data) { }
		void SkipIfXEqualY(OpCodeData data) { }
		void SetX(OpCodeData data) { }
		void AddX(OpCodeData data) { }
		void SetXFromY(OpCodeData data) { }
		void SkipIfXNotEqualY(OpCodeData data) { }
		void SetI(OpCodeData data) { }
		void JumpWithOffset(OpCodeData data) { }
		void Rnd(OpCodeData data) { }
		void DrawSprite(OpCodeData data) { }
		void SkipOnKey(OpCodeData data) { }
		void Misc(OpCodeData data) { }
	}
}
