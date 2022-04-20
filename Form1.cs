using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Memory;

namespace CheatTet0001
{
    public partial class Form1 : Form
    {
        VAMemory vam;
        // Bases
        int BaseAddress = 0x00400000;
        int OffsetLocalPlayer = 0x17E0A8;
        int LocalPlayer;
        // Weapons
        int OffsetHealth = 0x00EC;
        int OffsetAmmoAssaultRifle = 0x0140;
        int OffsetAvaliableAmmoAssaultRifle = 0x011c;
        int OffsetAmmoSubmachineGun = 0x0138;
        int OffsetAvaliableAmmoSubmachineGun = 0x0114;
        int OffsetAmmoSniper = 0x013C;
        int OffsetAvaliableAmmoSniper = 0x0118;
        int OffsetAmmoShotgun = 0x0134;
        int OffsetAvaliableAmmoShotgun = 0x0110;
        int OffsetAmmoCabine = 0x0130;
        int OffsetAvaliableAmmoCabine = 0x010C;
        int OffsetAmmoPistol = 0x012C;
        int OffsetAvaliableAmmoPistol = 0x0108;
        int OffsetAmmoDualPistol = 0x0148;
        int OffsetAvaliableAmmoDualPistol = 0x0124;
        // Utility
        int OffsetGranades = 0x0144;
        // Position
        int OffsetX = 0x0028;
        int OffsetY = 0x002C;
        int OffsetZ = 0x0030;

        Thread LoopThread;
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            LoopThread = new Thread(Loop);
        }
        public int GetModuleAddress(string pName, string dllName)
        {
            Process p = Process.GetProcessesByName(pName).FirstOrDefault();
            foreach (ProcessModule pm in p.Modules)
                if (pm.ModuleName.Equals(dllName))
                    return (int)pm.BaseAddress;
            return 0;
        }

        private void Loop()
        {
            vam = new VAMemory("ac_client");
            InfoProcess.Text = vam.processName;
            LocalPlayer = vam.ReadInt32((IntPtr)(BaseAddress + OffsetLocalPlayer));

            ToggleGodmode.Enabled = true;
            ToggleInfiniteAmmo.Enabled = true;
            ToggleInfiniteGranades.Enabled = true;

            IntPtr HealthAddr = (IntPtr)(LocalPlayer + OffsetHealth);
            IntPtr AssaultRifleAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoAssaultRifle);
            IntPtr SubmachineGunAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoSubmachineGun);
            IntPtr SniperAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoSniper);
            IntPtr ShotgunAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoShotgun);
            IntPtr CabineAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoCabine);
            IntPtr PistolAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoPistol);
            IntPtr DualPistolAmmoAddr = (IntPtr)(LocalPlayer + OffsetAmmoDualPistol);
            IntPtr GranadesAddr = (IntPtr)(LocalPlayer + OffsetGranades);
            IntPtr XAddr = (IntPtr)(LocalPlayer + OffsetX);
            IntPtr YAddr = (IntPtr)(LocalPlayer + OffsetY);
            IntPtr ZAddr = (IntPtr)(LocalPlayer + OffsetZ);

            //vam.WriteInt32(AmmoAddr, 9999);

            while (true)
            {
                if (vam.CheckProcess())
                {
                    if (ToggleGodmode.Checked)
                    {
                        vam.WriteInt32(HealthAddr, 9999);
                    }
                    if (ToggleInfiniteAmmo.Checked)
                    {
                        vam.WriteInt32(AssaultRifleAmmoAddr, 100);
                        vam.WriteInt32(SubmachineGunAmmoAddr, 100);
                        vam.WriteInt32(SniperAmmoAddr, 100);
                        vam.WriteInt32(ShotgunAmmoAddr, 100);
                        vam.WriteInt32(CabineAmmoAddr, 100);
                        vam.WriteInt32(PistolAmmoAddr, 100);
                        vam.WriteInt32(DualPistolAmmoAddr, 100);
                    }
                    if (ToggleInfiniteGranades.Checked)
                    {
                        vam.WriteInt32(GranadesAddr, 3);
                    }
                    InfoAmmo.Text = vam.ReadInt32(AssaultRifleAmmoAddr).ToString();
                    InfoHealth.Text = vam.ReadInt32(HealthAddr).ToString();
                    InfoX.Text = vam.ReadDouble(XAddr).ToString().Substring(0,4);
                    InfoY.Text = vam.ReadDouble(YAddr).ToString().Substring(0, 4);
                    InfoZ.Text = vam.ReadDouble(ZAddr).ToString().Substring(0, 4);
                    Thread.Sleep(100);
                } else
                {
                    StartBtn.Enabled = true;
                    InfoProcess.Text = "Closed";
                    break;
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            StartBtn.Enabled = false;
            LoopThread.Start();
        }

        private void ToggleGodmode_CheckedChanged(object sender, EventArgs e)
        {
            if (!ToggleGodmode.Checked)
            {
                IntPtr HealthAddr = (IntPtr)(LocalPlayer + OffsetHealth);
                vam.WriteInt32(HealthAddr, 100);
            }
        }
    }
}
