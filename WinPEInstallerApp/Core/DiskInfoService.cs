using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Forms;

namespace WinPEInstallerApp.Core
{
    public class DiskInfoService
    {
        public static void PopulateDiskTree(TreeView tv)
        {
            tv.Nodes.Clear();

            // 1. Get all Physical Disks
            var diskQuery = new ManagementObjectSearcher("SELECT DeviceID, Model, Size FROM Win32_DiskDrive");
            foreach (ManagementObject disk in diskQuery.Get())
            {
                string diskName = $"{disk["Model"]} ({FormatBytes(disk["Size"])})";
                TreeNode diskNode = new TreeNode(diskName) { Tag = disk["DeviceID"] };

                // 2. Get Partitions associated with THIS physical disk
                // This specific WMI query connects the physical drive to its logical partitions
                string partQueryText = $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{disk["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                var partQuery = new ManagementObjectSearcher(partQueryText);

                foreach (ManagementObject partition in partQuery.Get())
                {
                    string partName = $"{partition["Name"]} - {FormatBytes(partition["Size"])}";
                    TreeNode partNode = new TreeNode(partName) { Tag = partition["DeviceID"] };
                    diskNode.Nodes.Add(partNode);
                }

                tv.Nodes.Add(diskNode);
            }
            tv.ExpandAll(); // Keep tree expanded for easy selection
        }

        private static string FormatBytes(object bytes)
        {
            double b = Convert.ToDouble(bytes);
            return $"{Math.Round(b / 1024 / 1024 / 1024, 2)} GB";
        }
    }
}