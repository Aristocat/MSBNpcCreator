using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MSBNpcCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadProperties();
        }

        private void LoadProperties()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Application.StartupPath + @"\properties.xml");
            foreach (var op in typeof(RecvOpcode).GetFields())
                op.SetValue(this, ParseProperty(
                    xml.SelectSingleNode("/Properties/Recv/" + op.Name).InnerText));
            foreach (var op in typeof(SendOpcode).GetFields())
                op.SetValue(this, ParseProperty(
                    xml.SelectSingleNode("/Properties/Send/" + op.Name).InnerText));
        }

        private ushort ParseProperty(string property)
        {
            ushort result;
            if (ushort.TryParse(property, out result))
                return result;
            result = (ushort)new System.ComponentModel.UInt16Converter().ConvertFromString(property);
            return result;
        }

        private void btBrowse_OnClick(object sender, EventArgs e)
        {
            using (OpenFileDialog fDialog = new OpenFileDialog())
            {
                fDialog.Title = "MapleShark Binary File Location";
                fDialog.Filter = "MapleShark Binary Files|*.msb";
                if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fieldPath.Text = fDialog.FileName;
            }
        }

        private void btStart_OnClick(object sender, EventArgs e)
        {
            try
            {
                BinaryHandler.ParseBinary(fieldPath.Text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                MessageBox.Show("Failed to load file.");
                return;
            }
            BinaryHandler.Script = null;
        }
    }
}
