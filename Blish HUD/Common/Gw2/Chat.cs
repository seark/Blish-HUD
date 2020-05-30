﻿using System.Threading;
using System.Threading.Tasks;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Controls.Intern;
namespace Blish_HUD.Common.Gw2 {

    /// <summary>
    /// Methods related to interaction with the in-game chat.
    /// </summary>
    public static class Chat {
        private static readonly Logger Logger = Logger.GetLogger(typeof(Chat));
        /// <summary>
        /// Sends a message to the chat.
        /// </summary>
        public static async void Send(string message) {
            if (!Valid(message) && !GameService.GameIntegration.Gw2IsRunning) return;
            byte[] prevClipboardContent = await ClipboardUtil.WindowsClipboardService.GetAsUnicodeBytesAsync();
            await ClipboardUtil.WindowsClipboardService.SetTextAsync(message)
                               .ContinueWith((clipboardResult) => {
                                    if (clipboardResult.IsFaulted)
                                        Logger.Warn(clipboardResult.Exception, $"Failed to set clipboard text to \"{message}\"!",
                                                    message);
                                    else
                                        Task.Run(() => {
                                            Focus();
                                            Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                                            Keyboard.Stroke(VirtualKeyShort.KEY_V, true);
                                            Thread.Sleep(50);
                                            Keyboard.Release(VirtualKeyShort.LCONTROL, true);
                                            Keyboard.Stroke(VirtualKeyShort.RETURN);
                                        }).ContinueWith((result) => {
                                            if (prevClipboardContent != null)
                                                ClipboardUtil.WindowsClipboardService.SetUnicodeBytesAsync(prevClipboardContent);
                                        });
                                });
        }
        /// <summary>
        /// Adds a string to the input field.
        /// </summary>
        public static async void Paste(string text) {
            string currentInput = await GetInputText();
            if (!Valid(currentInput + text) && !GameService.GameIntegration.Gw2IsRunning) return;
            byte[] prevClipboardContent = await ClipboardUtil.WindowsClipboardService.GetAsUnicodeBytesAsync();
            await ClipboardUtil.WindowsClipboardService.SetTextAsync(text)
                               .ContinueWith((clipboardResult) => {
                                    if (clipboardResult.IsFaulted)
                                        Logger.Warn(clipboardResult.Exception, $"Failed to set clipboard text to \"{text}\"!",
                                                    text);
                                    else
                                        Task.Run(() => {
                                            Focus();
                                            Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                                            Keyboard.Stroke(VirtualKeyShort.KEY_V, true);
                                            Thread.Sleep(50);
                                            Keyboard.Release(VirtualKeyShort.LCONTROL, true);
                                        }).ContinueWith((result) => {
                                            if (prevClipboardContent != null)
                                                ClipboardUtil.WindowsClipboardService.SetUnicodeBytesAsync(prevClipboardContent);
                                        });
                                });
        }
        /// <summary>
        /// Returns the current string in the input field or null if empty.
        /// </summary>
        public static async Task<string> GetInputText() {
            if (!GameService.GameIntegration.Gw2IsRunning) return null;
            byte[] prevClipboardContent = await ClipboardUtil.WindowsClipboardService.GetAsUnicodeBytesAsync();
            await Task.Run(() => {
                        Focus();
                        Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                        Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
                        Keyboard.Stroke(VirtualKeyShort.KEY_C, true);
                        Thread.Sleep(50);
                        Keyboard.Release(VirtualKeyShort.LCONTROL, true);
                        Unfocus();
            });
            var currentInput = await ClipboardUtil.WindowsClipboardService.GetTextAsync();
            if (prevClipboardContent != null)
                await ClipboardUtil.WindowsClipboardService.SetUnicodeBytesAsync(prevClipboardContent);
            return currentInput;
        }
        /// <summary>
        /// Clears the input field.
        /// </summary>
        public static void Clear() {
            if (!GameService.GameIntegration.Gw2IsRunning) return;
            Task.Run(() => {
                Focus();
                Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
                Thread.Sleep(50);
                Keyboard.Release(VirtualKeyShort.LCONTROL, true);
                Keyboard.Stroke(VirtualKeyShort.BACK);
                Unfocus();
            });
        }

        private static void Focus() {
            Unfocus();
            Keyboard.Stroke(VirtualKeyShort.RETURN);
        }
        private static void Unfocus() {
            GameService.GameIntegration.FocusGw2();
            Mouse.Click(MouseButton.LEFT, GameService.Graphics.GraphicsDevice.Viewport.Width / 2, 0);
        }
        private static bool Valid(string text) {
            return (text != null && text.Length < 200);
            // More checks? (Symbols: https://wiki.guildwars2.com/wiki/User:MithranArkanere/Charset)
        }
    }
}