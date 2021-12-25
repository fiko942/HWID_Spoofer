using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


namespace animated_rain_loading
{
    public partial class Loading : Form
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
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
                int left,
                int top,
                int right,
                int bottom,
                int width,
                int height
            );

        int[] rainSpeeds = {4, 6, 8, 3, 5, 6, 7, 4};
        int loadingSpeed = 2;
        float initialPercentage = 0;

        public Loading()
        {
            InitializeComponent();
            //menambahkan fungsi konfirmasi exit
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClosingEventCancle_Closing);
            //menambahkan fungsi bordered form
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 7, 7));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);
            timer1.Start();
            timer2.Start();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        //animation for rain 1
                        pictureBox3.Location = new Point(pictureBox3.Location.X, pictureBox3.Location.Y + rainSpeeds[i]);
                        if (pictureBox3.Location.Y > panel1.Size.Height + pictureBox3.Size.Height)
                        {
                            pictureBox3.Location = new Point(pictureBox3.Location.X, 0 - pictureBox3.Size.Height);
                        }
                        break;
                    case 1:
                        //animation for rain 2
                        pictureBox4.Location = new Point(pictureBox4.Location.X, pictureBox4.Location.Y + rainSpeeds[i]);
                        if (pictureBox4.Location.Y > panel1.Size.Height + pictureBox4.Size.Height)
                        {
                            pictureBox4.Location = new Point(pictureBox4.Location.X, 0 - pictureBox4.Size.Height);
                        }
                        break;
                    case 2:
                        //animation for rain 3
                        pictureBox5.Location = new Point(pictureBox5.Location.X, pictureBox5.Location.Y + rainSpeeds[i]);
                        if (pictureBox5.Location.Y > panel1.Size.Height + pictureBox5.Size.Height)
                        {
                            pictureBox5.Location = new Point(pictureBox5.Location.X, 0 - pictureBox5.Size.Height);
                        }
                        break;
                    case 3:
                        //animation for rain 4
                        pictureBox6.Location = new Point(pictureBox6.Location.X, pictureBox6.Location.Y + rainSpeeds[i]);
                        if (pictureBox6.Location.Y > panel1.Size.Height + pictureBox6.Size.Height)
                        {
                            pictureBox6.Location = new Point(pictureBox6.Location.X, 0 - pictureBox6.Size.Height);
                        }
                        break;
                    case 4:
                        //animation for rain 5
                        pictureBox7.Location = new Point(pictureBox7.Location.X, pictureBox7.Location.Y + rainSpeeds[i]);
                        if (pictureBox7.Location.Y > panel1.Size.Height + pictureBox7.Size.Height)
                        {
                            pictureBox7.Location = new Point(pictureBox7.Location.X, 0 - pictureBox7.Size.Height);
                        }
                        break;
                    case 5:
                        //animation for rain 6
                        pictureBox8.Location = new Point(pictureBox8.Location.X, pictureBox8.Location.Y + rainSpeeds[i]);
                        if (pictureBox8.Location.Y > panel1.Size.Height + pictureBox8.Size.Height)
                        {
                            pictureBox8.Location = new Point(pictureBox8.Location.X, 0 - pictureBox8.Size.Height);
                        }
                        break;
                    case 6:
                        //animation for rain 7
                        pictureBox9.Location = new Point(pictureBox9.Location.X, pictureBox9.Location.Y + rainSpeeds[i]);
                        if (pictureBox9.Location.Y > panel1.Size.Height + pictureBox9.Size.Height)
                        {
                            pictureBox9.Location = new Point(pictureBox9.Location.X, 0 - pictureBox9.Size.Height);
                        }
                        break;
                    case 7:
                        //animation for rain 8
                        pictureBox10.Location = new Point(pictureBox10.Location.X, pictureBox10.Location.Y + rainSpeeds[i]);
                        if (pictureBox10.Location.Y > panel1.Size.Height + pictureBox10.Size.Height)
                        {
                            pictureBox10.Location = new Point(pictureBox10.Location.X, 0 - pictureBox10.Size.Height);
                        }
                        break;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            initialPercentage += loadingSpeed;
            float percentage = initialPercentage / pictureBox2.Height * 100;

            label1.Text = (int)percentage + " %";

            panel2.Location = new Point(panel2.Location.X, panel2.Location.Y + loadingSpeed);
            if (panel2.Location.Y > pictureBox2.Location.Y + pictureBox2.Height)
            {
                label1.Text = "100 %";
                this.Hide();
                HWID_Spoofer f2 = new HWID_Spoofer();
                f2.Activate();
                f2.ShowInTaskbar = true;
                f2.Focus();
                f2.Show();
                this.Hide();
                this.timer2.Stop();

                Thread.Sleep(2000);
                if (f2.Focused == false)
                {
                    f2.Focus();
                }

                if (f2.Focused == true)
                {
                    fixshowintaskbar.Enabled = false;
                    fixshowintaskbar.Stop();
                }
            }
        }

        private void fixshowintaskbar_Tick(object sender, EventArgs e)
        {
            
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
