﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Dynaframe3
{
    public static class Helpers
    {
        // Fisher Yates shuffle - source: https://stackoverflow.com/questions/273313/randomize-a-listt/4262134#4262134
        public static IList<T> Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = list.Count; i > 0; i--)
                list.Swap(0, rnd.Next(0, i));
            return list;
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        /// <summary>
        /// Source: https://stackoverflow.com/questions/3527203/getfiles-with-multiple-extensions
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            return dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
                          .Where(f => allowedExtensions.Contains(f.Extension)).Select(s => s.FullName);
        }

        /// <summary>
        /// Gets the ip address of the system. Dynaframe uses this to help the user locate thier device
        /// on the network
        /// </summary>
        /// <returns></returns>
        public static string GetIPString()
        {
            string returnval = "";
            NetworkInterface[] nets = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            if (nets.Length > 0)
            {
                foreach (NetworkInterface net in nets)
                {
                    try
                    {
                        var addresses = net.GetIPProperties().UnicastAddresses;
                        for (int i = 0; i < addresses.Count; i++)
                        {
                            string ip = addresses[i].Address.ToString();
                            // Filter out IPV6, local, loopback, etc.
                            if((!ip.StartsWith("169.")) && (!ip.StartsWith("127.")) && (!ip.Contains("::")))
                                returnval += "Setup at http://" + ip + ":8000 "+ Environment.NewLine;
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }
            return returnval;

        }

        /// <summary>
        /// Runs a process and exits. If there is a failure or exception, returns false
        /// </summary>
        /// <param name="process">Process path and name</param>
        /// <param name="args">any arguments to pass</param>
        /// <returns>Process ID if it runs, else -1 if something fails</returns>

        public static int RunProcess(string patoToProcess, string args)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(patoToProcess);
                if (!String.IsNullOrEmpty(args))
                {
                    info.Arguments = args;
                }
                Process process = new Process();
                process.StartInfo = info;
                process.Start();
                return process.Id;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }   
}
