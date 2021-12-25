using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Management;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Diagnostics;
/*kode hex warna paling kiri : #6d96ed
' kode hex warna tangah : #7279ed
' kode hex warna paling kanan : #c330f8 */


namespace animated_rain_loading
{
    /// <summary>
    /// <!-- ============================================================= -->
    ///<!-- I will still be a beginner                                     -->
    ///<!-- This program was developed by tobel                            -->
    ///<!-- Website : http://tobelsoft.my.id                               -->
    ///<!-- Whatsapp : https://wa.me/+14127755084                          -->
    ///<!-- Facebook : https://facebook.com/fiko.tobel                     -->
    ///<!-- Instagram : https://www.instagram.com/fikotobel_               -->
    ///<!-- GitHub : https://github.com/fiko942                            -->
    ///<!-- =============================================================  -->
    /// </summary>

    public partial class HWID_Spoofer : Form
    {
        //menambahkan fungsi moving form without border
        int mov;
        int movY;
        int movX;
        //----------------------------------------------


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
                int left,
                int top,
                int right,
                int bottom,
                int width,
                int height
            );

        int[] rainSpeeds = { 4, 6, 8, 3, 5, 6, 7, 4 };

        public class Adapter
        {
            public ManagementObject adapter;
            public string adaptername;
            public string customname;
            public int devnum;

            public Adapter(ManagementObject a, string aname, string cname, int n)
            {
                this.adapter = a;
                this.adaptername = aname;
                this.customname = cname;
                this.devnum = n;
            }

            public Adapter(NetworkInterface i) : this(i.Description) { }

            public Adapter(string aname)
            {
                this.adaptername = aname;

                var searcher = new ManagementObjectSearcher("select * from win32_networkadapter where Name='" + adaptername + "'");
                var found = searcher.Get();
                this.adapter = found.Cast<ManagementObject>().FirstOrDefault();

                try
                {
                    var match = Regex.Match(adapter.Path.RelativePath, "\\\"(\\d+)\\\"$");
                    this.devnum = int.Parse(match.Groups[1].Value);
                }
                catch
                {
                    return;
                }

                this.customname = NetworkInterface.GetAllNetworkInterfaces().Where(
                    i => i.Description == adaptername
                ).Select(
                    i => " (" + i.Name + ")"
                ).FirstOrDefault();
            }

            public NetworkInterface ManagedAdapter
            {
                get
                {
                    return NetworkInterface.GetAllNetworkInterfaces().Where(
                        nic => nic.Description == this.adaptername
                    ).FirstOrDefault();
                }
            }


            public string Mac
            {
                get
                {
                    try
                    {
                        return BitConverter.ToString(this.ManagedAdapter.GetPhysicalAddress().GetAddressBytes()).Replace("-", "").ToUpper();
                    }
                    catch { return null; }
                }
            }


            public string RegistryKey
            {
                get
                {
                    return String.Format(@"SYSTEM\ControlSet001\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{0:D4}", this.devnum);
                }
            }

