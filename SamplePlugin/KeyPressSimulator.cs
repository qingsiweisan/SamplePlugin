using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MarketBoardAutoBuyer
{
    public static class KeyPressSimulator
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void SendInput(uint nInputs, Input[] pInputs, int cbSize);

        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;

        public static void PressKey(VirtualKey key)
        {
            byte keyCode = (byte)key;
            keybd_event(keyCode, 0, 0, 0);
            Task.Delay(50).Wait();
            keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);
        }

        public static async Task TypeChineseText(string text)
        {
            foreach (char c in text)
            {
                await Task.Delay(200);
                SendUnicodeCharacter(c);
            }
        }

        private static void SendUnicodeCharacter(char c)
        {
            Input[] inputs = new Input[2];

            inputs[0].Type = 1; // INPUT_KEYBOARD
            inputs[0].Data.Keyboard.Vk = 0;
            inputs[0].Data.Keyboard.Scan = c;
            inputs[0].Data.Keyboard.Flags = KEYEVENTF_UNICODE;
            inputs[0].Data.Keyboard.Time = 0;
            inputs[0].Data.Keyboard.ExtraInfo = IntPtr.Zero;

            inputs[1] = inputs[0];
            inputs[1].Data.Keyboard.Flags |= KEYEVENTF_KEYUP;

            SendInput(2, inputs, Marshal.SizeOf(typeof(Input)));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Input
        {
            public int Type;
            public InputUnion Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MouseInput Mouse;
            [FieldOffset(0)] public KeyboardInput Keyboard;
            [FieldOffset(0)] public HardwareInput Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int Dx;
            public int Dy;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }
    }

   
}
