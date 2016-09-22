using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace school_color_changer {

    public partial class Schoolsucks : Form {

        private readonly string _programTitle = "Photo filter tool";
        private Bitmap _originalBitmap;
        private Bitmap _resultBitmap;

        public Schoolsucks ( )
        {
            InitializeComponent ( );
        }

        private static IEnumerable < string > get_files_in_directory ( string path )
        {
            return Directory.EnumerateFiles ( path ).ToArray ( );
        }

        private void proc_images ( )
        {
            var x = get_files_in_directory ( Directory.GetCurrentDirectory ( ) + "\\Ready" );

            foreach ( var variable in x ) {
                var streamReader = new StreamReader ( variable );
                _originalBitmap = ( Bitmap ) Image.FromStream ( streamReader.BaseStream );
                streamReader.Close ( );
                ApplyFilter ( );
                var fileExtension = Path.GetExtension ( variable ).ToUpper ( );
                var fileName = Path.GetFileName ( variable );

                var imgFormat = ImageFormat.Png;

                if ( fileExtension == "BMP" )
                    imgFormat = ImageFormat.Bmp;
                else if ( fileExtension == "JPG" )
                    imgFormat = ImageFormat.Jpeg;
                if ( _resultBitmap != null ) {
                    if ( !Directory.Exists ( "Finished" ) )
                        Directory.CreateDirectory ( "Finished" );

                    if ( ( string ) comboBox1.SelectedItem == "School Black" )
                        _resultBitmap = Image.FromFile ( variable ).grayscale_copy ( );

                    var sav = Directory.GetCurrentDirectory ( ) + "\\Finished\\" + fileName;
                    var streamWriter = new StreamWriter ( sav, false );
                    _resultBitmap.Save ( streamWriter.BaseStream, imgFormat );
                    streamWriter.Flush ( );
                    streamWriter.Close ( );
                }
                _resultBitmap = null;
            }
        }

        private void ApplyFilter ( )
        {
            _resultBitmap = _originalBitmap.color_tinter (
                trackBar1.Value / 100.0f,
                trackBar2.Value / 100.0f,
                trackBar3.Value / 100.0f );
        }

        private void button1_Click ( object sender, EventArgs e )
        {
            proc_images ( );
        }

        private void Form1_Load ( object sender, EventArgs e )
        {
            if ( !Directory.Exists ( "Ready" ) ) {
                Directory.CreateDirectory ( "Ready" );
                MessageBox.Show (
                    "It seems that the folder \"Ready\" was not created, I have done it for you. Please put your images in the folder and press the filter button.",
                    _programTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information );
            }
        }

        private void linkLabel1_LinkClicked ( object sender, LinkLabelLinkClickedEventArgs e )
        {
            Process.Start ( "http://dex73r.xyz/" );
        }

        private void comboBox1_SelectedIndexChanged ( object sender, EventArgs e )
        {
            foreach ( var comboBox1Item in comboBox1.Items )
                if ( comboBox1.SelectedItem.Equals ( "Pretty Blue" ) ) {
                    trackBar1.Value = 40;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                }
                else if ( comboBox1.SelectedItem.Equals ( "Pretty Green" ) ) {
                    trackBar1.Value = 0;
                    trackBar2.Value = 20;
                    trackBar3.Value = 0;
                }
                else if ( comboBox1.SelectedItem.Equals ( "Pretty Red" ) ) {
                    trackBar1.Value = 0;
                    trackBar2.Value = 0;
                    trackBar3.Value = 20;
                }
                else if ( comboBox1.SelectedItem.Equals ( "School Yellow" ) ) {
                    trackBar1.Value = 0;
                    trackBar2.Value = 50;
                    trackBar3.Value = 50;
                }
                else if ( comboBox1.SelectedItem.Equals ( "School Black" ) ) {
                    // disabled anyways
                    trackBar1.Value = 0;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                }
                else if ( comboBox1.SelectedItem.Equals ( "School Red" ) ) {
                    trackBar1.Value = 0;
                    trackBar2.Value = 0;
                    trackBar3.Value = 40;
                }
        }

    }

}