            public string RegistryMac
            {
                get
                {
                    try
                    {
                        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, false))
                        {
                            return regkey.GetValue("NetworkAddress").ToString();
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            public bool SetRegistryMac(string value)
            {
                bool shouldReenable = false;

                try
                {

                    if (value.Length > 0 && !Adapter.IsValidMac(value, false))
                        throw new Exception(value + " bukan alamat mac yang valid");

                    using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, true))
                    {
                        if (regkey == null)
                            throw new Exception("Gagal membuka registry key");

                        if (regkey.GetValue("AdapterModel") as string != this.adaptername
                            && regkey.GetValue("DriverDesc") as string != this.adaptername)
                            throw new Exception("Adapter tidak ditemukan di registry");

                        string question = value.Length > 0 ?
                            "Yakin ingin mengubah alamat MAC adaptor {0} dari {1} menjadi {2} ?" :
                            "Yakin ingin mengembalikan custom alamat MAC dari adaptor {0} ?";
                        DialogResult proceed = MessageBox.Show(
                            String.Format(question, this.ToString(), this.Mac, value),
                            "HWID Spoofer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (proceed != DialogResult.Yes)
                            return false;

                        var result = (uint)adapter.InvokeMethod("Disable", null);
                        if (result != 0)
                            throw new Exception("Gagal menonaktifkan adapter");

                        shouldReenable = true;

                        if (value.Length > 0)
                            regkey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                        else
                            regkey.DeleteValue("NetworkAddress");


                        return true;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }

                finally
                {
                    if (shouldReenable)
                    {
                        uint result = (uint)adapter.InvokeMethod("Enable", null);
                        if (result != 0)
                            MessageBox.Show("Gagal mengaktifkan ulang adapter");
                    }
                }
            }

            public override string ToString()
            {
                return this.adaptername + this.customname;
            }

            public static string GetNewMac()
            {
                System.Random r = new System.Random();

                byte[] bytes = new byte[6];
                r.NextBytes(bytes);

                bytes[0] = (byte)(bytes[0] | 0x02);

                bytes[0] = (byte)(bytes[0] & 0xfe);

                return MacToString(bytes);
            }

            public static bool IsValidMac(string mac, bool actual)
            {
                if (mac.Length != 12)
                    return false;

                if (mac != mac.ToUpper())
                    return false;

                if (!Regex.IsMatch(mac, "^[0-9A-F]*$"))
                    return false;

                if (actual)
                    return true;

                char c = mac[1];
                return (c == '2' || c == '6' || c == 'A' || c == 'E');
            }

            public static bool IsValidMac(byte[] bytes, bool actual)
            {
                return IsValidMac(Adapter.MacToString(bytes), actual);
            }

            public static string MacToString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }
        }
        public HWID_Spoofer()
        {
            InitializeComponent();
            //menambahkan fungsi draw border
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 7, 7));
            //menambahkan fungsi konfirmasi exit
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClosingEventCancle_Closing);
        }

        private void HWID_Spoofer_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            guna2ShadowForm1.SetShadowForm(this);
            guna2AnimateWindow1.SetAnimateWindow(this);
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(
                    a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)
                ).OrderByDescending(a => a.Speed))
            {
                AdaptersComboBox.Items.Add(new Adapter(adapter));
            }

            AdaptersComboBox.SelectedIndex = 0;
        }
        private void UpdateAddresses()
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;
            this.CurrentMacTextBox.Text = a.RegistryMac;
            this.label1.Text = "Active MAC address : " + a.Mac;
        }

        private void AdaptersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAddresses();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            CurrentMacTextBox.Text = Adapter.GetNewMac();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            SetRegistryMac("");
        }

        private void SetRegistryMac(string mac)
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;

            if (a.SetRegistryMac(mac))
            {
                System.Threading.Thread.Sleep(100);
                UpdateAddresses();
                MessageBox.Show("Berhasil mengubah alamat MAC", "HWID Spoofer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RereadButton_Click(object sender, EventArgs e)
        {
            UpdateAddresses();
        }

        private void CurrentMacTextBox_TextChanged(object sender, EventArgs e)
        {
            this.guna2GradientButton1.Enabled = Adapter.IsValidMac(this.CurrentMacTextBox.Text, false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            UpdateAddresses();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            CurrentMacTextBox.Text = Adapter.GetNewMac();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (!Adapter.IsValidMac(CurrentMacTextBox.Text, false))
            {
                MessageBox.Show("Alamat MAC tidak valid", "HWID Spoofer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetRegistryMac(CurrentMacTextBox.Text);
        }
        private void FormClosingEventCancle_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //jika ditekan close dimanapun maka akan menampilkan konfirmasi penutupan aplikasi
            DialogResult dr = MessageBox.Show("Are you sure to close this program now ?", "HWID Spoofer | TobelSoft", MessageBoxButtons.YesNo);
            if (dr == DialogResult.No)
                //jika tombol yes ditekan maka akan membatalkan penutupan aplikasi
                e.Cancel = true;
            else
                //jika tombol no ditekan maka akan menutup aplikasi
                Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            }
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://tobelsoft.my.id");
        }

        private void Button2_Click_1(object sender, System.EventArgs e)
        {
            UpdateAddresses();
        }

        private void buttonGenerate_Click(object sender, System.EventArgs e)
        {
            CurrentMacTextBox.Text = Adapter.GetNewMac();
        }

        private void ClearButton_Click_1(object sender, System.EventArgs e)
        {
            SetRegistryMac("");
        }

        private void UpdateButton_Click(object sender, System.EventArgs e)
        {
            if (!Adapter.IsValidMac(CurrentMacTextBox.Text, false))
            {
                MessageBox.Show("Alamat MAC tidak valid", "HWID Spoofer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetRegistryMac(CurrentMacTextBox.Text);
        }

        private void guna2ControlBox1_Click(object sender, System.EventArgs e)
        {
            
        }

        private void guna2GradientButton1_Click(object sender, System.EventArgs e)
        {
            SetRegistryMac("");
        }

        private void guna2GradientButton2_Click(object sender, System.EventArgs e)
        {
            if (!Adapter.IsValidMac(CurrentMacTextBox.Text, false))
            {
                MessageBox.Show("Alamat MAC tidak valid", "HWID Spoofer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetRegistryMac(CurrentMacTextBox.Text);
        }

        private void guna2GradientButton2_Click_1(object sender, System.EventArgs e)
        {
            CurrentMacTextBox.Text = Adapter.GetNewMac();
        }

        private void CurrentMacTextBox_TextChanged_1(object sender, System.EventArgs e)
        {

        }

        private void guna2GradientButton2_Click_2(object sender, System.EventArgs e)
        {
            UpdateAddresses();
        }

        private void guna2GradientPanel4_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }

        private void guna2GradientButton3_Click(object sender, System.EventArgs e)
        {
            
        }

        private void guna2GradientButton4_Click(object sender, System.EventArgs e)
        {
            
        }

        
        private void guna2GradientButton6_Click(object sender, System.EventArgs e)
        {
            
        }

        private void guna2GradientButton5_Click_1(object sender, System.EventArgs e)
        {
            
        }
        private void pnlCheat_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }

        private void send_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void controller_Tick(object sender, EventArgs e)
        {

        }
    }
}
