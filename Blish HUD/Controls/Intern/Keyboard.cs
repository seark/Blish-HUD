﻿using System;
using System.Collections.Generic;
using Blish_HUD.Controls.Extern;
namespace Blish_HUD.Controls.Intern
{
    public static class Keyboard
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const uint MAPVK_VK_TO_VSC = 0x00;
        private const uint MAPVK_VSC_TO_VK = 0x01;
        private const uint MAPVK_VK_TO_CHAR = 0x02;
        private const uint MAPVK_VSC_TO_VK_EX = 0x03;
        private const uint MAPVK_VK_TO_VSC_EX = 0x04;

        /// <summary>
        /// Presses a key.
        /// </summary>
        /// <param name="key">Virtual Key Short</param>
        public static void Press(VirtualKeyShort key)
        {
            if (!GameService.GameIntegration.Gw2IsRunning)
            {
                var nInputs = new[]
                {
                    new Extern.Input
                    {
                        type = InputType.KEYBOARD,
                        U = new InputUnion
                        {
                            ki = new KeybdInput
                            {
                                wScan = (ScanCodeShort)PInvoke.MapVirtualKey((uint)key, MAPVK_VK_TO_VSC),
                                wVk = key
                            }
                        }
                    }
                };
                PInvoke.SendInput((uint)nInputs.Length, nInputs, Extern.Input.Size);
            }
            else
            {
                uint vkCode = (uint)key;
                ExtraKeyInfo lParam = new ExtraKeyInfo(){
                    scanCode = (char)PInvoke.MapVirtualKey(vkCode, MAPVK_VK_TO_VSC)
                };
                PInvoke.PostMessage(GameService.GameIntegration.Gw2Process.MainWindowHandle, WM_KEYDOWN, vkCode, lParam.GetInt());
            }
        }

        /// <summary>
        /// Releases a key.
        /// </summary>
        /// <param name="key">Virtual Key Short</param>
        public static void Release(VirtualKeyShort key)
        {
            if (!GameService.GameIntegration.Gw2IsRunning)
            {
                var nInputs = new[]
                {
                    new Extern.Input
                    {
                        type = InputType.KEYBOARD,
                        U = new InputUnion
                        {
                            ki = new KeybdInput
                            {
                                wScan = (ScanCodeShort)PInvoke.MapVirtualKey((uint)key, MAPVK_VK_TO_VSC),
                                wVk = key,
                                dwFlags = KeyEventF.KEYUP
                            }
                        }
                    }
                };
                PInvoke.SendInput((uint)nInputs.Length, nInputs, Extern.Input.Size);
            }
            else
            {
                uint vkCode = (uint)key;
                ExtraKeyInfo lParam = new ExtraKeyInfo
                {
                    scanCode = (char)PInvoke.MapVirtualKey(vkCode, MAPVK_VK_TO_VSC),
                    repeatCount = 1,
                    prevKeyState = 1,
                    transitionState = 1
                };
                PInvoke.PostMessage(GameService.GameIntegration.Gw2Process.MainWindowHandle, WM_KEYUP, vkCode, lParam.GetInt());
            }
        }
    }
    class ExtraKeyInfo
    {
        public ushort repeatCount;
        public char scanCode;
        public ushort extendedKey, prevKeyState, transitionState;

        public int GetInt()
        {
            return repeatCount | (scanCode << 16) | (extendedKey << 24) |
                (prevKeyState << 30) | (transitionState << 31);
        }
    };
}